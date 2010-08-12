using System;
using System.Web;
using System.IO;

using Cuyahoga.Core;
using Cuyahoga.Core.Util;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Util;

using Cuyahoga.Core.Service.Membership;

namespace Cuyahoga.Web.UI
{
	/// <summary>
	/// Base class for all module user controls.
	/// Credits to the DotNetNuke team (http://www.dotnetnuke.com) for the output caching idea!
	/// </summary>
	public class BaseModuleControl : LocalizedUserControl
	{
		private ModuleBase _module;
		private PageEngine _pageEngine;
		private string _cachedOutput;
		private bool _displaySyndicationIcon;

        private User _cuyahogaUser;

		/// <summary>
		/// Indicator if there is cached content. The derived ModuleControls should determine whether to
		/// load content or not.
		/// </summary>
		protected bool HasCachedOutput
		{
			get { return this._cachedOutput != null; }
		}

		/// <summary>
		/// Indicate if the module should display the syndication icon at its current state.
		/// </summary>
		protected bool DisplaySyndicationIcon
		{
			get { return this._displaySyndicationIcon; }
			set { this._displaySyndicationIcon = value; }
		}

		/// <summary>
		/// The PageEngine that serves as the context for the module controls.
		/// </summary>
		public PageEngine PageEngine
		{
			get { return this._pageEngine; }
		}

		/// <summary>
		/// The accompanying ModuleBase business object. Use this property  to access
		/// module properties, sections and nodes from the code-behind of the module user controls.
		/// </summary>
		public ModuleBase Module
		{
			get { return this._module; }
			set { this._module = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BaseModuleControl()
		{
			// Show the syndication icon by default. It can be set by subclasses.
			this._displaySyndicationIcon = (this is ISyndicatable);
		}

		protected override void OnInit(EventArgs e)
		{
			if (this.Module.Section.CacheDuration > 0
				&& this.Module.CacheKey != null
				&& !this.Page.User.Identity.IsAuthenticated
				&& !this.Page.IsPostBack)
			{
				// Get the cached content. Don't use cached output after a postback.
				if (HttpContext.Current.Cache[this.Module.CacheKey] != null && !this.IsPostBack)
				{
					// Found cached content.
					this._cachedOutput = HttpContext.Current.Cache[this.Module.CacheKey].ToString();
				}
			}
			if (this.Page is PageEngine)
			{
				this._pageEngine = (PageEngine)this.Page;
			}

            //custom for admin panel popup on site pages
            _cuyahogaUser = this.Page.User.Identity as User;
            if (this._cuyahogaUser != null && (this._cuyahogaUser.CanEdit(this._module.Section) || this._cuyahogaUser.IsInRole("Administrator")))//.HasPermission(AccessLevel.Administrator)))
            {
                //jQuery script for pop-up admin panel
                string adminjqpath = String.Format("{0}js/jquery-1.4.1.min.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString());
                this._pageEngine.RegisterJavascript("jquery", adminjqpath);

                string adminjspath = String.Format("{0}admin/js/adminpanel.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString());
                this._pageEngine.RegisterJavascript("popadminjs", adminjspath);

                //CSS for popup panel
                string admincsspath = String.Format("{0}admin/css/adminpanel.css", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString());
                this._pageEngine.RegisterStylesheet("popadmincss", admincsspath);
            }

			base.OnInit(e);
		}

		/// <summary>
		/// Wrap the section content in a visual block.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			// Rss feed
			if (this._displaySyndicationIcon)
			{
				writer.Write(String.Format("<div class=\"syndicate\"><a href=\"{0}\"><img src=\"{1}\" alt=\"RSS-2.0\"/></a></div>",
					UrlHelper.GetRssUrlFromSection(this._module.Section) + this._module.ModulePathInfo, UrlHelper.GetApplicationPath() + "Images/feed-icon.png"));
			}

            //User cuyahogaUser = this.Page.User.Identity as User;
            if (this._cuyahogaUser != null && (this._cuyahogaUser.CanEdit(this._module.Section) || this._cuyahogaUser.HasRight(Rights.AccessAdmin)))//.HasPermission(AccessLevel.Administrator)))
            {
                string paneltitle = this._module.DisplayTitle.Length > 15 ? this._module.DisplayTitle.Substring(0, 12) + "..." : this._module.DisplayTitle;
                writer.Write(String.Format("<img class=\"editorcontextmenu\" src=\"{0}\"/><div class=\"moduletools\"><div class=\"moduletitle\"> " + Text.TruncateText(paneltitle, 16) + " </div>", UrlHelper.GetApplicationPath() + "admin/images/arrow-down-green.png"));
                // added for 1.6.0
                bool isPreview = (Request.QueryString["preview"] == null
                                    ? false
                                    : (Request.QueryString["preview"] == "on"));

                // Edit button
                if (!isPreview) // modified for 1.6.0
                {
                    if (this._module.Section.ModuleType.EditPath != null
                        && this._cuyahogaUser.CanEdit(this._module.Section))
                    {
                        if (this._module.Section.Node != null)
                        {
                            writer.Write(String.Format("&nbsp;<a id=\"editcontent\" href=\"{0}?NodeId={1}&amp;SectionId={2}&amp;SiteId={3}\"><span>Edit</span></a>"
                                                       , UrlHelper.GetApplicationPath() + this._module.Section.ModuleType.EditPath
                                                       , this._module.Section.Node.Id
                                                       , this._module.Section.Id
                                                       , this._module.Section.Site.Id));
                        }
                        else
                        {
                            writer.Write(String.Format("&nbsp;<a id=\"editcontent\" href=\"{0}?NodeId={1}&amp;SectionId={2}&amp;SiteId={3}\"><span>Edit</span></a>"
                                                       , UrlHelper.GetApplicationPath() + this._module.Section.ModuleType.EditPath
                                                       , this.PageEngine.ActiveNode.Id
                                                       , this._module.Section.Id
                                                       , this._module.Section.Site.Id));
                        }
                    }
                    if (this._cuyahogaUser.HasRight(Rights.AccessAdmin))//.HasPermission(AccessLevel.Administrator))
                    {
                        if (this._module.Section.Node != null)
                        {
                            writer.Write(
                                String.Format(
                                    "&nbsp;<a id=\"sectionproperties\" href=\"{0}Admin/SectionEdit.aspx?NodeId={1}&amp;SectionId={2}&amp;SiteId={3}\"><span>Section Properties</span></a>"
                                    , UrlHelper.GetApplicationPath()
                                    , this._module.Section.Node.Id
                                    , this._module.Section.Id
                                    , this._module.Section.Site.Id));
                            writer.Write(
                                String.Format(
                                    "&nbsp;<a id=\"pageproperties\" href=\"{0}Admin/NodeEdit.aspx?NodeId={1}&amp;SiteId={2}\"><span>Page Properties</span></a>"
                                    , UrlHelper.GetApplicationPath()
                                    , this._module.Section.Node.Id
                                    , this._module.Section.Site.Id));
                        }
                        else
                        {
                            writer.Write(String.Format("&nbsp;<a id=\"sectionproperties\" href=\"{0}Admin/SectionEdit.aspx?SectionId={1}&amp;SiteId={2}\"><span>Section Properties</span></a>"
                                                       , UrlHelper.GetApplicationPath()
                                                       , this._module.Section.Id
                                                       , this._module.Section.Site.Id));
                        }
                    }
                }
                writer.Write("</div>");
            }
            //Original
            //writer.Write("<div class=\"section\">");

            //Custom
            //If a Section CSSClass is provided then use it
            if (!string.IsNullOrEmpty(this._module.Section.CSSClass))
            {
                writer.Write("<div class=\"" + this._module.Section.CSSClass + "\">");
            }
            else
            {
                writer.Write("<div class=\"section\">");
            }

			// Section title
			if (this._module.Section != null && this._module.Section.ShowTitle)
			{
				writer.Write("<h3>" + this._module.DisplayTitle + "</h3>");
			}

			// Write module content and handle caching when neccesary.
			// Don't cache when the user is logged in or after a postback.
			if (this._module.Section.CacheDuration > 0
				&& this.Module.CacheKey != null
				&& !this.Page.User.Identity.IsAuthenticated
				&& !this.Page.IsPostBack)
			{
				if (this._cachedOutput == null)
				{
					StringWriter tempWriter = new StringWriter();
					base.Render(new System.Web.UI.HtmlTextWriter(tempWriter));
					this._cachedOutput = tempWriter.ToString();
					HttpContext.Current.Cache.Insert(this.Module.CacheKey, this._cachedOutput, null
						, DateTime.Now.AddSeconds(this._module.Section.CacheDuration), TimeSpan.Zero);
				}
				// Output the user control's content.
				writer.Write(_cachedOutput);
			}
			else
			{
				base.Render(writer);
			}
			writer.Write("</div>");
		}

		/// <summary>
		/// Empty the output cache for the current module state.
		/// </summary>
		protected void InvalidateCache()
		{
			if (this.Module.CacheKey != null)
			{
				HttpContext.Current.Cache.Remove(this.Module.CacheKey);
			}
		}

		/// <summary>
		/// Register module-specific stylesheets.
		/// </summary>
		/// <param name="key">The unique key for the stylesheet. Note that Cuyahoga already uses 'maincss' as key.</param>
		/// <param name="absoluteCssPath">The path to the css file from the application root (starting with /).</param>
		protected void RegisterStylesheet(string key, string absoluteCssPath)
		{
			this._pageEngine.RegisterStylesheet(key, absoluteCssPath);
		}

		/// <summary>
		/// Register module-specific javascripts.
		/// </summary>
		/// <param name="key">The unique key for the script.</param>
		/// <param name="absoluteJavaScriptPath">The path to the javascript file from the application root.</param>
		protected void RegisterJavascript(string key, string absoluteJavaScriptPath)
		{
			this._pageEngine.RegisterJavascript(key, absoluteJavaScriptPath);
		}
	}
}

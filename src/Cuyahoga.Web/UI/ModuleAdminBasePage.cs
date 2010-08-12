using System;
using System.Web;
using Cuyahoga.Core;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Search;
using Cuyahoga.Core.Service.Search;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.Components;

namespace Cuyahoga.Web.UI
{
	/// <summary>
	/// Summary description for ModuleAdminBasePage.
	/// </summary>
	public class ModuleAdminBasePage : GenericBasePage
	{
		private Node _node;
		private Section _section;
		private ModuleBase _module;
		private ModuleLoader _moduleLoader;
        private ISearchService _searchService;
        private ISectionService _sectionService;
        private INodeService _nodeService;

		/// <summary>
		/// Property Node (Node)
		/// </summary>
		public Node Node
		{
			get { return this._node; }
		}

		/// <summary>
		/// Property Section (Section)
		/// </summary>
		public Section Section
		{
			get { return this._section; }
		}

		/// <summary>
		/// Property Module (ModuleBase)
		/// </summary>
		protected ModuleBase Module
		{
			get { return this._module; }
		}

		/// <summary>
		/// Default constructor calls base constructor with parameters for templatecontrol, 
		/// templatepath and stylesheet.
		/// </summary>
		public ModuleAdminBasePage()
			: base("ModuleAdminTemplate.ascx", "~/Controls/", "~/Admin/Css/Admin.css")
		{
			this._node = null;
			this._section = null;

			this._moduleLoader = Container.Resolve<ModuleLoader>();
            this._searchService = Container.Resolve<ISearchService>();
            this._sectionService = Container.Resolve<ISectionService>();
            this._nodeService = Container.Resolve<INodeService>();
		}

		#region Register Javascript and CSS
		/// <summary>
		/// Register module-specific stylesheets.
		/// </summary>
		/// <param name="key">The unique key for the stylesheet. Note that Cuyahoga already uses 'maincss' as key.</param>
		/// <param name="absoluteCssPath">The path to the css file from the application root (starting with /).</param>
		public void RegisterAdminStylesheet(string key, string absoluteCssPath)
		{
			base.RegisterStylesheet(key, absoluteCssPath);
		}

		/// <summary>
		/// Register module-specific javascripts.
		/// </summary>
		/// <param name="key">The unique key for the javascript. </param>
		/// <param name="absoluteJavascriptPath">The path to the javascript file from the application root (starting with /).</param>
		public void RegisterAdminJavascript(string key, string absoluteJavascriptPath)
		{
			base.RegisterJavascript(key, absoluteJavascriptPath);
		}
		#endregion Register Javascript and CSS

		/// <summary>
		/// In the OnInit method the Node and Section of every ModuleAdminPage is set. 
		/// An exception is thrown when one of them cannot be set.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			try
			{
				int nodeId = Int32.Parse(Context.Request.QueryString["NodeId"]);
                this._node = _nodeService.GetNodeById(nodeId);
				
                int sectionId = Int32.Parse(Context.Request.QueryString["SectionId"]);
                this._section = _sectionService.GetSectionById(sectionId);

				this._module = this._moduleLoader.GetModuleFromSection(this._section);

                //Put SiteId in a session so the FCKEditor pop up can get it (Custom hack)
                //Session["SiteId"] = this._node.Site.Id;

                //Init Folders
                //ClientFoldersInit((Cuyahoga.Web.Util.UrlHelper.GetApplicationPath() + "SiteData/"), this._node.Site.Id);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to initialize the Module Admin page because a Node or a Section could not be created.", ex);
			}
			// Check permissions
			if (!Context.User.Identity.IsAuthenticated)
			{
				throw new AccessForbiddenException("You are not logged in.");
			}
			else
			{
				User user = (User) Context.User.Identity;
				if (!user.CanEdit(this._section))
				{
					throw new ActionForbiddenException("You are not allowed to edit the section.");
				}
			}

			// Optional indexing event handlers
			if (this._module is ISearchable && Boolean.Parse(Config.GetConfiguration()["InstantIndexing"]))
			{
				ISearchable searchableModule = (ISearchable)this._module;
				searchableModule.ContentCreated += new IndexEventHandler(searchableModule_ContentCreated);
				searchableModule.ContentUpdated += new IndexEventHandler(searchableModule_ContentUpdated);
				searchableModule.ContentDeleted += new IndexEventHandler(searchableModule_ContentDeleted);
			}

			// Set FCKEditor context (used by some module admin pages)
			// It would be nicer if we could do this in the Global.asax, but there the 
			// ultra-convenient ~/Path (ResolveUrl) isn't available :).

            //For multi-site
            string userFilesPath = Cuyahoga.Web.Util.UrlHelper.GetApplicationPath() + "SiteData/" + this._node.Site.Id.ToString();
            string userFilesAbsolutePath = Server.MapPath("~/SiteData/") + this._node.Site.Id.ToString();

			if (userFilesPath != null && HttpContext.Current.Application["FCKeditor:UserFilesPath"] == null)
			{
				HttpContext.Current.Application.Lock();
				HttpContext.Current.Application["FCKeditor:UserFilesPath"] = ResolveUrl(userFilesPath);
				HttpContext.Current.Application.UnLock();
			}

            if (userFilesAbsolutePath != null && HttpContext.Current.Application["FCKeditor:UserFilesAbsolutePath"] == null)
			{
				HttpContext.Current.Application.Lock();
                HttpContext.Current.Application["FCKeditor:UserFilesAbsolutePath"] = userFilesAbsolutePath;
				HttpContext.Current.Application.UnLock();
			}

			base.OnInit(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetBaseQueryString()
		{
			return String.Format("?NodeId={0}&SectionId={1}", this.Node.Id, this.Section.Id);
		}

		private void IndexContent(SearchContent searchContent, IndexAction action)
		{
			switch (action)
			{
				case IndexAction.Create:
                    this._searchService.AddContent(searchContent);
					break;
				case IndexAction.Update:
                    this._searchService.UpdateContent(searchContent);
					break;
				case IndexAction.Delete:
                    this._searchService.DeleteContent(searchContent);
					break;
			}
	
		}

		private void searchableModule_ContentCreated(object sender, IndexEventArgs e)
		{
			IndexContent(e.SearchContent, IndexAction.Create);
		}

		private void searchableModule_ContentUpdated(object sender, IndexEventArgs e)
		{
			IndexContent(e.SearchContent, IndexAction.Update);
		}

		private void searchableModule_ContentDeleted(object sender, IndexEventArgs e)
		{
			IndexContent(e.SearchContent, IndexAction.Delete);
		}

		private enum IndexAction
		{
			Create,
			Update,
			Delete
		}


        /*Custom*/
        private void ClientFoldersInit(string userFolder, int siteId)
        {
            string CurrentPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(userFolder), siteId.ToString());
            System.IO.DirectoryInfo EditDirectory = new System.IO.DirectoryInfo(CurrentPath);

            // Check if a folder exists and create if it does not
            if (!EditDirectory.Exists)
            {
                EditDirectory.Create();
                EditDirectory.CreateSubdirectory("file");
                EditDirectory.CreateSubdirectory("image");
                EditDirectory.CreateSubdirectory("flash");
                EditDirectory.CreateSubdirectory("media");
                EditDirectory.CreateSubdirectory("downloads");
            }
        }



	}
}

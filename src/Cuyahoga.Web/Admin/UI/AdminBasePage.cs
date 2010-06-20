using System;
using System.Configuration;

using Cuyahoga.Core;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Service.Files;
using Cuyahoga.Web.Components;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Web.Admin.UI
{
	/// <summary>
	/// The base class for the Cuyahoga admin pages. Provides generic functionality.
	/// </summary>
	public class AdminBasePage : GenericBasePage
	{
		private Site _activeSite;
		private Node _activeNode;
		private Section _activeSection;
		private ISiteService _siteService;
        private IUserService _userService;
		private INodeService _nodeService;
		private ISectionService _sectionService;
        private ITemplateService _templateService;
		private ModuleLoader _moduleLoader;

        //Custom Added
        private IFileService _fileService;

		/// <summary>
		/// The Site context of the admin page.
		/// </summary>
		public Site ActiveSite
		{
		    get { return this._activeSite; }
            set { this._activeSite = value; }
		}

	    /// <summary>
		/// The Node context of the admin page.
		/// </summary>
		public Node ActiveNode
		{
			get { return this._activeNode; }
			set { this._activeNode = value; }
		}

		/// <summary>
		/// The Node context of the admin page.
		/// </summary>
		public Section ActiveSection
		{
			get { return this._activeSection; }
		}

        /// <summary>
        /// Site service
        /// </summary>
        public IFileService FileService
        {
            get { return this._fileService; }
        }

		/// <summary>
		/// Site service
		/// </summary>
		public ISiteService SiteService
		{
			get { return this._siteService; }
		}

        /// <summary>
        /// Site service
        /// </summary>
        public IUserService UserService
        {
            get { return this._userService; }
        }

		/// <summary>
		/// Node Service
		/// </summary>
		public INodeService NodeService
		{
			get { return this._nodeService; }
		}

		/// <summary>
		/// Section service
		/// </summary>
		public ISectionService SectionService
		{
			get { return this._sectionService; }
		}

		/// <summary>
		/// Moduleloader
		/// </summary>
		public ModuleLoader ModuleLoader
		{
			get { return this._moduleLoader; }
		}

        /// <summary>
        /// Template service
        /// </summary>
        public ITemplateService TemplateService
        {
            get { return this._templateService; }
        }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AdminBasePage()
		{
			this._activeNode = null;

			this._siteService = Container.Resolve<ISiteService>();
            this._userService = Container.Resolve<IUserService>();
			this._nodeService = Container.Resolve<INodeService>();
			this._sectionService = Container.Resolve<ISectionService>();
			this._moduleLoader = Container.Resolve<ModuleLoader>();
            this._templateService = Container.Resolve<ITemplateService>();
            this._fileService = Container.Resolve<IFileService>();
		}

		protected override void OnInit(EventArgs e)
		{
			// Set template dir, template filename and css.
			base.TemplateDir = ConfigurationManager.AppSettings["TemplateDir"];
			base.TemplateFilename = ConfigurationManager.AppSettings["DefaultTemplate"];
			base.Css = ConfigurationManager.AppSettings["DefaultCss"];

			// Try to set active Site, Node and Section.
			if (Context.Request.QueryString["SectionId"] != null)
			{
				int sectionId = Int32.Parse(Context.Request.QueryString["SectionId"]);
				if (sectionId > 0)
				{
					this._activeSection = this._sectionService.GetSectionById(sectionId);
					if (this._activeSection.Node != null)
					{
						this._activeNode = this._activeSection.Node;
						this._activeSite = this._activeNode.Site;
					}
				}
                else
                {
                    this._activeSection = new Section();
                }
			}
			if (Context.Request.QueryString["NodeId"] != null && this._activeNode == null)
			{
				int nodeId = Int32.Parse(Context.Request.QueryString["NodeId"]);
				if (nodeId > 0)
				{
					this._activeNode = this._nodeService.GetNodeById(nodeId);
					this._activeSite = this._activeNode.Site;
				}
                else
                {
                    this._activeNode = new Node();
                }
			}
			if (Context.Request.QueryString["SiteId"] != null && this._activeSite == null)
			{
				int siteId = Int32.Parse(Context.Request.QueryString["SiteId"]);
                if (siteId > 0)
                {
                    this._activeSite = this._siteService.GetSiteById(siteId);
                }
                else 
                {
                    this._activeSite = new Site();
                }
			}

            //Context stuff
            ICuyahogaContext cuyahogaContext = Container.Resolve<ICuyahogaContext>();
            cuyahogaContext.SelectedSiteId = ActiveSite != null ? ActiveSite.Id : -1;


			// Now, we're here we could check authorization as well.
			// if (! ((User)this.User.Identity).IsInRole("Administrator"))//.HasPermission(AccessLevel.Administrator))
            if (!((User)this.User.Identity).HasRight(Rights.AccessAdmin))//.HasPermission(AccessLevel.Administrator))
			
            {
				throw new ActionForbiddenException("The logged-in user has insufficient rights to access the site administration");
			}

			base.OnInit (e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			string message = Request.QueryString["message"];
			if (message != null)
			{
				ShowMessage(message);
			}
		}
	}
}

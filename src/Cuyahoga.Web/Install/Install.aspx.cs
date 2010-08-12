using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;
using Cuyahoga.Core.DataAccess;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Service.Files;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.Components;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Web.Install
{
	/// <summary>
	/// Summary description for Install.
	/// </summary>
	public class Install : CuyahogaPage
	{
		private ICommonDao _commonDao;
		private ISiteService _siteService;
        private IFileService _fileService;
        private ModuleLoader _moduleLoader;

		protected Panel pnlErrors;
		protected Panel pnlIntro;
		protected Panel pnlAdmin;
		protected Panel pnlModules;
		protected HyperLink hplContinue;
		protected Label lblError;
		protected Button btnInstallDatabase;
		protected TextBox txtPassword;
		protected Button btnAdmin;
		protected RequiredFieldValidator rfvPassword;
		protected Label lblCoreAssembly;
		protected Label lblModulesAssembly;
		protected TextBox txtConfirmPassword;
		protected CompareValidator cpvPassword;
		protected RequiredFieldValidator rfvConfirmPassword;
		protected Label lblMessage;
		protected Panel pnlMessage;
		protected Panel pnlCreateSite;
		protected Button btnCreateSite;
		protected Button btnSkipCreateSite;
		protected Panel pnlFinished;
		protected Repeater rptModules;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Install()
		{
			this._commonDao = IoC.Resolve<ICommonDao>();
			this._siteService = IoC.Resolve<ISiteService>();
            this._moduleLoader = IoC.Resolve<ModuleLoader>();
            this._fileService = IoC.Resolve<IFileService>();
		}
	
		private void Page_Load(object sender, EventArgs e)
		{
			this.pnlErrors.Visible = false;

			if (! this.IsPostBack)
			{
				try
				{
					// Check installable state. Check both Cuyahoga.Core and Cuyahoga.Modules.
					bool canInstall = false;
					// Core
					DatabaseInstaller dbInstaller = new DatabaseInstaller(Server.MapPath("~/Install/Core"), Assembly.Load("Cuyahoga.Core"));
					if (dbInstaller.CanInstall)
					{
						lblCoreAssembly.Text = "Cuyahoga Core " + dbInstaller.NewAssemblyVersion.ToString(3);
						// Core modules
						DatabaseInstaller moduleDbInstaller = new DatabaseInstaller(Server.MapPath("~/Install/Modules"), Assembly.Load("Cuyahoga.Modules"));
						if (moduleDbInstaller.CanInstall)
						{
							canInstall = true;
							lblModulesAssembly.Text = "Cuyahoga Core Modules " + moduleDbInstaller.NewAssemblyVersion.ToString(3);
						}
					}
					if (canInstall)
					{
						this.pnlIntro.Visible = true;
					}
					else
					{
						// Check if we perhaps need to add an admin
						if (this._commonDao.GetAll(typeof(User)).Count == 0)
						{
							this.pnlAdmin.Visible = true;
						}
						else
						{
							ShowError("Can't install Cuyahoga. Check if the install.sql file exists in the /Install/Core|Modules/Database/DATABASE_TYPE/ directory and that there isn't already a version installed.");
						}
					}
				}
				catch (Exception ex)
				{
					ShowError("An error occured: <br/><br/>" + ex.ToString());
				}
			}
		}

		private void BindOptionalModules()
		{
			// Retrieve the modules that are already installed.
			IList availableModules = this._commonDao.GetAll(typeof(ModuleType), "Name");
			// Retrieve the available modules from the filesystem.
			string moduleRootDir = HttpContext.Current.Server.MapPath("~/Modules");
			DirectoryInfo[] moduleDirectories = new DirectoryInfo(moduleRootDir).GetDirectories();
			// Go through the directories and add the modules that can be installed
			ArrayList installableModules = new ArrayList();
			foreach (DirectoryInfo di in moduleDirectories)
			{
				// Skip hidden directories.
				bool shouldAdd = (di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden;
				foreach (ModuleType moduleType in availableModules)
				{
					if (moduleType.Name == di.Name)
					{
						shouldAdd = false;
						break;
					}
				}
				if (shouldAdd)
				{
					// Check if the module can be installed
					DatabaseInstaller moduleInstaller = new DatabaseInstaller(Path.Combine(Server.MapPath("~/Modules/" + di.Name), "Install"), null);
					if (moduleInstaller.CanInstall)
					{
						installableModules.Add(di.Name);
					}
				}
			}
			this.rptModules.DataSource = installableModules;
			this.rptModules.DataBind();
		}

		private void InstallOptionalModules()
		{
			foreach (RepeaterItem ri in this.rptModules.Items)
			{
				CheckBox chkInstall = ri.FindControl("chkInstall") as CheckBox;
				if (chkInstall != null && chkInstall.Checked)
				{
					Literal litModuleName = (Literal) ri.FindControl("litModuleName");
					string moduleName = litModuleName.Text;
					DatabaseInstaller moduleInstaller = new DatabaseInstaller(Path.Combine(Server.MapPath("~/Modules/" + moduleName), "Install"), null);
					moduleInstaller.Install();
				}
			}
		}

		private void ShowError(string message)
		{
			this.lblError.Text = message;
			this.pnlErrors.Visible = true;
			this.pnlMessage.Visible = false;
		}

		private void ShowMessage(string message)
		{
			this.lblMessage.Text = message;
			this.pnlMessage.Visible = true;
			this.pnlErrors.Visible = false;
		}

        private void FinishInstall()
        {
          //upon first install, the registration (on app startup) will have no effect,
          //so make sure this happens after installation
          this._moduleLoader.RegisterActivatedModules();
        }


		#region Site creation code
		    private void CreateSite() 
            {
			    User adminUser = (User) this._commonDao.GetObjectById(typeof(User), 1);
                
                //If SiteData/1/ exists then delete it
                if(Directory.Exists(Server.MapPath("~/SiteData/1/")))
                    this._fileService.DeleteDirectory(Server.MapPath("~/SiteData/1/"));

                #region Sections for Templates 
                    //Template defaultTemplate = this._commonDao.GetObjectByDescription(typeof(Template), "Name", "Another Red") as Template;
                    Template defaultTemplate = this._commonDao.GetObjectByDescription(typeof(Template), "Name", "Cuyahoga") as Template;
                    //Other templates list here
                    Template Corporate = this._commonDao.GetObjectByDescription(typeof(Template), "Name", "Corporate") as Template;
                    Template CityLights = this._commonDao.GetObjectByDescription(typeof(Template), "Name", "CityLights") as Template;
                    Template ImpactDroppy = this._commonDao.GetObjectByDescription(typeof(Template), "Name", "Impact(Droppy)") as Template;
                #endregion

                Role defaultAuthenticatedRole = this._commonDao.GetObjectByDescription(typeof(Role), "Name", "Authenticated user") as Role;

                #region Region for default site
                    // Site
			        Site site = new Site();
			        site.Name = "Cuyahoga Sample Site";
			        site.SiteUrl = UrlHelper.GetSiteUrl();
			        site.WebmasterEmail = "webmaster@localhost.com";
			        site.UseFriendlyUrls = true;
			        site.DefaultCulture = "en-US";
			        site.DefaultTemplate = defaultTemplate;
                    site.DefaultPlaceholder = "Content_Main";
			        site.DefaultRole = defaultAuthenticatedRole;
    			
			        string systemTemplatePath = Server.MapPath(Config.GetConfiguration()["TemplateDir"]);
			        this._siteService.CreateSite(site, Server.MapPath("~/SiteData"), this._commonDao.GetAll<Template>(), systemTemplatePath);

                    //Assign admin user to site
                    adminUser.Sites.Add(site);
                    this._commonDao.SaveObject(adminUser);

                #endregion

                #region Region for Templates
                    //Menu_Sub
                    Section Menu_Sub = new Section();
                    Menu_Sub.Site = site;
                    Menu_Sub.ModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "Menu") as ModuleType;
                    Menu_Sub.Title = "Menu_Sub";
                    Menu_Sub.CacheDuration = 0;

                    Menu_Sub.PlaceholderId = "Navigation_Sub";
                    Menu_Sub.Position = 0;
                    Menu_Sub.ShowTitle = false;

                    Menu_Sub.Settings.Add("LAST_LEVEL", "-1");
                    Menu_Sub.Settings.Add("FIRST_LEVEL", "1");
                    Menu_Sub.Settings.Add("TYPE_RENDER", "NavigationTree");
                    Menu_Sub.Settings.Add("REQUIRES_JQUERY", "False");


                    //Menu_Main
                    Section Menu_Main = new Section();
                    Menu_Main.Site = site;
                    Menu_Main.ModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "Menu") as ModuleType;
                    Menu_Main.Title = "Menu_Main";
                    Menu_Main.CacheDuration = 0;

                    Menu_Main.PlaceholderId = "Navigation_Main";
                    Menu_Main.Position = 0;
                    Menu_Main.ShowTitle = false;

                    Menu_Main.Settings.Add("LAST_LEVEL", "-1");
                    Menu_Main.Settings.Add("FIRST_LEVEL", "0");
                    Menu_Main.Settings.Add("TYPE_RENDER", "NavigationTree");
                    Menu_Main.Settings.Add("REQUIRES_JQUERY", "True");


                    //Section Permissions for all sections
                    IList allRolesSection = this._commonDao.GetAll(typeof(Role));
                    foreach (Role role in allRolesSection)
                    {
                        SectionPermission spMenu_Sub = new SectionPermission();
                        spMenu_Sub.Section = Menu_Sub;
                        spMenu_Sub.Role = role;
                        spMenu_Sub.ViewAllowed = true;
                        spMenu_Sub.EditAllowed = role.HasRight(Rights.EditSections);

                        Menu_Sub.SectionPermissions.Add(spMenu_Sub);

                        SectionPermission spMenu_Main = new SectionPermission();
                        spMenu_Main.Section = Menu_Main;
                        spMenu_Main.Role = role;
                        spMenu_Main.ViewAllowed = true;
                        spMenu_Main.EditAllowed = role.HasRight(Rights.EditSections);

                        Menu_Main.SectionPermissions.Add(spMenu_Main);
                    }

                    //Save the sections
                    this._commonDao.SaveOrUpdateObject(Menu_Sub);
                    this._commonDao.SaveOrUpdateObject(Menu_Main);

                    //Attach Sections to 'defaulttemplate' Sections[PlaceHolderName Key]
                    defaultTemplate.Sections["Navigation_Sub"] = Menu_Sub;
                    defaultTemplate.Sections["Navigation_Main"] = Menu_Main;
                    this._commonDao.SaveOrUpdateObject(defaultTemplate);

                    //Attach Sections to other templates Sections[PlaceHolderName Key]
                    CityLights.Sections["Navigation_Sub"] = Menu_Sub;
                    CityLights.Sections["Navigation_Main"] = Menu_Main;
                    this._commonDao.SaveOrUpdateObject(CityLights);

                    ImpactDroppy.Sections["Navigation_Sub"] = Menu_Sub;
                    ImpactDroppy.Sections["Navigation_Main"] = Menu_Main;
                    this._commonDao.SaveOrUpdateObject(ImpactDroppy);

                    Corporate.Sections["Navigation_Sub"] = Menu_Sub;
                    Corporate.Sections["Navigation_Main"] = Menu_Main;
                    this._commonDao.SaveOrUpdateObject(ImpactDroppy);

                    //Attach these Sections to all the sites templates
                    foreach (Template t in site.Templates) 
                    {
                        t.Sections["Navigation_Sub"] = Menu_Sub;
                        t.Sections["Navigation_Main"] = Menu_Main;
                        this._commonDao.UpdateObject(t);
                    }
                #endregion 


			    // Root node
			    Node rootNode = new Node();
			    rootNode.Culture = site.DefaultCulture;
			    rootNode.Position = 0;
			    rootNode.ShortDescription = "home";
			    rootNode.ShowInNavigation = true;
			    rootNode.Site = site;
                rootNode.Template = site.DefaultTemplate;
			    rootNode.Title = "Home";
			    IList allRoles = this._commonDao.GetAll(typeof(Role));
			    foreach (Role role in allRoles)
			    {
				    NodePermission np = new NodePermission();
				    np.Node = rootNode;
				    np.Role = role;
				    np.ViewAllowed = true;
                    np.EditAllowed = role.HasRight(Rights.EditSections);
				    rootNode.NodePermissions.Add(np);
			    }
			    this._commonDao.SaveOrUpdateObject(rootNode);

			    // Sections on root Node
			    Section loginSection = new Section();
			    loginSection.Site = site;
			    loginSection.ModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "User") as ModuleType;
			    loginSection.Title = "Login";
			    loginSection.CacheDuration = 0;
			    loginSection.Node = rootNode;
                loginSection.PlaceholderId = "Login_Main";
			    loginSection.Position = 0;
			    loginSection.ShowTitle = true;
			    loginSection.Settings.Add("SHOW_EDIT_PROFILE", "True");
			    loginSection.Settings.Add("SHOW_RESET_PASSWORD", "True");
			    loginSection.Settings.Add("SHOW_REGISTER", "True");
			    loginSection.CopyRolesFromNode();
			    rootNode.Sections.Add(loginSection);
			    this._commonDao.SaveOrUpdateObject(loginSection);
			    Section introSection = new Section();
			    introSection.Site = site;
			    introSection.ModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "StaticHtml") as ModuleType;
			    introSection.Title = "Welcome";
			    introSection.CacheDuration = 0;
			    introSection.Node = rootNode;
			    introSection.PlaceholderId = "Content_Main";
			    introSection.Position = 0;
			    introSection.ShowTitle = true;
			    introSection.CopyRolesFromNode();
			    rootNode.Sections.Add(introSection);
			    this._commonDao.SaveOrUpdateObject(introSection);
    			
			    // Pages
			    Node page1 = new Node();
			    page1.Culture = site.DefaultCulture;
			    page1.Position = 0;
			    page1.ShortDescription = "page1";
			    page1.ShowInNavigation = true;
			    page1.Site = site;
                page1.Template = site.DefaultTemplate;
			    page1.Title = "Articles";
			    page1.ParentNode = rootNode;
			    page1.CopyRolesFromParent();
			    this._commonDao.SaveOrUpdateObject(page1);
			    ModuleType articlesModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "Articles") as ModuleType;
			    // Check if the articles module is installed
			    if (articlesModuleType != null)
			    {
				    Section articleSection = new Section();
				    articleSection.Site = site;
				    articleSection.ModuleType = articlesModuleType;
				    articleSection.Title = "Articles";
				    articleSection.CacheDuration = 0;
				    articleSection.Node = page1;
				    articleSection.PlaceholderId = "Content_Main";
				    articleSection.Position = 0;
				    articleSection.ShowTitle = true;
				    articleSection.Settings.Add("DISPLAY_TYPE", "FullContent");
				    articleSection.Settings.Add("ALLOW_ANONYMOUS_COMMENTS", "True");
				    articleSection.Settings.Add("ALLOW_COMMENTS", "True");
				    articleSection.Settings.Add("SORT_BY", "DateOnline");
				    articleSection.Settings.Add("SORT_DIRECTION", "DESC");
				    articleSection.Settings.Add("ALLOW_SYNDICATION", "True");
				    articleSection.Settings.Add("NUMBER_OF_ARTICLES_IN_LIST", "5");
				    articleSection.CopyRolesFromNode();
				    page1.Sections.Add(articleSection);
				    this._commonDao.SaveOrUpdateObject(articleSection);
			    }
			    Node page2 = new Node();
			    page2.Culture = site.DefaultCulture;
			    page2.Position = 1;
			    page2.ShortDescription = "page2";
			    page2.ShowInNavigation = true;
			    page2.Site = site;
                page2.Template = site.DefaultTemplate;
			    page2.Title = "Page 2";
			    page2.ParentNode = rootNode;
			    page2.CopyRolesFromParent();
			    this._commonDao.SaveOrUpdateObject(page2);
			    Section page2Section = new Section();
			    page2Section.Site = site;
			    page2Section.ModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "StaticHtml") as ModuleType;
			    page2Section.Title = "Page 2";
			    page2Section.CacheDuration = 0;
			    page2Section.Node = page2;
                page2Section.PlaceholderId = "Content_Main";
			    page2Section.Position = 0;
			    page2Section.ShowTitle = true;
			    page2Section.CopyRolesFromNode();
			    rootNode.Sections.Add(page2Section);
			    this._commonDao.SaveOrUpdateObject(page2Section);

			    // User Profile node
			    Node userProfileNode = new Node();
			    userProfileNode.Culture = site.DefaultCulture;
			    userProfileNode.Position = 2;
			    userProfileNode.ShortDescription = "userprofile";
			    userProfileNode.ShowInNavigation = false;
			    userProfileNode.Site = site;
                userProfileNode.Template = site.DefaultTemplate;
			    userProfileNode.Title = "User Profile";
			    userProfileNode.ParentNode = rootNode;
			    userProfileNode.CopyRolesFromParent();
			    this._commonDao.SaveOrUpdateObject(userProfileNode);
			    Section userProfileSection = new Section();
			    userProfileSection.Site = site;
			    userProfileSection.ModuleType = this._commonDao.GetObjectByDescription(typeof(ModuleType), "Name", "UserProfile") as ModuleType;
			    userProfileSection.Title = "User Profile";
			    userProfileSection.CacheDuration = 0;
			    userProfileSection.Node = userProfileNode;
                userProfileSection.PlaceholderId = "Content_Main";
			    userProfileSection.Position = 0;
			    userProfileSection.ShowTitle = false;
			    userProfileSection.CopyRolesFromNode();
			    userProfileNode.Sections.Add(userProfileSection);
			    this._commonDao.SaveOrUpdateObject(userProfileSection);

			    // Connections from Login to User Profile
			    loginSection.Connections.Add("Register", userProfileSection);
			    loginSection.Connections.Add("ResetPassword", userProfileSection);
			    loginSection.Connections.Add("ViewProfile", userProfileSection);
			    loginSection.Connections.Add("EditProfile", userProfileSection);
			    this._commonDao.SaveOrUpdateObject(loginSection);
            }

            private void RemoveInstallLock()
		    {
			    HttpContext.Current.Application.Lock();
			    HttpContext.Current.Application["IsInstalling"] = false;
			    HttpContext.Current.Application.UnLock();
		    }

		#endregion


		#region Web Form Designer generated code

		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnInstallDatabase.Click += new System.EventHandler(this.BtnInstallDatabaseClick);
			this.btnAdmin.Click += new System.EventHandler(this.BtnAdminClick);
			this.btnCreateSite.Click += new System.EventHandler(this.BtnCreateSiteClick);
			this.btnSkipCreateSite.Click += new System.EventHandler(this.BtnSkipCreateSiteClick);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		private void BtnInstallDatabaseClick(object sender, EventArgs e)
		{	
			DatabaseInstaller dbInstaller = new DatabaseInstaller(Server.MapPath("~/Install/Core"), Assembly.Load("Cuyahoga.Core"));
			DatabaseInstaller modulesDbInstaller = new DatabaseInstaller(Server.MapPath("~/Install/Modules"), Assembly.Load("Cuyahoga.Modules"));

			try
			{
				dbInstaller.Install();
				modulesDbInstaller.Install();
				this.pnlIntro.Visible = false;
				this.pnlModules.Visible = true;
				BindOptionalModules();
				ShowMessage("Database tables successfully created.");
			}
			catch (Exception ex)
			{
				ShowError("An error occured while installing the database tables: <br/>" + ex.ToString());
			}
		}

		protected void BtnInstallModulesClick(object sender, EventArgs e)
		{
			try
			{
				InstallOptionalModules();
				this.pnlModules.Visible = false;
				this.pnlAdmin.Visible = true;
			}
			catch (Exception ex)
			{
				ShowError("An error occured while installing additional modules: <br/>" + ex.ToString());
			}
		}

		protected void BtnSkipInstallModulesClick(object sender, EventArgs e)
		{
			this.pnlModules.Visible = false;
			this.pnlAdmin.Visible = true;
		}

		private void BtnAdminClick(object sender, EventArgs e)
		{
			if (this.IsValid)
			{
				// Only create an admin if there are really NO users.
				if (this._commonDao.GetAll(typeof(User)).Count > 0)
				{
					ShowError("There is already a user in the database. For security reasons Cuyahoga won't add a new user!");
				}
				else
				{
					User newAdmin = new User();
					newAdmin.UserName = "admin";
					newAdmin.Email = "webmaster@yourdomain.com";
					newAdmin.Password = Core.Domain.User.HashPassword(this.txtPassword.Text);
					newAdmin.IsActive = true;
					newAdmin.TimeZone = 0;

					try
					{
						Role adminRole = (Role)this._commonDao.GetObjectById(typeof(Role), 1);	
						newAdmin.Roles.Add(adminRole);
						this._commonDao.SaveOrUpdateObject(newAdmin);
						this.pnlAdmin.Visible = false;
						this.pnlCreateSite.Visible = true;
					}
					catch (Exception ex)
					{
						ShowError("An error occured while creating the administrator: <br/>" + ex.ToString());
					}					
				}
			}
		}

		private void BtnCreateSiteClick(object sender, EventArgs e)
		{
			try
			{
				CreateSite();
                //at the moment, this is the last step of the installation, so 
                //call the finishing routine here
                this.FinishInstall();
				this.pnlCreateSite.Visible = false;
				this.pnlFinished.Visible = true;
				RemoveInstallLock();
			}
			catch (Exception ex)
			{
				ShowError("An error occured while creating the site: <br/>" + ex.ToString());
			}
		}

		private void BtnSkipCreateSiteClick(object sender, EventArgs e)
		{
            //at the moment, this is the last step of the installation, so 
            //call the finishing routine here
            this.FinishInstall();
			this.pnlCreateSite.Visible = false;
			this.pnlFinished.Visible = true;
			RemoveInstallLock();
		}

		
	}
}

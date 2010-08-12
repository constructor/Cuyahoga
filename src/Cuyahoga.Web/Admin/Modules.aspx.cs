using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service;
using Cuyahoga.Core.Service.Files;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{
    public partial class Modules : AdminBasePage
    {
        private IModuleTypeService _moduleTypeService;
        private IFileService _fileService;

        public Modules() 
        {
            this._moduleTypeService = this.Container.Resolve<IModuleTypeService>();
            this._fileService = this.Container.Resolve<IFileService>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Title = "Modules";
            if (!this.IsPostBack)
            {
                BindModules();
            }
        }

        protected void BindModules()
        {
            // Retrieve the available modules that are installed.
            IList<ModuleType> availableModules = this._moduleTypeService.GetAllModuleTypes();

            // Retrieve the available modules from the filesystem.
            string moduleRootDir = HttpContext.Current.Server.MapPath("~/Modules");
            DirectoryInfo[] moduleDirectories = new DirectoryInfo(moduleRootDir).GetDirectories();

            // Go through the directories and check if there are missing ones. Those have to be added
            // as new ModuleType candidates.
            foreach (DirectoryInfo di in moduleDirectories)
            {
                // Skip hidden directories (and obj folders)
                bool shouldAdd = (di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && 
                                    di.Name != "obj";
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
                    ModuleType newModuleType = new ModuleType();
                    newModuleType.Name = di.Name;
                    availableModules.Add(newModuleType);
                }
            }
            rptModules.DataSource = availableModules;
            rptModules.DataBind();
        }

        protected void rptModules_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ModuleType moduleType = e.Item.DataItem as ModuleType;
                string physicalModuleInstallDirectory = Path.Combine(Server.MapPath("~/Modules/" + moduleType.Name), "Install");
                Assembly moduleAssembly = null;
                if (moduleType.AssemblyName != null)
                {
                    moduleAssembly = Assembly.Load(moduleType.AssemblyName);
                }
                DatabaseInstaller dbInstaller = new DatabaseInstaller(physicalModuleInstallDirectory, moduleAssembly);
                bool canInstall = dbInstaller.CanInstall;
                bool canUpgrade = dbInstaller.CanUpgrade;
                bool canUninstall = dbInstaller.CanUninstall;
                LinkButton lbtInstall = e.Item.FindControl("lbtInstall") as LinkButton;
                lbtInstall.Visible = canInstall;
                lbtInstall.Attributes.Add("onclick", "return confirm('Install this module?')");
                LinkButton lbtUpgrade = e.Item.FindControl("lbtUpgrade") as LinkButton;
                lbtUpgrade.Visible = canUpgrade;
                lbtUpgrade.Attributes.Add("onclick", "return confirm('Upgrade this module?')");
                LinkButton lbtUninstall = e.Item.FindControl("lbtUninstall") as LinkButton;
                lbtUninstall.Visible = canUninstall;
                lbtUninstall.Attributes.Add("onclick", "return confirm('Uninstall this module?')");
                LinkButton lbtDelete = e.Item.FindControl("lbtDelete") as LinkButton;
                lbtDelete.Attributes.Add("onclick", "return confirm('Delete this module folder and contents?')");
                lbtDelete.Visible = canInstall;
                

                CheckBox chkBox = e.Item.FindControl("chkBoxActivation") as CheckBox;
                if (canInstall)
                {
                    chkBox.Enabled = false;
                    chkBox.Checked = moduleType.AutoActivate;
                }
                else
                {
                    chkBox.Enabled = true;
                    chkBox.Checked = moduleType.AutoActivate;
                    if (moduleType.Name != null) chkBox.InputAttributes.Add("moduleTypeId", moduleType.ModuleTypeId.ToString());
                }
                Literal litActivationStatus = e.Item.FindControl("litActivationStatus") as Literal;
                if (this.ModuleLoader.IsModuleActive(moduleType))
                {
                    litActivationStatus.Text = "<span style=\"color:green;\">Active</span>";
                }
                else
                {
                    litActivationStatus.Text = "<span style=\"color:red;\">Not Active</span>";
                }

                Literal litStatus = e.Item.FindControl("litStatus") as Literal;
                if (dbInstaller.CurrentVersionInDatabase != null)
                {
                    litStatus.Text = String.Format("Installed ({0}.{1}.{2})"
                        , dbInstaller.CurrentVersionInDatabase.Major
                        , dbInstaller.CurrentVersionInDatabase.Minor
                        , dbInstaller.CurrentVersionInDatabase.Build);
                    if (dbInstaller.CanUpgrade)
                    {
                        litStatus.Text += " (upgrade available) ";
                    }
                }
                else
                {
                    litStatus.Text = "Uninstalled";
                }
            }
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string[] commandArguments = e.CommandArgument.ToString().Split(':');
            string moduleName = commandArguments[0];
            string assemblyName = commandArguments[1];
            Assembly assembly = null;
            if (assemblyName.Length > 0)
            {
                assembly = Assembly.Load(assemblyName);
            }

            string moduleInstallDirectory = Path.Combine(Server.MapPath("~/Modules/" + moduleName), "Install");
            DatabaseInstaller dbInstaller = new DatabaseInstaller(moduleInstallDirectory, assembly);

            try
            {
                switch (e.CommandName.ToLower())
                {
                    case "install":
                        dbInstaller.Install();
                        break;
                    case "upgrade":
                        dbInstaller.Upgrade();
                        break;
                    case "uninstall":
                        dbInstaller.Uninstall();
                        break;
                    case "delete":
                        string moduleFolder = Server.MapPath("~/Modules/" + moduleName);
                        string dllFile = string.Format("{0}Cuyahoga.Modules.{1}.dll", Server.MapPath("~/bin/"), moduleName);
                        // Maybe add to a service and use a transaction
                        if (Directory.Exists(moduleFolder))
                            _fileService.DeleteDirectory(moduleFolder);
                        if (File.Exists(dllFile)) 
                        _fileService.DeleteFile(dllFile);
                        break;
                }

                // Rebind modules
                BindModules();

                ShowMessage(e.CommandName + ": operation succeeded for " + moduleName + ".");
            }
            catch (Exception ex)
            {
                ShowError(e.CommandName + ": operation failed for " + moduleName + ".<br/>" + ex.Message);
            }
        }

        protected void chkBoxActivation_CheckedChanged(object sender, EventArgs e)
        {
            ModuleType mt = null;
            try
            {
                CheckBox box = (CheckBox)sender;
                if (box.InputAttributes["moduleTypeId"] != null)
                {
                    mt = this._moduleTypeService.GetModuleById(int.Parse(box.InputAttributes["moduleTypeId"]));
                    if (box.Checked)
                    {
                        //set activation status
                        mt.AutoActivate = true;
                        this._moduleTypeService.SaveOrUpdateModuleType(mt);
                        //activate now
                        this.ModuleLoader.ActivateModule(mt);
                    }
                    else
                    {
                        //set activation status
                        mt.AutoActivate = false;
                        this._moduleTypeService.SaveOrUpdateModuleType(mt);
                    }
                }
                this.BindModules();
            }
            catch (Exception ex)
            {
                if (mt != null) ShowError("Loading failed for " + mt.Name + ".<br/>" + ex.Message);
                else ShowError("Loading failed for module.<br/>" + ex.Message);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFile theFile = Context.Request.Files[0];
                string filename = uplUploadModule.FileName;
                if (!filename.EndsWith(".zip"))
                {
                    throw new Exception("Invalid file");
                }

                string filePath = Path.Combine(Server.MapPath("~/Modules"), filename);

                try
                {
                    try
                    {
                        _moduleTypeService.ExtractModulePackage(filePath, theFile.InputStream);
                        litMessages.Text = "Module uploaded and unpacked.";
                    }
                    catch ( Exception ex )
                    {
                        litMessages.Text = "The uploaded file does not seem to be a valid Cuyahoga module pack. The error was: " + ex.Message;
                    }
                    
                }
                catch (Exception ex)
                {
                    litMessages.Text = "Could not upload module pack." + ex.Message;
                }
                this.BindModules();
            }
            else
            {
                litMessages.Text = "No file was uploaded, something must have went wrong!";
            }
        }
    }
}

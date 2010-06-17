using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Communication;
using Cuyahoga.Core.Domain;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

using Cuyahoga.Core.Service.Membership;

namespace Cuyahoga.Web.Admin
{

    public partial class SectionEdit : AdminBasePage
    {

        private Section _activeSection = null;
        private IList _availableModuleTypes;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSection();

            this.Title = "Edit section";

            if (this._activeSection != null && !this.IsPostBack)
            {
                BindSectionControls();
                BindModules();
                BindPlaceholders();
                BindCustomSettings();
                BindConnections();
                BindRoles();
            }

        }

        /// <summary>
        /// Loads an existing Section from the database or creates a new one if the SectionId = -1
        /// </summary>
        protected void LoadSection()
        {
            _activeSection = base.ActiveSection;
            //if (Context.Request.QueryString["SectionId"] != null)
            //{
            //    if (Int32.Parse(Context.Request.QueryString["SectionId"]) == -1)
            //    {
            //        // Create a new section instance
            //        this._activeSection = new Section();
            //        this._activeSection.Node = this.ActiveNode;
            //        this._activeSection.Site = this.ActiveSite;
            //        if (!this.IsPostBack)
            //        {
            //            this._activeSection.CopyRolesFromNode();
            //        }
            //    }
            //    else
            //    {
            //        // Get section data
            //        this._activeSection = (Section)base.CoreRepository.GetObjectById(typeof(Section),
            //            Int32.Parse(Context.Request.QueryString["SectionId"]));
            //    }
            //}
            // Preload available ModuleTypes because we might need them to display the CustomSettings
            // of the first ModuleType if none is selected (when adding a brand new Section).
            this._availableModuleTypes = base.CoreRepository.GetAll(typeof(ModuleType), "Name");
            // Create the controls for the ModuleType-specific settings.
            CreateCustomSettings();
        }

        protected void CreateCustomSettings()
        {
            // Find out the ModuleType. Existing Sections have ModuleType property but for new ones
            // we have to determine which ModuleType is selected.
            ModuleType mt = null;
            if (this._activeSection.ModuleType != null)
            {
                mt = this._activeSection.ModuleType;
            }
            else if (Context.Request.Form[this.ddlModule.UniqueID] != null)
            {
                // The user has selected a ModuleType. Fetch that one from the database and
                // create the settings.
                int moduleTypeId = Int32.Parse(Context.Request.Form[this.ddlModule.UniqueID]);
                mt = (ModuleType)base.CoreRepository.GetObjectById(typeof(ModuleType), moduleTypeId);
            }
            else
            {
                // Get the Settings of the first ModuleType in the list.
                if (this._availableModuleTypes.Count > 0)
                {
                    mt = (ModuleType)this._availableModuleTypes[0];
                }
            }

            if (mt != null)
            {
                foreach (ModuleSetting ms in mt.ModuleSettings)
                {
                    HtmlTableRow settingRow = new HtmlTableRow();
                    HtmlTableCell labelCell = new HtmlTableCell();
                    labelCell.InnerText = ms.FriendlyName;
                    HtmlTableCell controlCell = new HtmlTableCell();
                    controlCell.Controls.Add(SettingControlHelper.CreateSettingControl(ms.Name, ms.GetRealType(), null));
                    settingRow.Cells.Add(labelCell);
                    settingRow.Cells.Add(controlCell);
                    this.plcCustomSettings.Controls.Add(settingRow);
                }
            }
            this.pnlCustomSettings.Visible = mt.ModuleSettings.Count > 0;
        }

        protected void BindSectionControls()
        {
            this.txtTitle.Text = this._activeSection.Title;
            this.txtCSSClass.Text = this._activeSection.CSSClass;
            this.chkShowTitle.Checked = this._activeSection.ShowTitle;
            this.txtCacheDuration.Text = this._activeSection.CacheDuration.ToString();
        }

        protected void BindModules()
        {
            if (this._activeSection.ModuleType != null)
            {
                // A module is attached, there could be data already in it, so we don't give the option to change it
                this.lblModule.Text = this._activeSection.ModuleType.Name;
                this.ddlModule.Visible = false;
                this.lblModule.Visible = true;
            }
            else
            {
                // Note: this._availableModuleTypes are preloaded in LoadSection.
                foreach (ModuleType moduleType in this._availableModuleTypes)
                {
                    this.ddlModule.Items.Add(new ListItem(moduleType.Name, moduleType.ModuleTypeId.ToString()));
                }
                if (this._activeSection.ModuleType != null)
                {
                    this.ddlModule.Items.FindByValue(this._activeSection.ModuleType.ModuleTypeId.ToString()).Selected = true;
                }
                this.ddlModule.Visible = true;
                this.lblModule.Visible = false;
            }
        }

        protected void BindPlaceholders()
        {
            if (this.ActiveNode != null)
            {
                if (this.ActiveNode.Template != null)
                {
                    try
                    {
                        // Read template control and get the containers (placeholders)
                        string templatePath = UrlHelper.GetApplicationPath() + this.ActiveNode.Template.Path;
                        BaseTemplate template = (BaseTemplate)this.LoadControl(templatePath);
                        this.ddlPlaceholder.DataSource = template.Containers;
                        this.ddlPlaceholder.DataValueField = "Key";
                        this.ddlPlaceholder.DataTextField = "Key";
                        this.ddlPlaceholder.DataBind();
                        ListItem li = this.ddlPlaceholder.Items.FindByValue(this._activeSection.PlaceholderId);
                        if (!string.IsNullOrEmpty(this._activeSection.PlaceholderId)
                            && li != null)
                        {
                            li.Selected = true;
                        }
                        // Create url for lookup
                        this.hplLookup.NavigateUrl = "javascript:;";
                        this.hplLookup.Attributes.Add("onclick"
                            , String.Format("window.open(\"TemplatePreview.aspx?TemplateId={0}&Control={1}\", \"Preview\", \"width=760 height=400\")"
                            , this.ActiveNode.Template.Id
                            , this.ddlPlaceholder.ClientID)
                            );
                    }
                    catch (Exception ex)
                    {
                        this.ShowError(ex.Message);
                    }
                }
            }
            else
            {
                this.ddlPlaceholder.Enabled = false;
            }
        }

        protected void BindCustomSettings()
        {
            if (this._activeSection.Settings.Count > 0)
            {
                foreach (ModuleSetting ms in this._activeSection.ModuleType.ModuleSettings)
                {
                    Control ctrl = this.TemplateControl.FindControl(ms.Name);
                    if (this._activeSection.Settings[ms.Name] != null)
                    {
                        string settingValue = this._activeSection.Settings[ms.Name].ToString();
                        if (ctrl is TextBox)
                        {
                            ((TextBox)ctrl).Text = settingValue;
                        }
                        else if (ctrl is CheckBox)
                        {
                            ((CheckBox)ctrl).Checked = Boolean.Parse(settingValue);
                        }
                        else if (ctrl is DropDownList)
                        {
                            DropDownList ddl = (DropDownList)ctrl;
                            ListItem li = ddl.Items.FindByValue(settingValue);
                            if (li != null)
                            {
                                li.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        protected void BindConnections()
        {
            // First test if connections are possible
            if (this._activeSection.ModuleType != null)
            {
                ModuleBase moduleInstance = base.ModuleLoader.GetModuleFromSection(this._activeSection);
                if (moduleInstance is IActionProvider)
                {
                    IActionProvider actionProvider = (IActionProvider)moduleInstance;
                    // OK, show connections panel
                    this.pnlConnections.Visible = true;
                    this.rptConnections.DataSource = this._activeSection.Connections;
                    this.rptConnections.DataBind();
                    if (this._activeSection.Connections.Count < actionProvider.GetOutboundActions().Count)
                    {
                        this.hplNewConnection.Visible = true;
                        if (this.ActiveNode != null)
                        {
                            this.hplNewConnection.NavigateUrl = String.Format("~/Admin/ConnectionEdit.aspx?NodeId={0}&SectionId={1}", this.ActiveNode.Id, this._activeSection.Id);
                        }
                        else
                        {
                            this.hplNewConnection.NavigateUrl = String.Format("~/Admin/ConnectionEdit.aspx?SectionId={0}", this._activeSection.Id);
                        }
                    }
                    else
                    {
                        this.hplNewConnection.Visible = false;
                    }
                }
            }
        }

        protected void BindRoles()
        {
            //IList roles = base.CoreRepository.GetAll(typeof(Role), "PermissionLevel");
            IList roles = base.UserService.GetAllRoles();
            this.rptRoles.ItemDataBound += new RepeaterItemEventHandler(RptRolesItemDataBound);
            this.rptRoles.DataSource = roles;
            this.rptRoles.DataBind();
        }

        protected void SaveSection()
        {
            if (this._activeSection.Id > 0)
            {
                //base.CoreRepository.UpdateObject(this._activeSection);
                this.SectionService.UpdateSection(this._activeSection);
            }
            else
            {
                //base.CoreRepository.SaveObject(this._activeSection);
                this.SectionService.SaveSection(this._activeSection);
            }
        }

        protected void SetCustomSettings()
        {
            foreach (ModuleSetting ms in this._activeSection.ModuleType.ModuleSettings)
            {
                Control ctrl = this.TemplateControl.FindControl(ms.Name);
                object val = null;
                if (ctrl is TextBox)
                {
                    string text = ((TextBox)ctrl).Text;
                    if (ms.IsRequired && text == String.Empty)
                    {
                        throw new Exception(String.Format("The value for {0} is required.", ms.FriendlyName));
                    }
                    val = text;
                }
                else if (ctrl is CheckBox)
                {
                    val = ((CheckBox)ctrl).Checked;
                }
                else if (ctrl is DropDownList)
                {
                    val = ((DropDownList)ctrl).SelectedValue;
                }
                try
                {
                    // Check if the datatype is correct -> brute force casting :)
                    Type type = ms.GetRealType();
                    if (type.IsEnum && val is string)
                    {
                        val = Enum.Parse(type, val.ToString());
                    }
                    else
                    {
                        if (val.ToString().Length > 0)
                        {
                            object testObj = Convert.ChangeType(val, type);
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    throw new Exception(String.Format("Invalid value entered for {0}: {1}", ms.FriendlyName, val.ToString()), ex);
                }
                this._activeSection.Settings[ms.Name] = val.ToString();
            }
        }

        protected void ValidateSettings()
        {
            if (this._activeSection != null)
            {
                ModuleBase moduleInstance = base.ModuleLoader.GetModuleFromSection(this._activeSection);
                moduleInstance.ValidateSectionSettings();
            }
        }

        protected void SetRoles()
        {
            this._activeSection.SectionPermissions.Clear();
            foreach (RepeaterItem ri in rptRoles.Items)
            {
                // HACK: RoleId is stored in the ViewState because the repeater doesn't have a DataKeys property.
                CheckBox chkView = (CheckBox)ri.FindControl("chkViewAllowed");
                CheckBox chkEdit = (CheckBox)ri.FindControl("chkEditAllowed");
                if (chkView.Checked || chkEdit.Checked)
                {
                    SectionPermission sp = new SectionPermission();
                    sp.Section = this._activeSection;
                    sp.Role = (Role)base.CoreRepository.GetObjectById(typeof(Role), (int)ViewState[ri.ClientID]);
                    sp.ViewAllowed = chkView.Checked;
                    sp.EditAllowed = chkEdit.Checked;
                    this._activeSection.SectionPermissions.Add(sp);
                }
            }
        }

        protected void BtnBackClick(object sender, EventArgs e)
        {
            if (this.ActiveNode != null)
            {
                Context.Response.Redirect(String.Format("NodeEdit.aspx?SiteId={0}&NodeId={1}", this.ActiveSite.Id, this.ActiveNode.Id));
            }
            else
            {
                string redirectUrl = String.Format("~/Admin/Sections.aspx?SiteId={0}", this.ActiveSite.Id);
                Context.Response.Redirect(redirectUrl);
            }
        }

        protected void BtnSaveClick(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                // Remember the previous PlaceholderId and Position to detect changes
                string oldPlaceholderId = this._activeSection.PlaceholderId;
                int oldPosition = this._activeSection.Position;

                try
                {
                    this._activeSection.Title = this.txtTitle.Text;
                    this._activeSection.CSSClass = this.txtCSSClass.Text;
                    this._activeSection.ShowTitle = this.chkShowTitle.Checked;
                    this._activeSection.Site = this.ActiveSite;
                    this._activeSection.Node = this.ActiveNode;
                    if (this.ActiveNode != null)
                    {
                        this._activeSection.Node.Sections.Add(this._activeSection);
                    }
                    if (this.ddlModule.Visible)
                    {
                        this._activeSection.ModuleType = (ModuleType)CoreRepository.GetObjectById(
                            typeof(ModuleType), Int32.Parse(this.ddlModule.SelectedValue));
                    }
                    this._activeSection.PlaceholderId = this.ddlPlaceholder.SelectedValue;
                    this._activeSection.CacheDuration = Int32.Parse(this.txtCacheDuration.Text);

                    // Calculate new position if the section is new or when the PlaceholderId has changed
                    if (this._activeSection.Id == -1 || this._activeSection.PlaceholderId != oldPlaceholderId)
                    {
                        this._activeSection.CalculateNewPosition();
                    }

                    // Custom settings
                    SetCustomSettings();

                    // Validate settings
                    ValidateSettings();

                    // Roles
                    SetRoles();

                    // Detect a placeholderId change and change positions of adjacent sections if necessary.					
                    if (oldPosition != -1 && oldPlaceholderId != this._activeSection.PlaceholderId)
                        this._activeSection.ChangeAndUpdatePositionsAfterPlaceholderChange(oldPlaceholderId, oldPosition, true);

                    // Save the active section
                    //SaveSection();
                    this.SectionService.SaveSection(this._activeSection);

                    // Clear cached sections.
                    base.CoreRepository.ClearCollectionCache("Cuyahoga.Core.Domain.Node.Sections");

                    ShowMessage("Section saved.");
                }
                catch (Exception ex)
                {
                    this.ShowError(ex.Message);
                }

                if (!(this._activeSection.Id > 0))
                {
                    if (this.ActiveNode != null)
                    {
                        Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", this.ActiveNode.Id));
                    }
                    else
                    {
                        Context.Response.Redirect("Sections.aspx");
                    }
                }

            }
        }

        protected void RptRolesItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Role role = e.Item.DataItem as Role;
            if (role != null)
            {
                CheckBox chkView = (CheckBox)e.Item.FindControl("chkViewAllowed");
                chkView.Checked = this._activeSection.ViewAllowed(role);
                CheckBox chkEdit = (CheckBox)e.Item.FindControl("chkEditAllowed");
                
                //if (role.HasPermission(AccessLevel.Editor) || role.HasPermission(AccessLevel.Administrator))
                //if (role.Name == "Editor" || role.Name == "Administrator")
                if (role.HasRight(Rights.ManageSections))
                {
                    chkEdit.Checked = this._activeSection.EditAllowed(role);
                }
                else
                {
                    chkEdit.Visible = false;
                }
                // Add RoleId to the ViewState with the ClientID of the repeateritem as key.
                this.ViewState[e.Item.ClientID] = role.Id;
            }
        }

        protected void RptConnectionsItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteConnection")
            {
                string actionName = e.CommandArgument.ToString();

                try
                {
                    this._activeSection.Connections.Remove(actionName);
                    base.CoreRepository.UpdateObject(this._activeSection);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
                BindConnections();
            }
        }
    
    }

}

using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Web.Admin
{
    public partial class SiteEdit : AdminBasePage
    {        
        /// <summary>
        /// Cuyahoga admin site edit page
        /// </summary>
        /// 

        private Site _activeSite;
        private User _currentUser;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SiteEdit()
        {
            this._currentUser = HttpContext.Current.User as User;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            base.Title = "Edit site";

            //if (Context.Request.QueryString["SiteId"] != null)
            //{
                if (base.ActiveSite.Id == -1)
                {
                    // Create a new site instance
                    //this._activeSite = new Site();
                    this.btnDelete.Visible = false;
                    this.hplNewAlias.Visible = false;
                }
                else
                {
                    // Get site data
                    //this._activeSite = base.SiteService.GetSiteById(Int32.Parse(Context.Request.QueryString["SiteId"]));
                    this._activeSite = base.ActiveSite;
                    this.btnDelete.Visible = true;
                    this.btnDelete.Attributes.Add("onclick", "return confirm('Do you wish to permanently delete a site and all its folders?')");
                }
                if (!this.IsPostBack)
                {
                    BindSiteControls();
                    BindTemplates();
                    BindCultures();
                    BindRoles();
                    if (this._activeSite.Id > 0)
                    {
                        BindAliases();
                    }
                }
            //}
        }

        private void BindSiteControls()
        {
            this.txtName.Text = this._activeSite.Name;
            this.txtSiteUrl.Text = this._activeSite.SiteUrl;
            this.txtWebmasterEmail.Text = this._activeSite.WebmasterEmail;
            this.chkUseFriendlyUrls.Checked = this._activeSite.UseFriendlyUrls;

            if (this._activeSite.DefaultTemplate != null) 
                this.ddlTemplates.SelectedValue = this._activeSite.DefaultTemplate.Id.ToString();

            if(this._activeSite.DefaultPlaceholder != null)
                this.ddlPlaceholders.SelectedValue = this._activeSite.DefaultPlaceholder;

            this.txtMetaDescription.Text = this._activeSite.MetaDescription;
            this.txtMetaKeywords.Text = this._activeSite.MetaKeywords;
        }

        private void BindTemplates()
        {
            if (!this.Page.IsPostBack)
            {
                IList templates;

                if (this.ActiveSite != null)
                {
                    int activeSiteID = this.ActiveSite.Id;
                    Cuyahoga.Core.Domain.Site CurrentSite = this.SiteService.GetSiteById(this.ActiveSite.Id);
                    templates = this.TemplateService.GetAllTemplatesBySite(CurrentSite) as IList;
                    //templates = this.TemplateService.GetAllTemplates();
                }
                else
                {
                    templates = this.TemplateService.GetUnassignedTemplates() as IList;
                    //templates = base.CoreRepository.GetAll(typeof(Template), "Name");
                    ddlTemplates.Enabled = true;
                    ddlPlaceholders.Enabled = true;
                }

                // Insert option for no template
                Template emptyTemplate = new Template();
                emptyTemplate.Id = -1;
                emptyTemplate.Name = "No template";
                templates.Insert(0, emptyTemplate);

                // Bind
                this.ddlTemplates.DataSource = templates;
                this.ddlTemplates.DataValueField = "Id";
                this.ddlTemplates.DataTextField = "Name";

                this.ddlTemplates.DataBind();

                if (this._activeSite.DefaultTemplate != null)
                {
                    if (ddlTemplates.Items.FindByValue(this._activeSite.DefaultTemplate.Id.ToString()) != null)
                    {
                        ddlTemplates.Items.FindByValue(this._activeSite.DefaultTemplate.Id.ToString()).Selected = true;
                    }
                    BindPlaceholders();
                }
                this.ddlTemplates.Visible = true;
            }
        }

        private void BindPlaceholders()
        {
            // Try to find the placeholder in the selected template.
            if (this.ddlTemplates.SelectedIndex > 0)
            {
                try
                {
                    Template template = TemplateService.GetTemplateById(Int32.Parse(this.ddlTemplates.SelectedValue));
                    // Read template control and get the containers (placeholders)
                    string templatePath = UrlHelper.GetApplicationPath() + template.Path;
                    BaseTemplate templateControl = (BaseTemplate)this.LoadControl(templatePath);
                    this.ddlPlaceholders.DataSource = templateControl.Containers;
                    this.ddlPlaceholders.DataValueField = "Key";
                    this.ddlPlaceholders.DataTextField = "Key";
                    this.ddlPlaceholders.DataBind();
                    if (this._activeSite.DefaultPlaceholder != null && this._activeSite.DefaultPlaceholder != String.Empty)
                    {
                        this.ddlPlaceholders.Items.FindByValue(this._activeSite.DefaultPlaceholder).Selected = true;
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
            else
            {
                this.ddlPlaceholders.Items.Clear();
            }
        }

        private void BindCultures()
        {
            this.ddlCultures.DataSource = Globalization.GetOrderedCultures();
            this.ddlCultures.DataValueField = "Key";
            this.ddlCultures.DataTextField = "Value";
            this.ddlCultures.DataBind();
            if (this._activeSite.DefaultCulture != null)
            {
                ListItem li = ddlCultures.Items.FindByValue(this._activeSite.DefaultCulture);
                if (li != null)
                {
                    ddlCultures.Items.FindByValue(this._activeSite.DefaultCulture).Selected = true;
                }
            }
        }

        private void BindRoles()
        {
            this.ddlRoles.DataSource = UserService.GetAllRoles();
            this.ddlRoles.DataValueField = "Id";
            this.ddlRoles.DataTextField = "Name";
            this.ddlRoles.DataBind();
            if (this._activeSite.DefaultRole != null)
            {
                ddlRoles.Items.FindByValue(this._activeSite.DefaultRole.Id.ToString()).Selected = true;
            }
        }

        private void BindAliases()
        {
            this.rptAliases.DataSource = base.SiteService.GetSiteAliasesBySite(this._activeSite);
            this.rptAliases.DataBind();
            this.hplNewAlias.NavigateUrl = String.Format("~/Admin/SiteAliasEdit.aspx?SiteId={0}&SiteAliasId=-1", this._activeSite.Id);
        }

        protected void BtnSaveClick(object sender, EventArgs e)
        {
            if (this.ddlTemplates.SelectedValue != "-1")
            {
                if (this.IsValid)
                {
                    this._activeSite.Name = txtName.Text;
                    this._activeSite.SiteUrl = txtSiteUrl.Text;
                    this._activeSite.WebmasterEmail = txtWebmasterEmail.Text;
                    this._activeSite.UseFriendlyUrls = this.chkUseFriendlyUrls.Checked;

                    if (this.ddlTemplates.SelectedValue != "-1")
                    {
                        int templateId = Int32.Parse(this.ddlTemplates.SelectedValue);
                        Template template = TemplateService.GetTemplateById(templateId);
                        this._activeSite.DefaultTemplate = template;
                        if (this.ddlPlaceholders.SelectedIndex > -1)
                        {
                            this._activeSite.DefaultPlaceholder = this.ddlPlaceholders.SelectedValue;
                        }
                    }
                    //else if (this.ddlTemplates.SelectedValue == "-1")
                    //{
                    //    this._activeSite.DefaultTemplate = null;
                    //    this._activeSite.DefaultPlaceholder = null;
                    //}

                    this._activeSite.DefaultCulture = this.ddlCultures.SelectedValue;

                    int defaultRoleId = Int32.Parse(this.ddlRoles.SelectedValue);

                    this._activeSite.DefaultRole = UserService.GetRoleById(defaultRoleId);

                    this._activeSite.MetaDescription = this.txtMetaDescription.Text.Trim().Length > 0
                        ? this.txtMetaDescription.Text.Trim()
                        : null;

                    this._activeSite.MetaKeywords = this.txtMetaKeywords.Text.Trim().Length > 0
                        ? this.txtMetaKeywords.Text.Trim()
                        : null;

                    try
                    {
                        if (this._activeSite.Id > 0)
                        {
                            base.SiteService.SaveSite(this._activeSite);
                            ShowMessage("Site saved.");
                        }
                        else
                        {
                            //Custom Added
                            string systemTemplatePath = Server.MapPath(Config.GetConfiguration()["TemplateDir"]);
                            this.SiteService.CreateSite(this._activeSite, Server.MapPath("~/SiteData"), TemplateService.GetUnassignedTemplates(), systemTemplatePath);
                            Response.Redirect("Default.aspx?message=Site created successfully.");
                            //ShowMessage("Site created.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }
                }
            }
            else 
            {
                ShowError("A template and default placeholder must be selected for the site.");
            }
        }

        protected void BtnCancelClick(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void BtnDeleteClick(object sender, EventArgs e)
        {
            if (!this._currentUser.Sites.Contains(this._activeSite))
            {
                try
                {
                    Cuyahoga.Core.Domain.Site siteToDelete = this._activeSite;
                    this._activeSite = null;

                    //Get the SiteData Folder before the site is deleted
                    string siteDataFolder = System.Web.HttpContext.Current.Server.MapPath(siteToDelete.SiteDataDirectory);

                    //Delete site users
                    base.UserService.DeleteSiteUsers(siteToDelete);

                    //Delete site templates
                    this.TemplateService.DeleteSiteTemplates(siteToDelete);

                    //Delete site 
                    //Site templates folder is deleted via this 
                    //service BEFORE deleting the SiteData folder.
                    base.SiteService.DeleteSite(siteToDelete);

                    //Selete SiteData directory (after Templates folder is deleted)
                    FileService.DeleteDirectory(siteDataFolder);

                    Response.Redirect("Default.aspx");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
            else
            {
                ShowError("You can not delete a site that you are currently assigned to.");
            }
        }

        protected void DdlTemplatesSelectedIndexChanged(object sender, EventArgs e)
        {
            BindPlaceholders();
        }

        protected void RptAliasesItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            SiteAlias sa = (SiteAlias)e.Item.DataItem;
            HyperLink hplEdit = e.Item.FindControl("hplEdit") as HyperLink;
            if (hplEdit != null)
            {
                hplEdit.NavigateUrl = String.Format("~/Admin/SiteAliasEdit.aspx?SiteId={0}&SiteAliasId={1}", this._activeSite.Id, sa.Id);
            }
            Label lblEntryNode = e.Item.FindControl("lblEntryNode") as Label;
            if (lblEntryNode != null)
            {
                if (sa.EntryNode == null)
                {
                    lblEntryNode.Text = "Inherited from site";
                }
                else
                {
                    lblEntryNode.Text = sa.EntryNode.Title + " (" + sa.EntryNode.Culture + ")";
                }
            }
        }

    }
}

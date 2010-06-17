using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Web.Admin
{
    public partial class TemplateEdit : AdminBasePage
    {
        private Template _activeTemplate;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Edit template";

            if (Context.Request.QueryString["TemplateId"] != null)
            {
                int templateid = Int32.Parse(Context.Request.QueryString["TemplateId"]);
                if (templateid == -1)
                {
                    this._activeTemplate = new Template();
                }
                else
                {
                    this._activeTemplate = this.TemplateService.GetTemplateById(templateid);
                    this.pnlPlaceholders.Visible = true;
                }

                if (!Page.IsPostBack)
                {
                    BindTemplateControls();
                    BindTemplateUserControls();
                    BindCss();

                    if (this._activeTemplate.Id != -1)
                    {
                        BindPlaceholders();
                    }

                    //Custom Added
                    BindSites();
                }

                //Custom Added
                //TO DO: Add messages to localised resources
                btnCopyToSite.Visible = (this.ActiveSite != null && this._activeTemplate.Id > 0);
                ddlSites.Enabled = (this._activeTemplate.Id > 0);
                if (this.ActiveSite != null)
                {
                    litMessages.Text = "Uploads to: " + this.ActiveSite.Name + "'s tempates folder.";
                }
                else
                {
                    litMessages.Text = "Uploads to shared templates folder. Select a specific site to upload to its folder.";
                }
            }

        }

        protected void BindSites()
        {
            ddlSites.DataSource = this.SiteService.GetAllSites();
            ddlSites.DataBind();

            if (this.ActiveSite != null)
            {
                ddlSites.SelectedValue = this.ActiveSite.Id.ToString();
            }
        }

        protected void BindTemplateControls()
        {
            this.txtName.Text = this._activeTemplate.Name;
            this.txtBasePath.Text = this._activeTemplate.BasePath;
            this.btnDelete.Visible = (this._activeTemplate.Id > 0 && base.CoreRepository.GetNodesByTemplate(this._activeTemplate).Count <= 0);
            this.btnDelete.Attributes.Add("onclick", "return confirm('Are you sure?')");
        }

        protected void BindTemplateUserControls()
        {
            this.ddlTemplateControls.Items.Clear();

            string physicalTemplateDir = Context.Server.MapPath(
                UrlHelper.GetApplicationPath() + this._activeTemplate.BasePath);
            DirectoryInfo dir = new DirectoryInfo(physicalTemplateDir);
            if (dir.Exists)
            {
                FileInfo[] templateControls = dir.GetFiles("*.ascx");
                if (templateControls.Length == 0 && this.IsPostBack)
                {
                    this.lblTemplateControlWarning.Visible = true;
                    this.lblTemplateControlWarning.Text = "No template user controls found at the [base path] location.";
                }
                else
                {
                    foreach (FileInfo templateControlFile in templateControls)
                    {
                        this.ddlTemplateControls.Items.Add(templateControlFile.Name);
                    }
                    ListItem li = this.ddlTemplateControls.Items.FindByValue(this._activeTemplate.TemplateControl);
                    if (li != null)
                    {
                        li.Selected = true;
                    }
                }
            }
        }

        protected void BindCss()
        {
            this.ddlCss.Items.Clear();

            string physicalCssDir = Context.Server.MapPath(
                UrlHelper.GetApplicationPath() + this._activeTemplate.BasePath + "/Css");
            DirectoryInfo dir = new DirectoryInfo(physicalCssDir);
            if (dir.Exists)
            {
                FileInfo[] cssSheets = dir.GetFiles("*.css");
                if (cssSheets.Length == 0 && this.IsPostBack)
                {
                    this.lblCssWarning.Visible = true;
                    this.lblCssWarning.Text = "No stylesheet files found at the [base path]/Css location.";
                }
                else
                {
                    foreach (FileInfo css in cssSheets)
                    {
                        this.ddlCss.Items.Add(css.Name);
                    }
                    ListItem li = this.ddlCss.Items.FindByValue(this._activeTemplate.Css);
                    if (li != null)
                    {
                        li.Selected = true;
                    }
                }
            }
            else
            {
                if (Page.IsPostBack)
                {
                    this.lblCssWarning.Visible = true;
                    this.lblCssWarning.Text = "The location for the stylesheets ([base path]/Css) could not be found.";
                }
            }
        }

        protected void BindPlaceholders()
        {
            // Load template control first.
            string templateControlPath = UrlHelper.GetApplicationPath() + this._activeTemplate.Path;
            if (File.Exists(Server.MapPath(templateControlPath)))
            {
                BaseTemplate templateControl = (BaseTemplate)this.Page.LoadControl(templateControlPath);
                this.rptPlaceholders.DataSource = templateControl.Containers;
                this.rptPlaceholders.DataBind();
            }
            else
            {
                ShowError("Unable to load the template control " + templateControlPath);
            }
        }

        protected void CheckBasePath()
        {
            if (this._activeTemplate.BasePath.Trim() == String.Empty)
            {
                ShowError("The base path can not be empty.");
            }
            else
            {
                string physicalBasePath = Context.Server.MapPath(
                    UrlHelper.GetApplicationPath() + this._activeTemplate.BasePath);
                if (!Directory.Exists(physicalBasePath))
                {
                    ShowError("The base path you entered could not be found on the server.");
                }
                else
                {
                    BindTemplateUserControls();
                    BindCss();
                }
            }
        }

        protected void SaveTemplate()
        {
            try
            {
                if (this._activeTemplate.Id == -1)
                {
                    base.CoreRepository.SaveObject(this._activeTemplate);
                    Context.Response.Redirect("Templates.aspx");
                }
                else
                {
                    base.CoreRepository.UpdateObject(this._activeTemplate);
                    ShowMessage("Template saved");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void BtnSaveClick(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                if (this.ddlTemplateControls.SelectedIndex == -1 || this.ddlCss.SelectedIndex == -1)
                {
                    ShowError("No template control or css selected.");
                }
                else
                {
                    //Custom 
                    if (ddlSites.SelectedValue != "None")
                    {
                        int siteID = int.Parse(ddlSites.SelectedValue);
                        Cuyahoga.Core.Domain.Site CurrentSite = this.SiteService.GetSiteById(siteID);
                        this._activeTemplate.Site = CurrentSite;
                    }
                    else
                    {
                        this._activeTemplate.Site = null;
                    }

                    this._activeTemplate.Name = this.txtName.Text;
                    this._activeTemplate.BasePath = this.txtBasePath.Text;
                    this._activeTemplate.TemplateControl = this.ddlTemplateControls.SelectedValue;
                    this._activeTemplate.Css = this.ddlCss.SelectedValue;
                    SaveTemplate();
                }
            }
        }

        protected void BtnDeleteClick(object sender, EventArgs e)
        {
            if (this._activeTemplate.Id > 0)
            {
                try
                {
                    //Custom Added: Selete Tempate Folder
                    Directory.Delete(Server.MapPath("~/" + this._activeTemplate.BasePath), true);

                    base.CoreRepository.DeleteObject(this._activeTemplate);
                    Context.Response.Redirect("Templates.aspx");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        protected void BtnCancelClick(object sender, EventArgs e)
        {
            Context.Response.Redirect("Templates.aspx");
        }

        protected void BtnVerifyBasePathClick(object sender, EventArgs e)
        {
            this._activeTemplate.BasePath = this.txtBasePath.Text;
            CheckBasePath();
        }

        protected void RptPlaceholdersItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DictionaryEntry entry = (DictionaryEntry)e.Item.DataItem;

                string placeholder = entry.Key.ToString();
                Label lblPlaceholder = e.Item.FindControl("lblPlaceholder") as Label;
                HyperLink hplSection = e.Item.FindControl("hplSection") as HyperLink;
                HyperLink hplAttachSection = e.Item.FindControl("hplAttachSection") as HyperLink;
                LinkButton lbtDetachSection = e.Item.FindControl("lbtDetachSection") as LinkButton;
                lblPlaceholder.Text = placeholder;

                // Find an attached section
                Section section = null;
                if (this._activeTemplate.Sections.ContainsKey(placeholder))
                {
                    section = this._activeTemplate.Sections[placeholder] as Section;
                }
 
                if (section != null)
                {
                    hplSection.Text = section.Title;
                    hplSection.NavigateUrl = "~/Admin/SectionEdit.aspx?SectionId=" + section.Id;
                    hplSection.Visible = true;
                    hplAttachSection.Visible = false;
                    lbtDetachSection.Visible = true;
                    lbtDetachSection.Attributes.Add("onclick", "return confirm(\"Are you sure?\");");
                    lbtDetachSection.CommandArgument = placeholder;
                }
                else
                {
                    hplSection.Visible = false;
                    hplAttachSection.Visible = true;
                    hplAttachSection.NavigateUrl = String.Format("~/Admin/TemplateSection.aspx?TemplateId={0}&Placeholder={1}"
                        , this._activeTemplate.Id, placeholder);
                    lbtDetachSection.Visible = false;
                }
            }
        }

        protected void RptPlaceholdersItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "detach")
            {
                string placeholder = e.CommandArgument.ToString();
                this._activeTemplate.Sections.Remove(placeholder);

                try
                {
                    base.CoreRepository.UpdateObject(this._activeTemplate);
                    ShowMessage(String.Format("Section in Placeholder {0} detached", placeholder));
                    BindPlaceholders();
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        protected void CopyTemplateToSite(object sender, EventArgs e)
        {
            if (ActiveSite != null)
            {
                string siteDataRoot = Server.MapPath("~/SiteData/" + ActiveSite.Id.ToString()) + "\\";
                string templateDirectoryName = this._activeTemplate.BasePath.Substring(this._activeTemplate.BasePath.IndexOf("Templates/") + 10);
                string source = Server.MapPath("~/" + this._activeTemplate.BasePath);
                string destination = siteDataRoot + "templates\\" + templateDirectoryName;

                if (!Directory.Exists(destination))
                {
                    if (this.FileService.CheckIfDirectoryIsWritable(siteDataRoot))
                    {
                        Template newTemplate = this._activeTemplate.GetCopy();
                        newTemplate.Site = this.ActiveSite;
                        newTemplate.BasePath = "SiteData/" + ActiveSite.Id.ToString() + "/templates/" + templateDirectoryName;

                        //Also copy the sections that are assigned to this template
                        try
                        {
                            foreach (KeyValuePair<string, Section> entry in this._activeTemplate.Sections)
                            {
                                newTemplate.Sections.Add(entry.Key, entry.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            //log.Error("An unexpected error occured while creating a new site.", ex);
                            //throw;
                        }

                        this.TemplateService.SaveTemplate(newTemplate);

                        //Copy the template to SiteData folder
                        this.FileService.CopyDirectoryContents(source, destination);

                        if (Request.QueryString["SiteId"] != null)
                        {
                            //Custom Added
                            string redirecturl = String.Format("~/Admin/TemplateEdit.aspx?SiteId={0}&TemplateId={1}", Request.QueryString["SiteId"].ToString(), newTemplate.Id.ToString());
                            Response.Redirect(redirecturl);
                        }
                        else
                        {
                            //Custom Added
                            string redirecturl = String.Format("~/Admin/TemplateEdit.aspx?TemplateId={0}", newTemplate.Id.ToString());
                            Response.Redirect(redirecturl);
                        }
                    }
                    else
                    {
                        ShowError("'" + siteDataRoot + "'" + " is not writable.");
                    }
                }
                else 
                {
                    ShowError("This template folder already exists in that destination.");
                }
            }
        }

        protected void DdlSitesSelectedIndexChanged(object sender, EventArgs e)
        {
            //Custom Added
            if (ddlSites.SelectedValue != "None")
            {
                string navigateUrl = String.Format("~/Admin/TemplateEdit.aspx?SiteId={0}&TemplateId={1}", ddlSites.SelectedValue, this._activeTemplate.Id.ToString());
                Response.Redirect(navigateUrl);
            }
            else
            {
                string navigateUrl = String.Format("~/Admin/TemplateEdit.aspx?TemplateId={0}", this._activeTemplate.Id.ToString());
                Response.Redirect(navigateUrl);
            }
        }

        protected void BtnUploadClick(object sender, EventArgs e)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFile theFile = Context.Request.Files[0];
                string filename = uplUploadTemplate.FileName;
                if (!filename.EndsWith(".zip"))
                {
                    throw new Exception("Invalid file");
                }

                string templatesRoot;
                string tName;
                string tBasePath;

                if (this.ActiveSite != null)
                {
                    //Site drop down activation
                    ddlSites.Enabled = (this.ActiveSite.Id > 0);
                    ddlSites.SelectedValue = this.ActiveSite.Id.ToString();

                    //templatesRoot = "~/SiteData/" + this.ActiveSite.Id.ToString() + "/Templates/";
                    templatesRoot = VirtualPathUtility.Combine(this.ActiveSite.SiteDataDirectory, "templates");
                    tName = filename.Substring(0, filename.Length - 4);
                    tBasePath = "SiteData/" + this.ActiveSite.Id.ToString() + "/templates/" + filename.Substring(0, filename.Length - 4);
                }
                else
                {
                    templatesRoot = "~/templates/";
                    tName = filename.Substring(0, filename.Length - 4);
                    tBasePath = "templates/" + filename.Substring(0, filename.Length - 4);
                }

                string filePath = Path.Combine(Server.MapPath(templatesRoot), filename);

                try
                {
                    this.TemplateService.ExtractTemplatePackage(filePath, theFile.InputStream);
                    litMessages.Text = "Template uploaded and unpacked.";

                    //Set templates inputs
                    txtName.Text = tName;
                    txtBasePath.Text = tBasePath;
                }
                catch (Exception ex)
                {
                    litMessages.Text = "Could not upload template pack. " + ex.Message;
                }

            }
            else
            {
                litMessages.Text = "No file was uploaded, something must have went wrong!";
            }
        }

    }
}

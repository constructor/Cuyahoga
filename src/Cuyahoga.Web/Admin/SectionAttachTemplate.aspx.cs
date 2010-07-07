using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Web.UI;


namespace Cuyahoga.Web.Admin
{

    public partial class SectionAttachTemplate : AdminBasePage
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            base.Title = "Attach section to template(s)";
        }

        protected void BindSectionControls()
        {
            if (base.ActiveSection != null)
            {
                this.lblSection.Text = base.ActiveSection.Title;
                this.lblModuleType.Text = base.ActiveSection.ModuleType.Name;
            }
        }

        protected void BindTemplates()
        {
            this.rptTemplates.DataSource = TemplateService.GetAllTemplates();
            this.rptTemplates.DataBind();
        }

        protected void RptTemplatesItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Template template = e.Item.DataItem as Template;
            if (template != null)
            {
                DropDownList ddlPlaceHolders = e.Item.FindControl("ddlPlaceHolders") as DropDownList;
                HyperLink hplLookup = e.Item.FindControl("hplLookup") as HyperLink;

                // Read template control and get the containers (placeholders)
                string templatePath = Util.UrlHelper.GetApplicationPath() + template.Path;
                BaseTemplate templateControl = (BaseTemplate)this.LoadControl(templatePath);
                foreach (DictionaryEntry entry in templateControl.Containers)
                {
                    // Check if the placeholder isn't taken by another section
                    bool isTaken = false;
                    Section section = template.Sections[entry.Key.ToString()] as Section;
                    if (section != null)
                    {
                        if (section == base.ActiveSection)
                        {
                            // it's already connected to the current section -> check checkbox
                            CheckBox chkAttached = e.Item.FindControl("chkAttached") as CheckBox;
                            chkAttached.Checked = true;
                        }
                        else
                        {
                            isTaken = true;
                        }
                    }
                    if (!isTaken)
                    {
                        string placeHolderId = entry.Key.ToString();
                        ddlPlaceHolders.Items.Add(placeHolderId);
                        if (section == base.ActiveSection)
                        {
                            ddlPlaceHolders.SelectedValue = placeHolderId;
                        }
                    }
                }
                // Set lookup link
                hplLookup.NavigateUrl = "javascript:;";
                hplLookup.Attributes.Add("onclick"
                    , String.Format("window.open(\"TemplatePreview.aspx?TemplateId={0}&Control={1}\", \"Preview\", \"width=760 height=400\")"
                    , template.Id
                    , ddlPlaceHolders.ClientID)
                );

                // Add TemplateId to the ViewState with the ClientID of the repeateritem as key.
                this.ViewState[e.Item.ClientID] = template.Id;
            }
        }

        protected void BtnBackClick(object sender, System.EventArgs e)
        {
            Context.Response.Redirect("~/Admin/Sections.aspx");
        }

        protected void BtnSaveClick(object sender, System.EventArgs e)
        {
            try
            {
                foreach (RepeaterItem ri in this.rptTemplates.Items)
                {
                    Template template = TemplateService.GetTemplateById((int)this.ViewState[ri.ClientID]);

                    CheckBox chkAttached = ri.FindControl("chkAttached") as CheckBox;
                    DropDownList ddlPlaceHolders = ri.FindControl("ddlPlaceHolders") as DropDownList;
                    string selectedPlaceholderId = ddlPlaceHolders.SelectedValue;
                    // Find to find if the current section is already attached to the template
                    string attachedPlaceholderId = null;
                    foreach (KeyValuePair<string, Section> entry in template.Sections)
                    {
                        if (entry.Value.Equals(base.ActiveSection))
                        {
                            attachedPlaceholderId = entry.Key;
                            break;
                        }
                    }

                    // Attach
                    if (chkAttached.Checked)
                    {
                        if (attachedPlaceholderId != null && attachedPlaceholderId != selectedPlaceholderId)
                        {
                            template.Sections.Remove(attachedPlaceholderId);
                        }
                        template.Sections[selectedPlaceholderId] = base.ActiveSection;
                    }
                    else
                    {
                        // Remove a possible attached section
                        if (attachedPlaceholderId != null)
                        {
                            template.Sections.Remove(attachedPlaceholderId);
                        }
                    }
                    TemplateService.SaveTemplate(template);
                }
                Context.Response.Redirect("~/Admin/Sections.aspx");
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

    }

}

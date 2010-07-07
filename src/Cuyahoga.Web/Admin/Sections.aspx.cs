using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;
using log4net;

namespace Cuyahoga.Web.Admin
{
    public partial class Sections : AdminBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Sections));

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Sections";
            if (!this.IsPostBack)
            {
                BindSections();
            }
        }

        protected void BindSections()
        {
            this.rptSections.DataSource = SectionService.GetUnconnectedSections();
            this.rptSections.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            string redirectUrl = String.Format("~/Admin/SectionEdit.aspx?SectionId=-1&SiteId={0}", this.ActiveSite.Id);
            Context.Response.Redirect(redirectUrl);
        }

        protected void rptSections_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Section section = e.Item.DataItem as Section;
            if (section != null)
            {
                IList templates = SectionService.GetTemplatesBySection(section).ToList();

                HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");
                hplEdit.NavigateUrl = (this.ActiveSite != null) ? String.Format("~/Admin/SectionEdit.aspx?SiteId={0}&SectionId={1}", this.ActiveSite.Id.ToString(), section.Id) : String.Format("~/Admin/SectionEdit.aspx?SectionId={0}", section.Id);

                LinkButton lbtDelete = (LinkButton)e.Item.FindControl("lbtDelete");
                HyperLink hplAttachTemplate = (HyperLink)e.Item.FindControl("hplAttachTemplate");
                hplAttachTemplate.NavigateUrl = (this.ActiveSite != null) ? String.Format("~/Admin/SectionAttachTemplate.aspx?SiteId={0}&SectionId={1}", this.ActiveSite.Id.ToString(), section.Id) : String.Format("~/Admin/SectionAttachTemplate.aspx?SectionId={0}", section.Id);
                
                HyperLink hplAttachNode = (HyperLink)e.Item.FindControl("hplAttachNode");
                if (templates.Count > 0)
                {
                    Literal litTemplates = (Literal)e.Item.FindControl("litTemplates");
                    for (int i = 0; i < templates.Count; i++)
                    {
                        litTemplates.Text += ((Template)templates[i]).Name;
                        if (i < templates.Count - 1)
                        {
                            litTemplates.Text += ", ";
                        }
                    }
                    hplAttachNode.Visible = false;
                    lbtDelete.Visible = false;
                }
                else
                {
                    hplAttachNode.NavigateUrl = (this.ActiveSite != null) ? String.Format("~/Admin/SectionAttachNode.aspx?SiteId{0}&SectionId={1}", this.ActiveSite.Id.ToString() , section.Id) : String.Format("~/Admin/SectionAttachNode.aspx?SectionId={0}", section.Id);
                }
                lbtDelete.Attributes.Add("onclick", "return confirm('Are you sure?')");
            }
        }

        protected void rptSections_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int sectionId = Int32.Parse(e.CommandArgument.ToString());
            Section section = SectionService.GetSectionById(sectionId);

            if (e.CommandName == "Delete")
            {
                try
                {
                    // First tell the module to remove its content.
                    ModuleBase module = this.ModuleLoader.GetModuleFromSection(section);
                    module.DeleteModuleContent();

                    // Remove from all template sections
                    IList templates = SectionService.GetTemplatesBySection(section).ToList();

                    foreach (Template template in templates)
                    {
                        string attachedPlaceholderId = null;
                        foreach (KeyValuePair<string, Section> entry in template.Sections)
                        {
                            if (entry.Value.Equals(section))
                            {
                                attachedPlaceholderId = entry.Key.ToString();
                                break;
                            }
                        }
                        template.Sections.Remove(attachedPlaceholderId);
                        TemplateService.SaveTemplate(template);
                    }

                    // Now delete the Section.
                    SectionService.DeleteSection(section, module);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    log.Error(String.Format("Error deleting section : {0}.", section.Id.ToString()), ex);
                }
            }
            BindSections();
        }
    }
}

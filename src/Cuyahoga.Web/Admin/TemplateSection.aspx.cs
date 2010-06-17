using System;
using System.Collections;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{
    public partial class TemplateSection : AdminBasePage
    {
        private Template _activeTemplate;
        private string _activePlaceholder;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Title = "Attach Section to Template Placeholder";
            if (Context.Request.QueryString["TemplateId"] != null
                && Context.Request.QueryString["Placeholder"] != null)
            {
                this._activeTemplate = (Template)base.CoreRepository.GetObjectById(typeof(Template)
                    , Int32.Parse(Context.Request.QueryString["TemplateId"]));
                this._activePlaceholder = Context.Request.QueryString["Placeholder"];

                if (!this.IsPostBack)
                {
                    BindTemplateControls();
                    BindSections();
                }
            }
            else
            {
                ShowError("Passed invalid template or placeholder");
            }
        }

        protected void BindTemplateControls()
        {
            this.lblTemplate.Text = this._activeTemplate.Name;
            this.lblPlaceholder.Text = this._activePlaceholder;
        }

        protected void BindSections()
        {
            IList unconnectedSections = base.CoreRepository.GetUnconnectedSections();
            if (unconnectedSections.Count > 0)
            {
                this.ddlSections.DataSource = unconnectedSections;
                this.ddlSections.DataValueField = "Id";
                this.ddlSections.DataTextField = "Title";
                this.ddlSections.DataBind();
            }
            else
            {
                this.btnAttach.Enabled = false;
            }
        }

        protected void BtnAttachClick(object sender, System.EventArgs e)
        {
            int selectedSectionId = Int32.Parse(this.ddlSections.SelectedValue);
            Section section = (Section)base.CoreRepository.GetObjectById(typeof(Section), selectedSectionId);
            this._activeTemplate.Sections[this._activePlaceholder] = section;
            try
            {
                base.CoreRepository.UpdateObject(this._activeTemplate);
                Context.Response.Redirect("~/Admin/TemplateEdit.aspx?TemplateId=" + this._activeTemplate.Id);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void BtnBackClick(object sender, System.EventArgs e)
        {
            Context.Response.Redirect("~/Admin/TemplateEdit.aspx?TemplateId=" + this._activeTemplate.Id);
        }
    }
}

﻿using System;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;

namespace Cuyahoga.Web.Admin
{
    public partial class Templates : Cuyahoga.Web.Admin.UI.AdminBasePage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindTemplates();
                BindSites();
            }

            this.Title = this.ActiveSite != null ? string.Format("Templates of site '{0}'",this.ActiveSite.Name) : "Templates";
        }

        protected void UpdateTemplates()
        {
            //Get only templates for dropdown selected site (Custom) Replaces above
            Int32 selectedsiteid = Convert.ToInt32(ddlSites.SelectedValue);

            if (selectedsiteid < 0)
            {
                Cuyahoga.Core.Domain.Site selectedsite = SiteService.GetSiteById(selectedsiteid);
                this.rptTemplates.DataSource = this.TemplateService.GetUnassignedTemplates();
                this.rptTemplates.DataBind();
            }
            else
            {
                if (selectedsiteid < 1)
                {
                    this.rptTemplates.DataSource = this.TemplateService.GetAllTemplates();
                    this.rptTemplates.DataBind();
                }
                else
                {
                    Cuyahoga.Core.Domain.Site selectedsite = SiteService.GetSiteById(selectedsiteid);
                    this.rptTemplates.DataSource = this.TemplateService.GetAllTemplatesBySite(selectedsite);
                    this.rptTemplates.DataBind();
                }
            }
        }

        protected void BindTemplates()
        {
            //Get only templates for dropdown selected site (Custom) Replaces above
            Int32 selectedsiteid = Convert.ToInt32(ddlSites.SelectedValue);

            if (this.ActiveSite != null)
            {
                Cuyahoga.Core.Domain.Site selectedsite = SiteService.GetSiteById(this.ActiveSite.Id);
                this.rptTemplates.DataSource = this.TemplateService.GetAllTemplatesBySite(selectedsite);
                this.rptTemplates.DataBind();
            }
            else
            {
                this.rptTemplates.DataSource = this.TemplateService.GetAllTemplates();
                this.rptTemplates.DataBind();
            }

        }

        protected void BindSites()
        {
            ddlSites.DataSource = this.SiteService.GetAllSites();
            ddlSites.DataTextField = "Name";
            ddlSites.DataValueField = "Id";
            ddlSites.DataBind();

            if (this.ActiveSite != null)
            {
                ddlSites.SelectedValue = this.ActiveSite.Id.ToString();
            }
        }

        protected void RptTemplatesItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            Template template = e.Item.DataItem as Template;
            if (template != null)
            {
                HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");
                hplEdit.NavigateUrl = String.Format("~/Admin/TemplateEdit.aspx?TemplateId={0}", template.Id);
            }
        }

        protected void BtnNewClick(object sender, System.EventArgs e)
        {
            Context.Response.Redirect("TemplateEdit.aspx?TemplateId=-1");
        }

        protected void DdlSitesSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTemplates();
        }
    }
}

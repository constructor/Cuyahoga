using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{
    public partial class SectionAttachNode : AdminBasePage
    {
        private Section _activeSection;
        private Site _selectedSite;
        private Node _selectedNode;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Title = "Attach section";
            if (Context.Request.QueryString["SectionId"] != null)
            {
                // Get section data
                this._activeSection = SectionService.GetSectionById(Int32.Parse(Context.Request.QueryString["SectionId"]));
            }
            if (!this.IsPostBack)
            {
                BindSectionControls();
                BindSites();
            }
            else
            {
                if (this.ddlSites.SelectedIndex > -1)
                {
                    this._selectedSite = SiteService.GetSiteById(Int32.Parse(this.ddlSites.SelectedValue));
                }
                if (this.lbxAvailableNodes.SelectedIndex > -1)
                {
                    this._selectedNode =  NodeService.GetNodeById(Int32.Parse(this.lbxAvailableNodes.SelectedValue));
                }
            }
        }

        protected void BindSectionControls()
        {
            if (this._activeSection != null)
            {
                this.lblSection.Text = this._activeSection.Title;
                this.lblModuleType.Text = this._activeSection.ModuleType.Name;
            }
        }

        protected void BindSites()
        {
            IList sites = SiteService.GetAllSites();

            foreach (Site site in sites)
            {
                if (this._selectedSite == null)
                {
                    this._selectedSite = site;
                }
                ListItem li = new ListItem(site.Name + " (" + site.SiteUrl + ")", site.Id.ToString());
                this.ddlSites.Items.Add(li);
            }
            BindNodes();
        }

        protected void BindNodes()
        {
            if (this._selectedSite != null)
            {
                this.lbxAvailableNodes.Visible = true;
                this.lbxAvailableNodes.Items.Clear();
                IList<Node> rootNodes = this._selectedSite.RootNodes;
                AddAvailableNodes(rootNodes);
            }
            this.btnSave.Enabled = false;
            this.ddlPlaceholder.Visible = false;
            this.hplLookup.Visible = false;
        }

        protected void BindPlaceholders()
        {
            if (this._selectedNode != null)
            {
                if (this._selectedNode.Template != null)
                {
                    try
                    {
                        this.ddlPlaceholder.Visible = true;
                        // Read template control and get the containers (placeholders)
                        string templatePath = Util.UrlHelper.GetApplicationPath() + this._selectedNode.Template.Path;
                        BaseTemplate template = (BaseTemplate)this.LoadControl(templatePath);
                        this.ddlPlaceholder.DataSource = template.Containers;
                        this.ddlPlaceholder.DataValueField = "Key";
                        this.ddlPlaceholder.DataTextField = "Key";
                        this.ddlPlaceholder.DataBind();
                        // Create url for lookup
                        this.hplLookup.Visible = true;
                        this.hplLookup.NavigateUrl = "javascript:;";
                        this.hplLookup.Attributes.Add("onclick"
                            , String.Format("window.open(\"TemplatePreview.aspx?TemplateId={0}&Control={1}\", \"Preview\", \"width=760 height=400\")"
                            , this._selectedNode.Template.Id
                            , this.ddlPlaceholder.ClientID)
                            );
                        this.btnSave.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }
                }
                else
                {
                    this.ddlPlaceholder.Visible = false;
                    this.btnSave.Enabled = false;
                    this.hplLookup.Visible = false;
                }
            }
        }

        protected void AddAvailableNodes(IList<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                int indentSpaces = node.Level * 5;
                string itemIndentSpaces = String.Empty;
                for (int i = 0; i < indentSpaces; i++)
                {
                    itemIndentSpaces += "&nbsp;";
                }
                ListItem li = new ListItem(Context.Server.HtmlDecode(itemIndentSpaces) + node.Title, node.Id.ToString());
                this.lbxAvailableNodes.Items.Add(li);
                if (node.ChildNodes.Count > 0)
                {
                    AddAvailableNodes(node.ChildNodes);
                }
            }
        }

        protected void BtnBackClick(object sender, System.EventArgs e)
        {
            Context.Response.Redirect("~/Admin/Sections.aspx");
        }

        protected void DdlSitesSelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindNodes();
        }

        protected void LbxAvailableNodesSelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindPlaceholders();
        }

        protected void BtnSaveClick(object sender, System.EventArgs e)
        {
            this._activeSection.Node = this._selectedNode;
            this._activeSection.PlaceholderId = this.ddlPlaceholder.SelectedValue;
            this._selectedNode.Sections.Add(this._activeSection);
            this._activeSection.CalculateNewPosition();

            try
            {
                SectionService.UpdateSection(this._activeSection);
                // Update the full text index to make sure that the content can be found.
                SearchHelper.UpdateIndexFromSection(this._activeSection);

                Context.Response.Redirect("~/Admin/NodeEdit.aspx?NodeId=" + this._selectedNode.Id.ToString());
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.DataAccess;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;
using log4net;

namespace Cuyahoga.Web.Admin
{
    public partial class NodeEdit : AdminBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NodeEdit));
        private ICommonDao _commonDao;

        private void Page_Load(object sender, EventArgs e)
        {
            _commonDao = IoC.Resolve<ICommonDao>();

            this.Title = "Edit node";

            // Note: ActiveNode is handled primarily by the AdminBasePage because other pages use it.
            // ActiveNode is always freshly retrieved (also after postbacks), so it will be tracked by NHibernate.
            if (Context.Request.QueryString["NodeId"] != null
                && Int32.Parse(Context.Request.QueryString["NodeId"]) == -1) 
            {
                // Create an empty new node if NodeId is set to -1 (and also assign it to the active site)
                this.ActiveNode = new Node();
                this.ActiveNode.Site = this.ActiveSite;
                if (Context.Request.QueryString["ParentNodeId"] != null)
                {
                    int parentNodeId = Int32.Parse(Context.Request.QueryString["ParentNodeId"]);
                    this.ActiveNode.ParentNode = NodeService.GetNodeById(parentNodeId);

                    // Copy Site property from parent.
                    this.ActiveNode.Site = this.ActiveNode.ParentNode.Site;

                    if (!this.IsPostBack)
                    {
                        // Set defaults.
                        this.ActiveNode.Template = this.ActiveNode.ParentNode.Template;
                        this.ActiveNode.Culture = this.ActiveNode.ParentNode.Culture;
                        // Copy security from parent.
                        this.ActiveNode.CopyRolesFromParent();
                    }
                }
                else if (Context.Request.QueryString["SiteId"] != null)
                {
                    int siteId = Int32.Parse(Context.Request.QueryString["SiteId"]);
                    
                    this.ActiveNode.Site = SiteService.GetSiteById(siteId);

                    // Set defaults inheriting from site
                    this.ActiveNode.Culture = this.ActiveNode.Site.DefaultCulture;
                    this.ActiveNode.Template = this.ActiveNode.Site.DefaultTemplate;
                }
                // Short description is auto-generated, so we don't need the controls with new nodes.
                this.txtShortDescription.Visible = false;
                this.rfvShortDescription.Enabled = false;
                this.revShortDescription.Enabled = false;
            }
            if (!this.IsPostBack)
            {
                // There could be a section movement in the request. Check this and move sections if necessary.
                if (Context.Request.QueryString["SectionId"] != null && Context.Request.QueryString["Action"] != null)
                {
                    MoveSections();
                }
                else
                {
                    if (this.ActiveNode != null)
                    {
                        BindNodeControls();
                        BindSections();
                        if (this.ActiveNode.IsRootNode)
                        {
                            //BindMenus();
                        }
                    }
                    BindCultures();
                    BindTemplates();
                    BindRoles();
                }
            }
            if (this.ActiveNode != null)
            {
                BindPositionButtonsVisibility();
            }

            //Custom - Do not allow site admins to add templates (only system admins with user.site == null)
            User currentUser = Context.User.Identity as User;
            if (!currentUser.IsInRole("Administrator"))//If is a site admin but not an OVERALL ADMIN ie/(only system admins with user.site == null)
            {
                hplAddTemplate0.Enabled = false;
            }
        }

        private void BindNodeControls()
        {
            this.txtTitle.Text = this.ActiveNode.Title;
            this.txtTitleSEO.Text = this.ActiveNode.TitleSEO;
            this.txtCSSClass.Text = this.ActiveNode.CSSClass;
            this.txtShortDescription.Text = this.ActiveNode.ShortDescription;
            if (this.ActiveNode.ParentNode != null)
            {
                this.lblParentNode.Text = this.ActiveNode.ParentNode.Title;
            }
            this.chkShowInNavigation.Checked = this.ActiveNode.ShowInNavigation;
            this.txtMetaDescription.Text = this.ActiveNode.MetaDescription;
            this.txtMetaKeywords.Text = this.ActiveNode.MetaKeywords;

            this.chkLink.Enabled = this.ActiveNode.Sections.Count == 0;
            if (this.ActiveNode.IsExternalLink)
            {
                this.chkLink.Checked = true;
                this.pnlLink.Visible = true;
                //this.pnlMenus.Visible = false;
                this.pnlTemplate.Visible = false;
                this.pnlSections.Visible = false;
                this.txtLinkUrl.Text = this.ActiveNode.LinkUrl;
                this.ddlLinkTarget.Items.FindByValue(this.ActiveNode.LinkTarget.ToString()).Selected = true;
            }
            // main buttons visibility
            btnNew.Visible = (this.ActiveNode.Id > 0);
            btnNew2.Visible = (this.ActiveNode.Id > 0);
            btnDelete.Visible = (this.ActiveNode.Id > 0);
            btnDelete2.Visible = (this.ActiveNode.Id > 0);
            btnDuplicate.Visible = (this.ActiveNode.Id > 0);
            btnDuplicate2.Visible = (this.ActiveNode.Id > 0);
            btnDelete.Attributes.Add("onclick", "return confirmDeleteNode();");
            btnDelete2.Attributes.Add("onclick", "return confirmDeleteNode();");

            // custom template to all pages confirm
            btnApplyToAll.Attributes.Add("onclick", "return confirmSiteApplyTemplate();");
        }

        private void BindCultures()
        {
            this.ddlCultures.DataSource = Globalization.GetOrderedCultures();
            this.ddlCultures.DataValueField = "Key";
            this.ddlCultures.DataTextField = "Value";
            this.ddlCultures.DataBind();
            if (this.ActiveNode.Culture != null)
            {
                ddlCultures.Items.FindByValue(this.ActiveNode.Culture).Selected = true;
            }
        }

        private void BindPositionButtonsVisibility()
        {
            // node location buttons visibility
            btnUp.Visible = (this.ActiveNode.Position > 0);
            btnDown.Visible = ((this.ActiveNode.ParentNode != null) && (this.ActiveNode.Position != this.ActiveNode.ParentNode.ChildNodes.Count - 1) && this.ActiveNode.Id != -1);
            btnLeft.Visible = (this.ActiveNode.Level > 0 && this.ActiveNode.Id != -1 && this.ActiveNode.ParentNode.ParentNode != null);
            btnRight.Visible = (this.ActiveNode.Position > 0);
        }

        private void BindTemplates()
        {
            IList templates;

            if (this.ActiveSite != null)
            {
                int activeSiteID = this.ActiveSite.Id;
                Site CurrentSite = this.SiteService.GetSiteById(this.ActiveSite.Id);
                templates = this.TemplateService.GetAllTemplatesBySite(CurrentSite) as IList;
            }
            else
            {
                //Get all of the template (the legacy way)
                templates = this.TemplateService.GetAllTemplates();
            }

            // Bind
            this.ddlTemplates.DataSource = templates;
            this.ddlTemplates.DataValueField = "Id";
            this.ddlTemplates.DataTextField = "Name";
            this.ddlTemplates.DataBind();
            if (this.ActiveNode != null && this.ActiveNode.Template != null)
            {
                ListItem li = ddlTemplates.Items.FindByValue(this.ActiveNode.Template.Id.ToString());
                if (li != null)
                {
                    li.Selected = true;
                }
            }

            if (this.ddlTemplates.Items.Count < 1)
            {
                Response.Redirect("~/Admin/TemplateEdit.aspx?TemplateId=-1");
            }
        }

        private void BindSections()
        {
            IList<Section> sortedSections = SectionService.GetSortedSectionsByNode(this.ActiveNode);

            // Synchronize sections, otherwise we'll have two collections with the same Sections
            this.ActiveNode.Sections = sortedSections;
            this.rptSections.DataSource = sortedSections;
            this.rptSections.DataBind();
            if (this.ActiveNode.Id > 0 && this.ActiveNode.Template != null)
            {
                // Also enable add section link
                this.hplNewSection.NavigateUrl = String.Format("~/Admin/SectionEdit.aspx?SectionId=-1&NodeId={0}", this.ActiveNode.Id);
                this.hplNewSection.Visible = true;
            }
        }

        private void BindRoles()
        {
            IList roles = base.UserService.GetAllRoles();
            this.rptRoles.ItemDataBound += rptRoles_ItemDataBound;
            this.rptRoles.DataSource = roles;
            this.rptRoles.DataBind();
        }

        private void SetTemplate()
        {
            if (this.ddlTemplates.Visible && this.ddlTemplates.SelectedValue != "-1")
            {
                int templateId = Int32.Parse(this.ddlTemplates.SelectedValue);
                this.ActiveNode.Template = TemplateService.GetTemplateById(templateId);
            }
        }

        private void SetRoles()
        {
            this.ActiveNode.NodePermissions.Clear();
            foreach (RepeaterItem ri in rptRoles.Items)
            {
                // HACK: RoleId is stored in the ViewState because the repeater doesn't have a DataKeys property.
                CheckBox chkView = (CheckBox)ri.FindControl("chkViewAllowed");
                CheckBox chkEdit = (CheckBox)ri.FindControl("chkEditAllowed");
                if (chkView.Checked || chkEdit.Checked)
                {
                    NodePermission np = new NodePermission();
                    np.Node = this.ActiveNode;
                    np.Role = UserService.GetRoleById((int)ViewState[ri.ClientID]);

                    np.ViewAllowed = chkView.Checked;
                    np.EditAllowed = chkEdit.Checked;
                    this.ActiveNode.NodePermissions.Add(np);
                }
            }
        }

        private void SaveNode()
        {
            _commonDao.RemoveQueryFromCache("Nodes");

            if (this.ActiveNode.Id > 0)
            {
                NodeService.UpdateNode(this.ActiveNode, this.chkPropagateToChildNodes.Checked, this.chkPropagateToSections.Checked);
            }
            else
            {
                IList rootNodes = NodeService.GetRootNodes(this.ActiveNode.Site).ToList();

                this.ActiveNode.CalculateNewPosition(rootNodes);

                // Add node to the parent node's ChildNodes first
                if (this.ActiveNode.ParentNode != null)
                {
                    this.ActiveNode.ParentNode.ChildNodes.Add(this.ActiveNode);
                }
                NodeService.SaveNode(this.ActiveNode);
                Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}&message=Node created sucessfully", this.ActiveNode.Id));
            }
        }

        private void MoveSections()
        {
            int sectionId = Int32.Parse(Context.Request.QueryString["SectionId"]);
            Section section = SectionService.GetSectionById(sectionId);

            section.Node = this.ActiveNode;
            if (Context.Request.QueryString["Action"] == "MoveUp")
            {
                section.MoveUp();
                _commonDao.Flush();

                // reset sections, so they will be refreshed from the database when required.
                this.ActiveNode.ResetSections();
            }
            else if (Context.Request.QueryString["Action"] == "MoveDown")
            {
                section.MoveDown();
                _commonDao.Flush();

                // reset sections, so they will be refreshed from the database when required.
                this.ActiveNode.ResetSections();
            }
            // Redirect to the same page without the section movement parameters
            Context.Response.Redirect(Context.Request.Path + String.Format("?NodeId={0}", this.ActiveNode.Id));
        }

        private void SetShortDescription()
        {
            // TODO: check uniqueness. It's now handled by the database constraint but that is not
            // too descriptive.
            if (this.ActiveNode.Id > 0)
            {
                this.ActiveNode.ShortDescription = this.txtShortDescription.Text;
            }
            else
            {
                // Generate the short description for new nodes.
                this.ActiveNode.CreateShortDescription();
            }
        }

        private void MoveNode(NodePositionMovement npm)
        {
            _commonDao.RemoveQueryFromCache("Nodes");
            IList<Node> rootNodes = NodeService.GetRootNodes(this.ActiveNode.Site);

            this.ActiveNode.Move(rootNodes, npm);
            _commonDao.Flush();

            Context.Response.Redirect(Context.Request.RawUrl);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetTemplate();
                if (this.IsValid)
                {
                    this.ActiveNode.Site = this.ActiveSite;
                    this.ActiveNode.Title = this.txtTitle.Text;
                    this.ActiveNode.TitleSEO = this.txtTitleSEO.Text;
                    this.ActiveNode.CSSClass = this.txtCSSClass.Text;
                    this.ActiveNode.Culture = this.ddlCultures.SelectedValue;
                    this.ActiveNode.ShowInNavigation = this.chkShowInNavigation.Checked;
                    this.ActiveNode.MetaDescription = this.txtMetaDescription.Text.Trim().Length > 0
                    ? this.txtMetaDescription.Text.Trim()
                    : null;
                    this.ActiveNode.MetaKeywords = this.txtMetaKeywords.Text.Trim().Length > 0
                        ? this.txtMetaKeywords.Text.Trim()
                        : null;
                    if (this.chkLink.Checked)
                    {
                        this.ActiveNode.LinkUrl = this.txtLinkUrl.Text;
                        this.ActiveNode.LinkTarget = (LinkTarget)Enum.Parse(typeof(LinkTarget), this.ddlLinkTarget.SelectedValue);
                    }
                    else  // rabol: [#CUY-51] - Clear the link in the database
                    {
                        this.ActiveNode.LinkUrl = null;
                        this.ActiveNode.LinkTarget = LinkTarget.Self;
                    }

                    this.ActiveNode.Validate();
                    SetShortDescription();
                    SetRoles();
                    SaveNode();
                    ShowMessage("Node saved.");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ", " + ex.InnerException.Message;
                }
                ShowError(msg);
                log.Error("Error saving Node", ex);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            // Create an url with NodeId -1 and the Id of the current node as ParentId
            string url = String.Format("NodeEdit.aspx?NodeId=-1&ParentNodeId={0}", this.ActiveNode.Id);
            // Redirect to the new url
            Context.Response.Redirect(url);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.ActiveNode.Id == -1 && this.ActiveNode.ParentNode != null)
            {
                Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", this.ActiveNode.ParentNode.Id));
            }
            else
            {
                Context.Response.Redirect("Default.aspx");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.ActiveNode.Sections.Count > 0)
            {
                this.ShowError("Can't delete a node when there are sections attached. Please delete or detach all sections first.");
            }
            else if (this.ActiveNode.ChildNodes.Count > 0)
            {
                this.ShowError("Can't delete a node when there are child nodes attached. Please delete all childnodes first.");
            }
            else
            {
                try
                {
                    _commonDao.RemoveQueryFromCache("Nodes");

                    bool hasParentNode = (this.ActiveNode.ParentNode != null);
                    if (hasParentNode)
                    {
                        this.ActiveNode.ParentNode.ChildNodes.Remove(this.ActiveNode);
                    }
                    else
                    {
                        IList rootNodes = NodeService.GetRootNodes(this.ActiveNode.Site).ToList();
                        rootNodes.Remove(this.ActiveNode);
                    }
                    NodeService.DeleteNode(this.ActiveNode);

                    // Reset the position of the 'neighbour' nodes.
                    if (this.ActiveNode.Level == 0)
                    {
                        this.ActiveNode.ReOrderNodePositions(this.ActiveNode.Site.RootNodes, this.ActiveNode.Position);
                    }
                    else
                    {
                        this.ActiveNode.ReOrderNodePositions(this.ActiveNode.ParentNode.ChildNodes, this.ActiveNode.Position);
                    }
                    _commonDao.Flush();

                    if (hasParentNode)
                    {
                        Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", this.ActiveNode.ParentNode.Id));
                    }
                    else
                    {
                        Context.Response.Redirect("Default.aspx");
                    }
                }
                catch (Exception ex)
                {
                    this.ShowError(ex.Message);
                    log.Error(String.Format("Error deleting Node: {0}.", this.ActiveNode.Id), ex);
                }
            }
        }

        protected void ddlTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetTemplate();
                // Also save the current node (validate first)
                this.ActiveNode.Title = this.txtTitle.Text;
                this.ActiveNode.Culture = this.ddlCultures.SelectedValue;
                Validate();
                if (this.IsValid)
                {
                    SetShortDescription();
                    SetRoles();
                    this.SaveNode();
                    this.ShowMessage("Node saved while setting the template.");
                    BindSections();
                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex.Message);
                log.Error("Error while switching the Template.", ex);
            }
        }

        protected void btnUp_Click(object sender, ImageClickEventArgs e)
        {
            MoveNode(NodePositionMovement.Up);
        }

        protected void btnDown_Click(object sender, ImageClickEventArgs e)
        {
            MoveNode(NodePositionMovement.Down);
        }

        protected void btnLeft_Click(object sender, ImageClickEventArgs e)
        {
            MoveNode(NodePositionMovement.Left);
        }

        protected void btnRight_Click(object sender, ImageClickEventArgs e)
        {
            MoveNode(NodePositionMovement.Right);
        }

        protected void rptSections_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Section section = e.Item.DataItem as Section;
            if (section != null)
            {
                HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");

                // HACK: as long as ~/ doesn't work properly in mono we have to use a relative path from the Controls
                // directory due to the template construction.
                hplEdit.NavigateUrl = String.Format("~/Admin/SectionEdit.aspx?SectionId={0}&NodeId={1}", section.Id, this.ActiveNode.Id);
                if (section.CanMoveUp())
                {
                    HyperLink hplSectionUp = (HyperLink)e.Item.FindControl("hplSectionUp");
                    hplSectionUp.NavigateUrl = Context.Request.RawUrl + String.Format("&SectionId={0}&Action=MoveUp", section.Id);
                    hplSectionUp.Visible = true;
                }
                if (section.CanMoveDown())
                {
                    HyperLink hplSectionDown = (HyperLink)e.Item.FindControl("hplSectionDown");
                    hplSectionDown.NavigateUrl = Context.Request.RawUrl + String.Format("&SectionId={0}&Action=MoveDown", section.Id);
                    hplSectionDown.Visible = true;
                }
                LinkButton lbtDelete = (LinkButton)e.Item.FindControl("lbtDelete");
                lbtDelete.Attributes.Add("onclick", "return confirm('Are you sure?')");

                // Check if the placeholder exists in the currently attached template
                BaseTemplate templateControl = (BaseTemplate)this.LoadControl(UrlHelper.GetApplicationPath() + this.ActiveNode.Template.Path);
                Label lblNotFound = (Label)e.Item.FindControl("lblNotFound");
                lblNotFound.Visible = (templateControl.Containers[section.PlaceholderId] == null);

                // added for 1.6.0
                HyperLink hplAdmin = (HyperLink)e.Item.FindControl("hplAdmin");
                if (hplAdmin != null)
                {
                    if (section.ModuleType.EditPath != null && section.ModuleType.EditPath != string.Empty)
                    {
                        hplAdmin.NavigateUrl = String.Format(
                            "{0}?NodeId={1}&SectionId={2}"
                            , UrlHelper.GetApplicationPath() + section.ModuleType.EditPath
                            , section.Node.Id
                            , section.Id);
                        hplAdmin.Visible = true;
                    }
                    else
                    {
                        hplAdmin.Visible = false;
                    }
                }
                // added for 1.6.0
            }
        }

        protected void rptRoles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Role role = e.Item.DataItem as Role;
            if (role != null)
            {
                CheckBox chkView = (CheckBox)e.Item.FindControl("chkViewAllowed");
                chkView.Checked = this.ActiveNode.ViewAllowed(role);
                CheckBox chkEdit = (CheckBox)e.Item.FindControl("chkEditAllowed");
                
                //if (role.HasPermission(AccessLevel.Editor) || role.HasPermission(AccessLevel.Administrator))
                //if (role.Name == "Editor" || role.Name == "Site Administrator" || role.Name == "Administrator")
                if (role.HasRight(Rights.ManageSections))
                {
                    chkEdit.Checked = this.ActiveNode.EditAllowed(role);
                }
                else
                {
                    chkEdit.Visible = false;
                }
                // Add RoleId to the ViewState with the ClientID of the repeateritem as key.
                this.ViewState[e.Item.ClientID] = role.Id;
            }
        }

        protected void rptSections_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete" || e.CommandName == "Detach")
            {
                int sectionId = Int32.Parse(e.CommandArgument.ToString());
                Section section = SectionService.GetSectionById(sectionId);

                if (e.CommandName == "Delete")
                {
                    section.Node = this.ActiveNode;
                    try
                    {
                        // First tell the module to remove its content.
                        ModuleBase module = ModuleLoader.GetModuleFromSection(section);
                        module.DeleteModuleContent();
                        // Make sure there is no gap in the section indexes. 
                        // ABUSE: this method was not designed for this, but works fine.
                        section.ChangeAndUpdatePositionsAfterPlaceholderChange(section.PlaceholderId, section.Position, false);
                        // Now delete the Section.
                        this.ActiveNode.Sections.Remove(section);

                        SectionService.DeleteSection(section, module);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                        log.Error(String.Format("Error deleting section : {0}.", section.Id), ex);
                    }
                }
                if (e.CommandName == "Detach")
                {
                    try
                    {
                        // Make sure there is no gap in the section indexes. 
                        // ABUSE: this method was not designed for this, but works fine.
                        section.ChangeAndUpdatePositionsAfterPlaceholderChange(section.PlaceholderId, section.Position, false);
                        // Now detach the Section.
                        this.ActiveNode.Sections.Remove(section);
                        section.Node = null;
                        section.PlaceholderId = null;

                        SectionService.UpdateSection(section);
                        // Update search index to make sure the content of detached sections doesn't 
                        // show up in a search.
                        SearchHelper.UpdateIndexFromSection(section);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                        log.Error(String.Format("Error detaching section : {0}.", section.Id), ex);
                    }
                }
                BindSections();
            }
        }

        protected void chkLink_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlLink.Visible = this.chkLink.Checked;
            this.pnlTemplate.Visible = !this.chkLink.Checked;
            this.pnlSections.Visible = !this.chkLink.Checked;
        }

        protected void btnApplyToAll_Click(object sender, EventArgs e)
        {
            int templateId = int.Parse(ddlTemplates.SelectedValue);
            Template t = this.TemplateService.GetTemplateById(templateId);
            Site s = this.SiteService.GetSiteById(this.ActiveSite.Id);

            this.NodeService.ApplyTemplateAllNodesInSite(t, s);
            this.ShowMessage("Template '" + t.Name + "' set to all pages successfully.");
        }

        // added for v1.6.0
        /// <summary>
        /// Handles the Click event of the btnDuplicate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnDuplicate_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActiveNode == null)
                {
                    ShowError("btnDuplicate_Click:: non starting node found.");
                    return;
                }

                Node node = new Node();
                node.ParentNode = ActiveNode.ParentNode;
                node.Site = ActiveNode.Site;
                node.Title = "Copy of " + ActiveNode.Title;
                node.Template = ActiveNode.Template;
                node.Culture = ActiveNode.Culture;
                node.LinkUrl = ActiveNode.LinkUrl;
                node.MetaDescription = ActiveNode.MetaDescription;
                node.MetaKeywords = ActiveNode.MetaKeywords;
                node.LinkTarget = ActiveNode.LinkTarget;
                node.ShowInNavigation = ActiveNode.ShowInNavigation;

                node.CreateShortDescription();

                foreach (NodePermission np in ActiveNode.NodePermissions)
                {
                    NodePermission npNew = new NodePermission();
                    npNew.Node = node;
                    npNew.Role = np.Role;
                    npNew.ViewAllowed = np.ViewAllowed;
                    npNew.EditAllowed = np.EditAllowed;
                    node.NodePermissions.Add(npNew);
                }

                IList rootNodes = NodeService.GetRootNodes(node.Site).ToList();

                node.CalculateNewPosition(rootNodes);
                ActiveNode.ChildNodes.Add(node);

                NodeService.SaveNode(node);

                CopySectionsFromNode(ActiveNode, node);

                _commonDao.RemoveQueryFromCache("Nodes");
                _commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Node.ChildNodes");

                Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}&message=Node has been duplicated.", node.Id));

            }
            catch (Exception ee)
            {
                ShowException(ee);
            }
        }

        /// <summary>
        /// Copies the sections from node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="nodeTarget">The node target.</param>
        private void CopySectionsFromNode(Node node, Node nodeTarget)
        {
            foreach (Section section in node.Sections)
            {
                Section newsection = new Section();

                newsection.Node = nodeTarget;

                newsection.CacheDuration = section.CacheDuration;

                foreach (KeyValuePair<string, Section> entry in section.Connections)
                {
                    newsection.Connections.Add(entry.Key, entry.Value);
                }

                newsection.ModuleType = section.ModuleType;
                newsection.PlaceholderId = section.PlaceholderId;
                newsection.Position = section.Position;

                // copy module settings
                foreach (DictionaryEntry sectionitem in section.Settings)
                {
                    newsection.Settings.Add(sectionitem.Key, sectionitem.Value);
                }

                newsection.ShowTitle = section.ShowTitle;
                newsection.Title = section.Title;

                newsection.CopyRolesFromNode();
                newsection.CalculateNewPosition();

                SectionService.SaveSection(newsection);

            }
        }
        // added for v1.6.0
    }
}

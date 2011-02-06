using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Web.Util;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Service;

namespace Cuyahoga.Web.Admin
{
    /// <summary>
    /// Web service for Cuyahoga site and node management
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")] //Replace namespace
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class AdminService : Cuyahoga.Web.Admin.AdminServices.AdminWebService
    {
        #region Constructor
            public AdminService() {
                CurrentUser = Context.User.Identity as User;
            }
        #endregion

        #region protected
            protected System.Web.UI.WebControls.PlaceHolder plhNodes;
        #endregion

        #region private
            private User _currentUser;
            private Node _activeNode;
            private Node _sourceNode;
            private Node _destinationNode;
            private bool _showAll;
        #endregion private

        #region protected
            protected User CurrentUser
            {
                get
                {
                    return _currentUser;
                }
                set
                {
                    _currentUser = value;
                }
            }
            protected Node ActiveNode
            {
                get
                {
                    return _activeNode;
                }
                set
                {
                    _activeNode = value;
                }
            }
            protected Node SourceNode
            {
                get { return this._sourceNode; }
                set { this._sourceNode = value; }
            }
            protected Node DestinationNode
            {
                get { return this._destinationNode; }
                set { this._destinationNode = value; }
            }
            protected bool ShowAll
            {
                get { return this._showAll; }
                set { _showAll = value; }
            }
        #endregion public

        public class ReturnObject
        {
            bool success;
            int siteId;
            int nodeId;
            string details;
            string sitesHTML;

            public bool Success
            {
                get { return success; }
                set { success = value; }
            }
            public int SiteId
            {
                get { return siteId; }
                set { siteId = value; }
            }
            public int NodeId
            {
                get { return nodeId; }
                set { nodeId = value; }
            }
            public string Details
            {
                get { return details; }
                set { details = value; }
            }
            public string SitesHTML
            {
                get { return sitesHTML; }
                set { sitesHTML = value; }
            }
        }

        #region Admin Service Methods
            [WebMethod]
            public ReturnObject PasteNode(int sourceSiteId, int sourceNodeId, int destinationSiteId, int destinationNodeId, bool showAll)
            {
                try
                {   
                    this.ShowAll = showAll;
                    this.SourceNode = this.NodeService.GetNodeById(sourceNodeId);
                    this.DestinationNode = this.NodeService.GetNodeById(destinationNodeId);

                    ////When source is in root and destination is root do not count SourceNode in the collection. To complete paste and reorder use below code.
                    //int newPosition = (this.DestinationNode.ParentNode == null && this.SourceNode.ParentNode.ParentNode == null) ? this.DestinationNode.ChildNodes.Count - 1 : this.DestinationNode.ChildNodes.Count;
                    ////or...
                    ////When source is in root and destination is root throw 'Node already exists' exception 
                    int newPosition;
                    if (this.DestinationNode.ParentNode == null && this.SourceNode.ParentNode.ParentNode == null)
                    {
                        throw new Exception("Node already exists in destination.");
                    }
                    else
                    {
                        newPosition = this.DestinationNode.ChildNodes.Count;
                    }
                    
                    SourceNode.ParentNode.ChildNodes.Remove(SourceNode);
                    ReOrderNodePositions(SourceNode.ParentNode.ChildNodes, SourceNode.Position);
                    SourceNode.ParentNode = this.DestinationNode;
                    if (SourceNode.ParentNode != null)
                    {
                        SourceNode.ParentNode.ChildNodes.Add(SourceNode);
                    }

                    SourceNode.Position = newPosition;
                    CommonDao.Flush();

                    string Details = "Node '" + this.SourceNode.Title + "' has been paste to '" + this.DestinationNode.Title + "'";

                    //Build sites list
                    BuildNodeTree(this._destinationNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    return new ReturnObject() { Success = true, SiteId = destinationSiteId, NodeId = destinationNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
                catch (System.Exception excep)
                {
                    //Build sites list
                    BuildNodeTree(this._sourceNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    string Details = "Error: Node '" + this.SourceNode.Title + "' has been paste to '" + this.DestinationNode.Title + "'. " + excep.Message;
                    return new ReturnObject() { Success = false, SiteId = destinationSiteId, NodeId = destinationNodeId, Details = excep.Message, SitesHTML = sitesHTMLTW };
                }
            }

            [WebMethod]
            public ReturnObject MoveNode(int sourceSiteId, int sourceNodeId, string direction, bool showAll)
            {
                this.ShowAll = showAll;
                this.SourceNode = this.NodeService.GetNodeById(sourceNodeId);

                IList<Node> rootNodes = this.SourceNode.Site.RootNodes.OrderBy(x => x.Position) as IList<Node>;

                NodePositionMovement npm = new NodePositionMovement();
                switch (direction)
                {
                    case "up":
                        npm = NodePositionMovement.Up;
                        break;
                    case "down":
                        npm = NodePositionMovement.Down;
                        break;
                    case "left":
                        npm = NodePositionMovement.Left;
                        break;
                    case "right":
                        npm = NodePositionMovement.Right;
                        break;
                }

                try
                {
                    CommonDao.RemoveQueryFromCache("Nodes");

                    this.SourceNode.Move(rootNodes, npm);
                    //this.NodeService.SaveNode(this.SourceNode);

                    CommonDao.Flush();

                    if (npm == NodePositionMovement.Left || npm == NodePositionMovement.Right)
                    CommonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Node.ChildNodes");

                    string Details = "Node '" + this.SourceNode.Title + "' has been moved " + direction;

                    //Build sites list
                    BuildNodeTree(this.SourceNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    return new ReturnObject() { Success = true, SiteId = sourceSiteId, NodeId = sourceNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
                catch (System.Exception excep)
                {
                    //Build sites list
                    BuildNodeTree(this.SourceNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    string Details = "Error: Node: '" + this.SourceNode.Title + "' has not been moved " + direction + ". " + excep.Message;
                    return new ReturnObject() { Success = false, SiteId = sourceSiteId, NodeId = sourceNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
            }

            [WebMethod]
            public ReturnObject DeleteNode(int sourceSiteId, int sourceNodeId, bool showAll)
            {
                this.ShowAll = showAll;
                CommonDao.RemoveQueryFromCache("Nodes");

                this.SourceNode = this.NodeService.GetNodeById(sourceNodeId);

                int destinationSiteId = sourceSiteId;
                int destinationNodeId = this.SourceNode.ParentNode != null ? this.SourceNode.ParentNode.Id : 0;

                try
                {
                    this.NodeService.DeleteNode(this.SourceNode);
                    CommonDao.Flush();

                    string Details = "Node '" + this.SourceNode.Title + "' of site '" + this.SourceNode.Site.Name +"' has been deleted.";

                    //Build sites list
                    BuildNodeTree(this.SourceNode.ParentNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    return new ReturnObject() { Success = true, SiteId = destinationSiteId, NodeId = destinationNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
                catch (System.Exception excep)
                {
                    //Build sites list
                    BuildNodeTree(this.SourceNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    string Details = "Error: Node '" + this.SourceNode.Title + "' of site '" + this.SourceNode.Site.Name + "' could not be deleted. " + excep.Message;
                    return new ReturnObject() { Success = false, SiteId = destinationSiteId, NodeId = destinationNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
            }

            [WebMethod]
            public ReturnObject PasteCopyNode(int sourceSiteId, int sourceNodeId, int destinationSiteId, int destinationNodeId, bool showAll)
            {
                this.ShowAll = showAll;

                this.SourceNode = this.NodeService.GetNodeById(sourceNodeId);
                this.DestinationNode = this.NodeService.GetNodeById(destinationNodeId);

                try
                {
                    CommonDao.RemoveQueryFromCache("Nodes");
                    Node node = NodeService.CopyNode(this.SourceNode.Id, this.DestinationNode.Id);

                    CommonDao.Flush();
                    CommonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Node.ChildNodes");

                    string Details = "A copy of node '" + this.SourceNode.Title + "' has been made at node '" + this.DestinationNode.Title + "'.";

                    //Build sites list
                    BuildNodeTree(this._destinationNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    return new ReturnObject() { Success = true, SiteId = destinationSiteId, NodeId = destinationNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
                catch (Exception ee)
                {
                    //Build sites list
                    BuildNodeTree(this._sourceNode);
                    string sitesHTMLTW = RenderControlString(plhNodes);

                    string Details = "Error: A copy of node '" + this.SourceNode.Title + "' has not been made at node '" + this.DestinationNode.Title + "'. " + ee.Message;
                    return new ReturnObject() { Success = false, SiteId = destinationSiteId, NodeId = destinationNodeId, Details = Details, SitesHTML = sitesHTMLTW };
                }
            }
        #endregion

        #region Admin service helper methods            
            public virtual void ReOrderNodePositions(IList<Node> nodeListWithGap, int gapPosition)
            {
                foreach (Node node in nodeListWithGap)
                {
                    if (node.Position > gapPosition)
                    {
                        node.Position--;
                    }
                }
            }

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

                    // newsection.Settings = section.Settings;

                    if (section.ModuleType.Name.ToLower() == "html")
                    {
                    }

                    newsection.ShowTitle = section.ShowTitle;
                    newsection.Title = section.Title;

                    newsection.CopyRolesFromNode();
                    newsection.CalculateNewPosition();

                    //nodeTarget.Sections.Add(newsection);

                    SectionService.SaveSection(newsection);

                }
            }

            private string RenderControlString(Control ctrl)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter tw = new StringWriter(sb);
                HtmlTextWriter hw = new HtmlTextWriter(tw);

                ctrl.RenderControl(hw);
                return sb.ToString();
            }
        #endregion

        #region Display Sites
            private void BuildNodeTree(Node activeNode)
            {
                this.ActiveNode = activeNode;

                IList sites = SiteService.GetAllSites();
                DisplaySites(sites);
            }

            private void DisplaySites(IList sites)
            {
                this.plhNodes = new PlaceHolder();

                HtmlGenericControl RootContainer = new HtmlGenericControl("div");
                RootContainer.Attributes.Add("class", "rootcontainer");
                this.plhNodes.Controls.Add(RootContainer);

                foreach (Site site in sites)
                {
                    //Custom statement to separate Administrator and Site Administrator
                    if (CurrentUser.IsInRole("Administrator") || (CurrentUser.IsInRole("Site Administrator") && CurrentUser.Sites.Contains(site)))
                    {
                        HtmlGenericControl sitecontainer = CreateDisplaySite(site);
                        RootContainer.Controls.Add(sitecontainer);

                        if (this.ActiveNode != null && this.ActiveNode.Site == site && this.ActiveNode.Id > 0)
                        {
                            RootContainer.Controls.Add(CreateNewChildNodeControl(sitecontainer));
                        }

                        if (CurrentUser.IsInRole("Administrator"))//Only admins create sites (need to formalise how this works)
                        {
                            RootContainer.Controls.Add(CreateNewNodeControl(site, sitecontainer));
                            sitecontainer.Controls.Add(new LiteralControl("<hr/>"));
                        }
                    }
                }
            }

            private HtmlGenericControl CreateDisplaySite(Site site)
            {
                string imgFolder = UrlHelper.GetSiteUrl() + "/Admin/Images/";
                string adminUrl = UrlHelper.GetSiteUrl() + "/Admin/";

                HtmlGenericControl container = new HtmlGenericControl("div");
                container.Attributes.Add("class", "sitepanel");
                container.Attributes.Add("id", "site" + site.Id.ToString());

                HtmlGenericControl siteul = new HtmlGenericControl("ul");
                container.Controls.Add(siteul);

                HtmlGenericControl siteli = new HtmlGenericControl("li");
                siteli.Attributes.Add("class", "site");
                siteli.Attributes.Add("site", site.Id.ToString());

                siteul.Controls.Add(siteli);
                Image img = new Image();
                img.ImageUrl = imgFolder + "site.png";
                img.ImageAlign = ImageAlign.Left;
                img.AlternateText = "Site";
                siteli.Controls.Add(img);

                HyperLink hpl = new HyperLink();
                hpl.Text = String.Format("{0}", site.SiteUrl);
                hpl.NavigateUrl = String.Format("{0}SiteEdit.aspx?SiteId={1}", adminUrl, site.Id.ToString());
                hpl.CssClass = "nodeLink";
                siteli.Controls.Add(hpl);

                DisplayNodes(site.RootNodes, siteli);
                return container;
            }

            private void DisplayNodes(IList<Node> nodes, HtmlGenericControl siteli)
            {
                //Sort the list in case they have been cached after edit
                foreach (Node node in nodes.OrderBy(x => x.Position))
                {
                    Control displayNode = CreateDisplayNode(node);
                    siteli.Controls.Add(displayNode);

                    if (this.ShowAll)
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            // The node is in the trail, expand.
                            Control childDisplayNode = CreateDisplayChild(node);
                            displayNode.Controls.Add(childDisplayNode);
                        }
                    }
                    else
                    {
                        if (this.ActiveNode != null
                            && node.Level <= this.ActiveNode.Level
                            && node.Id == this.ActiveNode.Trail[node.Level])
                        {
                            if (node.ChildNodes.Count > 0)
                            {
                                // The node is in the trail, expand.
                                Control childDisplayNode = CreateDisplayChild(node);
                                displayNode.Controls.Add(childDisplayNode);
                            }
                        }
                    }
                }
            }

            private Control CreateDisplayNode(Node node)
            {
                string nodeText;
                string adminUrl = UrlHelper.GetSiteUrl() + "/Admin/";
                string imgFolder = UrlHelper.GetSiteUrl() + "/Admin/Images/";
                string imgUrl;
                // Display root nodes with their culture.
                if (node.Level == 0)
                {
                    nodeText = node.Title + " (" + node.Culture + ")";
                    imgUrl = imgFolder + "sitepage-home.png";
                }
                else
                {
                    nodeText = node.Title;
                    if (node.ShowInNavigation)
                    {
                        imgUrl = imgFolder + "sitepage-smaller.png";
                    }
                    else
                    {
                        imgUrl = imgFolder + "sitepage-disabled-smaller.png";
                    }
                }

                HtmlGenericControl nodeli = new HtmlGenericControl("li");

                if (node.Level == 0)
                {
                    nodeli.Attributes.Add("class", "rootnode");
                }
                else
                {
                    nodeli.Attributes.Add("class", "node");
                }
                nodeli.Attributes.Add("site", node.Site.Id.ToString());
                nodeli.Attributes.Add("node", node.Id.ToString());

                Image img = new Image();
                img.ImageUrl = imgUrl;
                img.ImageAlign = ImageAlign.Left;
                img.AlternateText = "Node";
                nodeli.Controls.Add(img);

                if (this.ActiveNode != null && node.Id == this.ActiveNode.Id)
                {
                    Label lpl = new Label();
                    lpl.Text = nodeText;
                    lpl.CssClass = "nodeActive";
                    nodeli.Controls.Add(lpl);
                }
                else
                {
                    HyperLink hpl = new HyperLink();
                    hpl.Text = nodeText;
                    hpl.NavigateUrl = String.Format("{0}NodeEdit.aspx?SiteId={1}&NodeId={2}", adminUrl, node.Site.Id.ToString(), node.Id.ToString());
                    hpl.CssClass = "nodeLink";
                    nodeli.Controls.Add(hpl);
                }

                return nodeli;
            }

            private Control CreateDisplayChild(Node node)
            {
                HtmlGenericControl siteul = new HtmlGenericControl("ul");
                DisplayNodes(node.ChildNodes, siteul);
                return siteul;
            }

            private Control CreateNewChildNodeControl(HtmlGenericControl container)
            {
                string adminUrl = UrlHelper.GetSiteUrl() + "/Admin/";
                string imgFolder = UrlHelper.GetSiteUrl() + "/Admin/Images/";

                if (this.ActiveNode != null)
                {
                    Image img = new Image();
                    img.ImageUrl = imgFolder + "sitepage-new.png";
                    img.ImageAlign = ImageAlign.Left;
                    img.AlternateText = "New child node";
                    container.Controls.Add(img);
                    HyperLink hpl = new HyperLink();
                    hpl.Text = "Add child node";
                    hpl.NavigateUrl = String.Format("{0}NodeEdit.aspx?NodeId=-1&ParentNodeId={1}", adminUrl, _activeNode.Id);
                    hpl.CssClass = "navLink";
                    container.Controls.Add(hpl);
                }
                return container;
            }

            private Control CreateNewNodeControl(Site site, HtmlGenericControl container)
            {
                string imgFolder = UrlHelper.GetSiteUrl() + "/Admin/Images/";

                Image img = new Image();
                img.ImageUrl = imgFolder + "sitepage-new-home.png";
                img.ImageAlign = ImageAlign.Left;
                img.AlternateText = "New Node";
                container.Controls.Add(img);
                HyperLink hpl = new HyperLink();
                hpl.Text = "Add new root node";
                hpl.NavigateUrl = String.Format("NodeEdit.aspx?SiteId={0}&NodeId=-1", site.Id.ToString());
                hpl.CssClass = "navLink";
                container.Controls.Add(hpl);
                return container;
            }
        #endregion

    }
}
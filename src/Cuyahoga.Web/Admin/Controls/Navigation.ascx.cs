using System.Collections.Generic;

namespace Cuyahoga.Web.Admin.Controls
{
	using System;
	using System.Collections;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Cuyahoga.Core.Domain;
	using Cuyahoga.Web.Admin.UI;

	/// <summary>
	///		Summary description for Navigation.
	/// </summary>
	public class Navigation : System.Web.UI.UserControl
	{
        private AdminBasePage _page;

		protected System.Web.UI.WebControls.PlaceHolder plhNodes;
        protected System.Web.UI.WebControls.Panel pnlNewSite;
		protected System.Web.UI.WebControls.Image i1;
		protected System.Web.UI.WebControls.Image i2;
		protected System.Web.UI.WebControls.Image i3;
		protected System.Web.UI.WebControls.Image inew;
		protected System.Web.UI.WebControls.HyperLink hplNew;
		protected System.Web.UI.WebControls.HyperLink hplSections;
		protected System.Web.UI.WebControls.HyperLink hplModules;
		protected System.Web.UI.WebControls.HyperLink hplTemplates;
        protected System.Web.UI.WebControls.HyperLink hplCategories;
		protected System.Web.UI.WebControls.Image i5;
		protected System.Web.UI.WebControls.HyperLink hplUsers;
		protected System.Web.UI.WebControls.Image i6;
		protected System.Web.UI.WebControls.HyperLink hplRoles;
		protected System.Web.UI.WebControls.Image i7;
		protected System.Web.UI.WebControls.HyperLink hplRebuild;
		protected System.Web.UI.WebControls.Image i4;

        //Custom
        protected System.Web.UI.WebControls.PlaceHolder plhAdminOnlyOptions;
        protected System.Web.UI.WebControls.PlaceHolder plhAdminOnlyOptionAddSite;
        protected System.Web.UI.WebControls.CheckBox chkShowAll;

        protected User _currentUser;

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

        public Navigation() 
        {
            CurrentUser = Context.User.Identity as User;
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            //Navigation show all nodes session to False if null
            if (Session["ShowAll"] == null)
            {
                Session["ShowAll"] = true;
            }

            try
            {
                this._page = (AdminBasePage)this.Page;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("This control requires a Page of the type AdminBasePage.", ex);
            }

            BuildNodeTree();

            if (!Page.IsPostBack && Request.QueryString["SiteId"] != null)
            {
                hplTemplates.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplUsers.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplSections.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplCategories.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplRebuild.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
            }
            else 
            {
                hplSections.Visible = false;
                hplCategories.Visible = false;
            }

            if (CurrentUser.IsInRole("Site Administrator"))//If user is admin of a specific site (not admin of all sites)
            {
                plhAdminOnlyOptions.Visible = false;
                plhAdminOnlyOptionAddSite.Visible = false;
                pnlNewSite.Visible = false;

                //hplSections.Enabled = false;
                //hplModules.Enabled = false;
                //hplTemplates.Enabled = false;
                //hplUsers.Enabled = false;
                //hplRoles.Enabled = false;
                //hplNew.Enabled = false;
            }

            ////Alternative fine grained rights for admin
            //if (CurrentUser.HasRight("Access Admin"))//If user is admin of a specific site (not admin of all sites)
            //{
            //    plhAdminOnlyOptions.Visible = true;
            //    plhAdminOnlyOptionAddSite.Visible = CurrentUser.HasRight("Manage Site"); //false;
            //    pnlNewSite.Visible = CurrentUser.HasRight("Manage Site"); //false;

            //    hplSections.Enabled = CurrentUser.HasRight("Manage Sections");
            //    hplModules.Enabled = CurrentUser.HasRight("Manage Modules");
            //    hplTemplates.Enabled = CurrentUser.HasRight("Manage Templates");
            //    hplUsers.Enabled = CurrentUser.HasRight("Manage Users");
            //    hplRoles.Enabled = CurrentUser.HasRight("Global Permissions");
            //    hplNew.Enabled = CurrentUser.HasRight("Manage Site");
            //}
        }

		private void BuildNodeTree()
		{
			IList sites = this._page.SiteService.GetAllSites();
			DisplaySites(sites);
		}

        private void ClearNodeTreeControls() 
        {
            this.plhNodes.Controls.Clear();
        }

		private void DisplaySites(IList sites)
		{
			foreach (Site site in sites)
			{
                //Custom statement to separate Administrator and Site Administrator
                if (CurrentUser.IsInRole("Administrator") || (CurrentUser.IsInRole("Site Administrator") && CurrentUser.Sites.Contains(site)) )
                {
                    HtmlGenericControl sitecontainer = CreateDisplaySite(site);
                    this.plhNodes.Controls.Add(sitecontainer);

                    if (this._page.ActiveNode != null && this._page.ActiveNode.Site == site && this._page.ActiveNode.Id > 0)
                    {
                        this.plhNodes.Controls.Add(CreateNewChildNodeControl(sitecontainer));
                    }
                        this.plhNodes.Controls.Add(CreateNewNodeControl(site, sitecontainer));
                        sitecontainer.Controls.Add(new LiteralControl("<hr/>"));
                }
			}
		}

        private HtmlGenericControl CreateDisplaySite(Site site)
        {
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
            img.ImageUrl = "../Images/site.png";
            img.ImageAlign = ImageAlign.Left;
            img.AlternateText = "Site";
            siteli.Controls.Add(img);

            HyperLink hpl = new HyperLink();
            hpl.Text = String.Format("{0}", site.SiteUrl);
            hpl.NavigateUrl = String.Format("../SiteEdit.aspx?SiteId={0}", site.Id.ToString());
            hpl.CssClass = "nodeLink";
            siteli.Controls.Add(hpl);

            DisplayNodes(site.RootNodes, siteli);

            return container;
        }

        private Control CreateDisplayChild(Node node)
        {
            HtmlGenericControl siteul = new HtmlGenericControl("ul");
            DisplayNodes(node.ChildNodes, siteul);
            return siteul;
        }

        private void DisplayNodes(IList<Node> nodes, HtmlGenericControl siteli)
        {
            foreach (Node node in nodes)
            {
                //this.plhNodes.Controls.Add(CreateDisplayNode(node));
                Control displayNode = CreateDisplayNode(node);
                siteli.Controls.Add(displayNode);

                if (Convert.ToBoolean(Session["ShowAll"]))
                {
                    if (node.ChildNodes.Count > 0)
                    {
                        // The node is in the trail, expand.
                        Control childDisplayNode = CreateDisplayChild(node);
                        displayNode.Controls.Add(childDisplayNode);
                    }

                    if (this._page.ActiveNode != null)
                    if (this._page.ActiveNode.Id == node.Id)
                    {
                        // HACK: Replace the activenode with the one found while building the node tree to reduce future 
                        // database calls.
                        this._page.ActiveNode = node;
                    }
                }
                else
                {
                    if (this._page.ActiveNode != null
                        && node.Level <= this._page.ActiveNode.Level
                        && node.Id == this._page.ActiveNode.Trail[node.Level])
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            // The node is in the trail, expand.
                            Control childDisplayNode = CreateDisplayChild(node);
                            displayNode.Controls.Add(childDisplayNode);
                        }

                        if (this._page.ActiveNode.Id == node.Id)
                        {
                            // HACK: Replace the activenode with the one found while building the node tree to reduce future 
                            // database calls.
                            this._page.ActiveNode = node;
                        }
                    }
                }

            }
        }

        private Control CreateDisplayNode(Node node)
        {
            string nodeText;
            string imgUrl;
            // Display root nodes with their culture.
            if (node.Level == 0)
            {
                nodeText = node.Title + " (" + node.Culture + ")";
                imgUrl = "../Images/sitepage-home.png";
            }
            else
            {
                nodeText = node.Title;
                if (node.ShowInNavigation)
                {
                    imgUrl = "../Images/sitepage-smaller.png";
                }
                else
                {
                    imgUrl = "../Images/sitepage-disabled-smaller.png";
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
                            
            if (this._page.ActiveNode != null && node.Id == this._page.ActiveNode.Id)
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
                hpl.NavigateUrl = String.Format("../NodeEdit.aspx?SiteId={0}&NodeId={1}", node.Site.Id.ToString(), node.Id.ToString());
                hpl.CssClass = "nodeLink";
                nodeli.Controls.Add(hpl);
            }

            return nodeli;
        }

        private Control CreateNewChildNodeControl(HtmlGenericControl container)
		{
			if (this._page.ActiveNode != null)
            {
				Image img = new Image();
				img.ImageUrl = "../Images/sitepage-new.png";
				img.ImageAlign = ImageAlign.Left;
				img.AlternateText = "New child node";
				container.Controls.Add(img);
				HyperLink hpl = new HyperLink();
				hpl.Text = "Add child node";
				hpl.NavigateUrl = String.Format("../NodeEdit.aspx?NodeId=-1&ParentNodeId={0}", this._page.ActiveNode.Id);
				hpl.CssClass = "navLink";
				container.Controls.Add(hpl);
			}
			return container;
		}

		private Control CreateNewNodeControl(Site site, HtmlGenericControl container )
		{
			Image img = new Image();
			img.ImageUrl ="../Images/sitepage-new-home.png";
			img.ImageAlign = ImageAlign.Left;
			img.AlternateText = "New Node";
			container.Controls.Add(img);
			HyperLink hpl = new HyperLink();
			hpl.Text = "Add new root node";
			hpl.NavigateUrl = String.Format("../NodeEdit.aspx?SiteId={0}&NodeId=-1", site.Id.ToString());
			hpl.CssClass = "navLink";
			container.Controls.Add(hpl);
			return container;
		}

		#region Web Form Designer generated code
		    override protected void OnInit(EventArgs e)
		    {
			    //
			    // CODEGEN: This call is required by the ASP.NET Web Form Designer.
			    //
			    InitializeComponent();
			    base.OnInit(e);
		    }
    		
		    /// <summary>
		    ///		Required method for Designer support - do not modify
		    ///		the contents of this method with the code editor.
		    /// </summary>
		    private void InitializeComponent()
		    {
			    this.Load += new System.EventHandler(this.Page_Load);
		    }
		#endregion

        protected void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            //Set show all nodes session on change
            Session["ShowAll"] = chkShowAll.Checked.ToString();
            ClearNodeTreeControls();
            BuildNodeTree();
        }

        protected void chkShowAll_PreRender(object sender, EventArgs e)
        {
            //Set checkbox to session for page reload/redirect
            chkShowAll.Checked = Convert.ToBoolean(Session["ShowAll"].ToString());
        }
	}
}

using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts; 
using System.Web.UI.HtmlControls;
using Cuyahoga.Web.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Modules.Menu.Web
{
    public partial class MenuControl : BaseModuleControl
    {
        private MenuModule _module;
        private Repeater rptMain = new Repeater();
        private Repeater rptbreadcumb = new Repeater();
        private bool flgHasContent = false;
        private PageEngine _page;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._module = this.Module as MenuModule;
            if (this.Page is PageEngine)
            {
                this._page = (PageEngine)this.Page;
            }

            //Requires jQuery
            //jQuery script for pop-up admin panel
            if (this._module.RequiresjQuery())
            {
                string jquery = String.Format("{0}js/jquery-1.4.2.min.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString());
                this._page.RegisterJavascript("jquery", jquery);
                string droppy = String.Format("{0}js/jquery.droppy.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString());
                this._page.RegisterJavascript("droppypath", droppy);
                string droppymenu = String.Format("{0}js/droppymenu.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString());
                this._page.RegisterJavascript("droppymenu", droppymenu);
            }

            if (this._module._typerender == TypeRender.NavigationTree)
            {
                BuildNavigationTree();
            }
            else
            {
                BuildNavigationBreadcrumb();
            }
        }

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            rptMain.ItemDataBound += new RepeaterItemEventHandler(rpt_ItemDataBound);
            rptbreadcumb.ItemDataBound += new RepeaterItemEventHandler(rptdb_ItemDataBound);
        }
     
        #region NavigationBreadcrumb
            private void BuildNavigationBreadcrumb()
            {
                rptbreadcumb.DataSource = base.PageEngine.RootNode.NodePath[0].ChildNodes;
                rptbreadcumb.DataBind();

                //It renders the "ul" label only if there is content
                if (flgHasContent)
                {
                    HtmlGenericControl List = new HtmlGenericControl("ul");
                    List.Attributes.Add("id", _module.Section.Title);
                    List.Attributes.Add("class", "breadcrumb");
                    //check if the "admim" tab has to be rendered
                    if (_module.FirstLevel <= 1)
                    {
                        if (base.PageEngine.CuyahogaUser != null
                            && ( base.PageEngine.CuyahogaUser.IsInRole("Administrator") || base.PageEngine.CuyahogaUser.IsInRole("SiteAdministrator")))//.HasPermission(AccessLevel.Administrator))
                        {
                            HtmlGenericControl lstItem = new HtmlGenericControl("li");
                            HyperLink hpl = new HyperLink();
                            hpl.NavigateUrl = base.PageEngine.ResolveUrl("~/Admin");
                            hpl.Text = "Admin";
                            lstItem.Controls.Add(hpl);
                            List.Controls.Add(lstItem);
                        }
                    }

                    //check if "home" item has to be rendered or not
                    if ( _module.FirstLevel==0)
                    {
                        HyperLink hpli = new HyperLink();
                        HtmlGenericControl lstItemi = new HtmlGenericControl("li");
                        hpli.NavigateUrl = UrlHelper.GetUrlFromNode(base.PageEngine.RootNode);
                        UrlHelper.SetHyperLinkTarget(hpli, base.PageEngine.RootNode);
                        hpli.Text = base.PageEngine.RootNode.Title;
                        lstItemi.Controls.Add(hpli);
                        lstItemi.Attributes.Add("class", "root");
                        List.Controls.Add(lstItemi);                    
                    }

                    //add the "ul" control to the placeholder
                    List.Controls.Add(rptbreadcumb); //add the repeater to de "ul" control
                    plHolder.Controls.Add(List); 
                }
            }
           
            void rptdb_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
                {
                    Node node = (Node)e.Item.DataItem;
                    //check permissions the range of menu level to render. lastLevel=-1 to render untill the end
                    HyperLink hpl = new HyperLink();
                    HtmlGenericControl lstItem = new HtmlGenericControl("li");
                    if (node.Id != this._page.ActiveNode.Id)
                    {
                        hpl.NavigateUrl = UrlHelper.GetUrlFromNode(node);

                    }
                    if (node.Id != this._page.ActiveNode.Id){  }
                    else { lstItem.Attributes.Add("class", "selected"); }

                    UrlHelper.SetHyperLinkTarget(hpl, node);
            
                    //Check if the node is in Trail to apply a specific CSS class
                    foreach(int Id in this._page.ActiveNode.Trail)
                    {
                        if (this._page.ActiveNode.ParentNode != null)
                        {
                            if (node.Id == Id || node.Id == this._page.ActiveNode.Id)  
                            {
                 
                                hpl.Text = node.Title ;                                
                                lstItem.Controls.Add(hpl);
                                flgHasContent = true;
                                e.Item.Controls.Add(lstItem);
                                if (node.ChildNodes.Count > 0)
                                {
                                    if (node.Level >= 1) { renderBreadcrumb(node, e); }
                               }                                
                      
                            }                       
                        }
                        else 
                        {
                          //This  node, is the "home" node, we can show the title of the home in this zone 
                        }
                    }
                }
            }

            void renderBreadcrumb(Node node, RepeaterItemEventArgs e)
            {
                // Bind child nodes
                Repeater rptChildsmiga = new Repeater();
                Control List = new Control();

                //checks if is necesary to render the "ul" control
                if ((node.Level >= _module.FirstLevel) && ((node.Level < _module.LastLevel) || _module.LastLevel == -1))
                {
                    List = new HtmlGenericControl("li");
                }

                List.Controls.Add(rptChildsmiga);
                e.Item.Controls.Add(List);
                rptChildsmiga.ItemDataBound += new RepeaterItemEventHandler(this.rptdb_ItemDataBound);
                rptChildsmiga.DataSource = node.ChildNodes;
                rptChildsmiga.DataBind();
            }
        #endregion

        #region  NavigatrionTree
            private void BuildNavigationTree()
            {
                rptMain.DataSource = base.PageEngine.RootNode.NodePath[0].ChildNodes;
                rptMain.DataBind();
                //It renders the "ul" label only if there is content
                if (flgHasContent)
                {
                    HtmlGenericControl List = new HtmlGenericControl("ul");
                    List.Attributes.Add("id", _module.Section.Title);

                    //check if the "admin" tab has to be rendered
                    if (_module.FirstLevel <= 1)
                    {
                        if (base.PageEngine.CuyahogaUser != null
                            && (base.PageEngine.CuyahogaUser.HasRight("Access Admin")))
                        {
                            HtmlGenericControl lstItem = new HtmlGenericControl("li");
                            HyperLink hpl = new HyperLink();
                            hpl.NavigateUrl = base.PageEngine.ResolveUrl("~/Admin");
                            hpl.Text = "Admin";
                            lstItem.Controls.Add(hpl);
                            List.Controls.Add(lstItem);
                        }
                    }

                    //check if "home" item has to be rendered
                    if (base.PageEngine.RootNode.ShowInNavigation && base.PageEngine.RootNode.ViewAllowed(base.PageEngine.CuyahogaUser) && (base.PageEngine.RootNode.Level >= _module.FirstLevel))
                    {
                        HyperLink hpl = new HyperLink();
                        HtmlGenericControl lstItem = new HtmlGenericControl("li");

                        hpl.NavigateUrl = UrlHelper.GetUrlFromNode(base.PageEngine.RootNode);
                        UrlHelper.SetHyperLinkTarget(hpl, base.PageEngine.RootNode);
                        hpl.Text = base.PageEngine.RootNode.Title;
                        lstItem.Controls.Add(hpl);

                        //Custom - add css class for home if exists
                        bool homeselected = base.PageEngine.ActiveNode.Id == base.PageEngine.RootNode.Id;
                        if (!string.IsNullOrEmpty(base.PageEngine.RootNode.CSSClass) && homeselected)
                        {
                            lstItem.Attributes.Add("class", "selected " + base.PageEngine.RootNode.CSSClass);
                        }
                        else if (homeselected)
                        {
                            lstItem.Attributes.Add("class", "selected");
                        }

                        List.Controls.Add(lstItem);
                    }

                    List.Controls.Add(rptMain); //add the repeater to de "ul" control
                    plHolder.Controls.Add(List); //add the "ul" control to the placeholder
                }
            }

            void rpt_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    Node node = (Node)e.Item.DataItem;
                    HtmlGenericControl lstItem = new HtmlGenericControl("li");

                    //check permissions the range of menu level to render. lastLevel=-1 to render untill the end
                    if ((node.Level >= _module.FirstLevel) && ((node.Level <= _module.LastLevel) || (_module.LastLevel == -1))
                        && node.ShowInNavigation && node.ViewAllowed(base.PageEngine.CuyahogaUser))
                    {
                        HyperLink hpl = new HyperLink();

                        hpl.NavigateUrl = UrlHelper.GetUrlFromNode(node);
                        UrlHelper.SetHyperLinkTarget(hpl, node);
                        hpl.Text = node.Title;
                        lstItem.Controls.Add(hpl);

                        //Check if the node is in Trail to apply a specific CSS class
                        foreach (int Id in base.PageEngine.ActiveNode.Trail)
                        {
                            if (Id == node.Id)
                            {
                                if (!string.IsNullOrEmpty(node.CSSClass))
                                {
                                    lstItem.Attributes.Add("class", "selected " + node.CSSClass);
                                }
                                else 
                                {
                                    lstItem.Attributes.Add("class", "selected");
                                }
                            }
                            else 
                            {
                                if (!string.IsNullOrEmpty(node.CSSClass))
                                {
                                    lstItem.Attributes.Add("class", node.CSSClass);
                                }
                            }
                        }

                        flgHasContent = true;
                        e.Item.Controls.Add(lstItem);
                    }


                    if (this._module.ChildrenWhenClicked == true)
                    {
                        if (node.Level <= base.PageEngine.ActiveNode.Level
                            && node.Id == base.PageEngine.ActiveNode.Trail[node.Level]
                            && node.ChildNodes.Count > 0)
                        {
                            renderChild(node, lstItem);
                        }
                    }
                    else
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            renderChild(node, lstItem);
                        }
                    }
                }
            }

            void renderChild(Node node, HtmlGenericControl listitem)
            {
                // Bind child nodes
                Repeater rptChilds = new Repeater();
                Control List = new Control();

                //checks if is necesary to render the "ul" control
                if ((node.Level >= _module.FirstLevel) && ((node.Level < _module.LastLevel) || _module.LastLevel == -1))
                {
                    List = new HtmlGenericControl("ul");
                }

                List.Controls.Add(rptChilds);

                listitem.Controls.Add(List);
                
                rptChilds.ItemDataBound += new RepeaterItemEventHandler(this.rpt_ItemDataBound);
                rptChilds.DataSource = node.ChildNodes;
                rptChilds.DataBind();
            }
        #endregion
    }
}
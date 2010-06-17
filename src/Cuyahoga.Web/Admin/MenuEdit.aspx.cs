using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Web.Admin
{
    public partial class MenuEdit : AdminBasePage
    {
        private CustomMenu _activeMenu;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Title = "Edit custom menu";
            if (Context.Request.QueryString["MenuId"] != null)
            {
                if (Int32.Parse(Context.Request.QueryString["MenuId"]) == -1)
                {
                    // Create a new CustomMenu instance
                    this._activeMenu = new CustomMenu();
                    this._activeMenu.RootNode = base.ActiveNode;
                    this.btnDelete.Visible = false;
                }
                else
                {
                    // Get Menu data
                    this._activeMenu = (CustomMenu)base.CoreRepository.GetObjectById(typeof(CustomMenu),
                        Int32.Parse(Context.Request.QueryString["MenuId"]));
                    this.btnDelete.Visible = true;
                    this.btnDelete.Attributes.Add("onclick", "return confirm('Are you sure?');");
                }
            }

            if (!this.IsPostBack)
            {
                this.txtName.Text = this._activeMenu.Name;
                BindPlaceholders();
                BindAvailableNodes();
                BindSelectedNodes();
            }
        }

        protected void BindPlaceholders()
        {
            string templatePath = UrlHelper.GetApplicationPath() + this.ActiveNode.Template.Path;
            BaseTemplate template = (BaseTemplate)this.LoadControl(templatePath);
            this.ddlPlaceholder.DataSource = template.Containers;
            this.ddlPlaceholder.DataValueField = "Key";
            this.ddlPlaceholder.DataTextField = "Key";
            this.ddlPlaceholder.DataBind();
            if (this._activeMenu.Id != -1)
            {
                ListItem li = this.ddlPlaceholder.Items.FindByValue(this._activeMenu.Placeholder);
                if (li != null)
                {
                    li.Selected = true;
                }
            }
        }

        protected void BindAvailableNodes()
        {
            IList<Node> rootNodes = this.ActiveNode.Site.RootNodes;
            AddAvailableNodes(rootNodes);
        }

        protected void BindSelectedNodes()
        {
            foreach (Node node in this._activeMenu.Nodes)
            {
                this.lbxSelectedNodes.Items.Add(new ListItem(node.Title, node.Id.ToString()));
                // also remove from available nodes
                ListItem item = this.lbxAvailableNodes.Items.FindByValue(node.Id.ToString());
                if (item != null)
                {
                    this.lbxAvailableNodes.Items.Remove(item);
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

        protected void AttachSelectedNodes()
        {
            this._activeMenu.Nodes.Clear();
            foreach (ListItem item in this.lbxSelectedNodes.Items)
            {
                Node node = (Node)base.CoreRepository.GetObjectById(typeof(Node), Int32.Parse(item.Value));
                this._activeMenu.Nodes.Add(node);
            }
        }

        protected void SaveMenu()
        {
            base.CoreRepository.ClearQueryCache("Menus");

            if (this._activeMenu.Id > 0)
            {
                base.CoreRepository.UpdateObject(this._activeMenu);
            }
            else
            {
                base.CoreRepository.SaveObject(this._activeMenu);
                Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", this.ActiveNode.Id));
            }
        }

        protected void SynchronizeNodeListBoxes()
        {
            // First fetch a fresh list of available nodes because everything has to be
            // nice in place.
            this.lbxAvailableNodes.Items.Clear();
            BindAvailableNodes();
            // make sure the selected nodes are not in the available nodes list.
            int itemCount = this.lbxAvailableNodes.Items.Count;
            for (int i = itemCount - 1; i >= 0; i--)
            {
                ListItem item = this.lbxAvailableNodes.Items[i];
                if (this.lbxSelectedNodes.Items.FindByValue(item.Value) != null)
                {
                    this.lbxAvailableNodes.Items.RemoveAt(i);
                }
            }
        }

        protected void BtnBackClick(object sender, EventArgs e)
        {
            Context.Response.Redirect("NodeEdit.aspx?NodeId=" + this.ActiveNode.Id);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                this._activeMenu.Name = this.txtName.Text;
                this._activeMenu.Placeholder = this.ddlPlaceholder.SelectedValue;
                try
                {
                    AttachSelectedNodes();
                    SaveMenu();
                    ShowMessage("Menu saved");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (this._activeMenu != null)
            {
                base.CoreRepository.ClearQueryCache("Menus");

                try
                {
                    base.CoreRepository.DeleteObject(this._activeMenu);
                    Context.Response.Redirect("NodeEdit.aspx?NodeId=" + this.ActiveNode.Id);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ListItem item = this.lbxAvailableNodes.SelectedItem;
            if (item != null)
            {
                this.lbxSelectedNodes.Items.Add(item);
                item.Selected = false;
            }
            SynchronizeNodeListBoxes();
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            ListItem item = this.lbxSelectedNodes.SelectedItem;
            if (item != null)
            {
                this.lbxSelectedNodes.Items.Remove(this.lbxSelectedNodes.SelectedItem);
                item.Selected = false;
            }
            SynchronizeNodeListBoxes();
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            ListItem item = this.lbxSelectedNodes.SelectedItem;
            if (item != null)
            {
                int origPosition = this.lbxSelectedNodes.SelectedIndex;
                if (origPosition > 0)
                {
                    this.lbxSelectedNodes.Items.Remove(item);
                    this.lbxSelectedNodes.Items.Insert(origPosition - 1, item);
                }
            }
        }

        protected void btnDown_Click(object sender, EventArgs e)
        {
            ListItem item = this.lbxSelectedNodes.SelectedItem;
            if (item != null)
            {
                int origPosition = this.lbxSelectedNodes.SelectedIndex;
                if (origPosition < this.lbxSelectedNodes.Items.Count - 1)
                {
                    this.lbxSelectedNodes.Items.Remove(item);
                    this.lbxSelectedNodes.Items.Insert(origPosition + 1, item);
                }
            }
        }
    }
}

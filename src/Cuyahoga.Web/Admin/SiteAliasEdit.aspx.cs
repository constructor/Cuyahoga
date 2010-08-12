using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{
	/// <summary>
	/// Summary description for SiteAliasEdit.
	/// </summary>
	public partial class SiteAliasEdit : AdminBasePage
	{
		private SiteAlias _activeSiteAlias;

		protected TextBox txtUrl;
		protected RequiredFieldValidator rfvName;
		protected Button btnSave;
		protected Button btnCancel;
		protected Button btnDelete;
		protected DropDownList ddlEntryNodes;

		private void Page_Load(object sender, System.EventArgs e)
		{
			base.Title = "Edit site alias";

			if (Context.Request.QueryString["SiteAliasId"] != null)
			{
				if (Int32.Parse(Context.Request.QueryString["SiteAliasId"]) == -1)
				{
					// Create a new site alias instance
					this._activeSiteAlias = new SiteAlias();
					if (Context.Request.QueryString["SiteId"] != null)
					{
						this._activeSiteAlias.Site = base.SiteService.GetSiteById(Int32.Parse(Request.QueryString["SiteId"]));
					}
					else
					{
						throw new Exception("No site specified for the new alias.");
					}
					this.btnDelete.Visible = false;
				}
				else
				{
					// Get site alias data
					this._activeSiteAlias = base.SiteService.GetSiteAliasById(Int32.Parse(Request.QueryString["SiteAliasId"]));
					this.btnDelete.Visible = true;
					this.btnDelete.Attributes.Add("onclick", "return confirm('Are you sure?')");
				}
				if (! this.IsPostBack)
				{
					BindSiteAliasControls();
					BindAvailableNodes();
				}
			}
		}

		private void BindSiteAliasControls()
		{
			this.txtUrl.Text = this._activeSiteAlias.Url;
		}

		private void BindAvailableNodes()
		{
			// First create an option for the default root node of the site.
			ListItem li = new ListItem("-- inherit from site --", "-1");
			this.ddlEntryNodes.Items.Add(li);
			IList<Node> rootNodes = this._activeSiteAlias.Site.RootNodes;
			AddAvailableNodes(rootNodes);
		}

		private void AddAvailableNodes(IList<Node> nodes)
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
				li.Selected = (this._activeSiteAlias.EntryNode != null && this._activeSiteAlias.EntryNode.Id == node.Id);
				this.ddlEntryNodes.Items.Add(li);
				if (node.ChildNodes.Count > 0)
				{
					AddAvailableNodes(node.ChildNodes);
				}
			}
		}


		protected void BtnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("SiteEdit.aspx?SiteId=" + this._activeSiteAlias.Site.Id.ToString());
		}

        protected void BtnSave_Click(object sender, System.EventArgs e)
		{
			if (this.IsValid)
			{
				this._activeSiteAlias.Url = this.txtUrl.Text;
				if (this.ddlEntryNodes.SelectedIndex > 0)
				{
					int entryNodeId = Int32.Parse(this.ddlEntryNodes.SelectedValue);
					this._activeSiteAlias.EntryNode = base.NodeService.GetNodeById(entryNodeId);
				}
				else
				{
					this._activeSiteAlias.EntryNode = null;
				}
				try
				{
					base.SiteService.SaveSiteAlias(this._activeSiteAlias);
					Response.Redirect("SiteEdit.aspx?SiteId=" + this._activeSiteAlias.Site.Id.ToString());
				}
				catch(Exception ex)
				{
					ShowError(ex.Message);
				}
			}
		}

        protected void BtnDelete_Click(object sender, System.EventArgs e)
		{
			if (this._activeSiteAlias != null)
			{
				try
				{
					base.SiteService.DeleteSiteAlias(this._activeSiteAlias);
					Context.Response.Redirect("SiteEdit.aspx?SiteId=" + this._activeSiteAlias.Site.Id.ToString());
				}
				catch (Exception ex)
				{
					ShowError(ex.Message);
				}
			}
		}

	}
}

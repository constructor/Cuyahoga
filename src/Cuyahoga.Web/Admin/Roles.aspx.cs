using System;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{

    public partial class Roles : AdminBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Roles";
            if (!this.IsPostBack)
            {
                BindRoles();

                if (Request.QueryString["SiteId"] != null && Request.QueryString["SiteId"] != "-1")
                {
                    btnNew.Enabled = true;
                }
                else
                {
                    btnNew.Enabled = false;
                }
            }

        }

        protected void BindRoles()
        {
            this.rptRoles.DataSource = base.UserService.GetAllRoles();
            this.rptRoles.DataBind();
        }

        protected void RptRolesItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Role role = e.Item.DataItem as Role;
            if (role != null)
            {
                HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");
                Image imgRole = (Image)e.Item.FindControl("imgRole");
                if (role.Name == "Administrator" || role.Name == "Site Administrator" || role.Name == "Anonymous user")
                {
                    imgRole.ImageUrl = "~/Admin/Images/lock.png";
                    hplEdit.Visible = false;
                }
                else
                {
                    imgRole.Visible = false;
                    // HACK: as long as ~/ doesn't work properly in mono we have to use a relative path from the Controls
                    // directory due to the template construction.
                    hplEdit.NavigateUrl = String.Format("~/Admin/RoleEdit.aspx?RoleId={0}", role.Id);
                }
                // Permissions
                Label lblRights = (Label)e.Item.FindControl("lblRights");
                lblRights.Text = role.RightsString;
                // Last update
                Label lblLastUpdate = (Label)e.Item.FindControl("lblLastUpdate");
                lblLastUpdate.Text = role.UpdateTimestamp.ToString();
            }
        }

        protected void BtnNewClick(object sender, EventArgs e)
        {
            string redirectUrl = String.Format("~/Admin/RoleEdit.aspx?RoleId=-1&SiteId={0}", this.ActiveSite.Id);
            Response.Redirect(redirectUrl);
            //Context.Response.Redirect("RoleEdit.aspx?RoleId=-1");
        }

    }

}

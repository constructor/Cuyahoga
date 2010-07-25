using System;
using System.Linq;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{
    public partial class RoleEdit : AdminBasePage
    {
        private Role _activeRole;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Edit role";

            if (Context.Request.QueryString["RoleId"] != null)
            {
                if (Int32.Parse(Context.Request.QueryString["RoleId"]) == -1)
                {
                    this._activeRole = new Role();
                }
                else
                {
                    this._activeRole = base.UserService.GetRoleById(Int32.Parse(Context.Request.QueryString["RoleId"]));
                }

                if (!this.IsPostBack)
                {
                    BindRoleControls();
                    BindRights();
                }
            }
        }

        protected void BindRoleControls()
        {
            this.txtName.Text = this._activeRole.Name;
            this.btnDelete.Visible = (this._activeRole.Id > 0);
            this.btnDelete.Attributes.Add("onclick", "return confirm(\"Ary you sure?\")");
        }

        protected void BindRights()
        {
            this.cblRights.DataSource = base.UserService.GetAllRights().OrderBy(x => x.Id);
            this.cblRights.DataValueField = "Id";
            this.cblRights.DataTextField = "Name";
            this.cblRights.DataBind();
            foreach (Right right in this._activeRole.Rights)
            {
                ListItem li = cblRights.Items.FindByValue(right.Id.ToString());
                li.Selected = true;
            }
        }

        protected void SetPermissions()
        {
            this._activeRole.Rights.Clear();
            foreach (ListItem listItem in this.cblRights.Items)
            {
                if (listItem.Selected)
                {
                    int rightId = Int32.Parse(listItem.Value);
                    this._activeRole.Rights.Add(base.UserService.GetRightById(rightId));
                }
            }
        }

        protected void SaveRole()
        {
            try
            {
                if (this._activeRole.Id == -1)
                {
                    UserService.CreateRole(this._activeRole, this.ActiveSite);
                    Context.Response.Redirect("Roles.aspx");
                }
                else
                {
                    UserService.UpdateRole(this._activeRole, this.ActiveSite);
                    ShowMessage("Role saved");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void BtnCancelClick(object sender, EventArgs e)
        {
            Context.Response.Redirect("Roles.aspx");
        }

        protected void BtnSaveClick(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                this._activeRole.Name = txtName.Text;
                SetPermissions();
                if (this._activeRole.Rights.Count == 0)
                {
                    ShowError("Please select one or more Right(s)");
                }
                else
                {
                    SaveRole();
                }
            }
        }

        protected void BtnDeleteClick(object sender, EventArgs e)
        {
            if (this._activeRole.Id > 0)
            {
                if (this._activeRole.Name == "AdministratorRole" || this._activeRole.Name == "Anonymous User")
                {
                    ShowError("You can't delete the Administrator Role or the Anonymous Role.");
                }
                else
                {
                    try
                    {
                        UserService.DeleteRole(this._activeRole);
                        Context.Response.Redirect("Roles.aspx");
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }
                }
            }
        }

    }
}

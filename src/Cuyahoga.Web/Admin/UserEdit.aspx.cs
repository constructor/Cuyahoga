using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;

namespace Cuyahoga.Web.Admin
{
    public partial class UserEdit : AdminBasePage
    {
        private User _activeUser;

        public UserEdit() 
        {
            _activeUser = HttpContext.Current.User as User;
        }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Title = "Edit user";

			if (Context.Request.QueryString["UserId"] != null)
            {
                int userid = Int32.Parse(Context.Request.QueryString["UserId"]);

                if (userid == -1)
				{
					// Create a new user
					this._activeUser = new User();
				}
				else
				{
					// Get existing user data
                    this._activeUser = base.UserService.GetUserById(userid);
				}

				if (! this.IsPostBack)
				{
					BindTimeZones();
					BindUserControls();
					BindRoles();
                    BindWebsites(); 
				}
			}	
		}

        protected void BindTimeZones()
		{
			this.ddlTimeZone.DataSource = TimeZoneUtil.GetTimeZones();
			this.ddlTimeZone.DataValueField = "Key";
			this.ddlTimeZone.DataTextField = "Value";
			this.ddlTimeZone.DataBind();
		}

        protected void BindUserControls()
		{
			if (this._activeUser.Id > 0)
			{
				this.txtUsername.Visible = false;
				this.lblUsername.Text = this._activeUser.UserName;
				this.lblUsername.Visible = true;
				this.rfvUsername.Enabled = false;
			}
			else
			{
				this.txtUsername.Text = this._activeUser.UserName;
				this.txtUsername.Visible = true;
				this.lblUsername.Visible = false;
				this.rfvUsername.Enabled = true;
			}
			this.txtFirstname.Text = this._activeUser.FirstName;
			this.txtLastname.Text = this._activeUser.LastName;
			this.txtEmail.Text = this._activeUser.Email;
			this.txtWebsite.Text = this._activeUser.Website;

			this.ddlTimeZone.Items.FindByValue(this._activeUser.TimeZone.ToString()).Selected = true;
			this.chkActive.Checked = this._activeUser.IsActive;
			this.btnDelete.Visible = (this._activeUser.Id > 0);
			this.btnDelete.Attributes.Add("onclick", "return confirmDeleteUser();");
		}

        protected void BindWebsites()
        {
            this.cblUserSitesList.DataSource = base.SiteService.GetAllSites();
            this.cblUserSitesList.DataBind();

            foreach (ListItem s in cblUserSitesList.Items)
            {
                Site site = base.SiteService.GetSiteById(int.Parse(s.Value));

                if (_activeUser.Sites != null && _activeUser.Sites.Contains(site))
                {
                    s.Selected = true;
                }
                else 
                {
                    s.Selected = false;
                }

            }
        }

        protected void BindRoles()
		{
            IList<Role> roles = base.UserService.GetAllRoles().Cast<Role>().ToList();
			this.rptRoles.ItemDataBound += new RepeaterItemEventHandler(RptRolesItemDataBound);
            this.rptRoles.DataSource = roles.Where(x => x.Name != "Anonymous User");
			this.rptRoles.DataBind();
		}

        protected void SetRoles()
		{
			this._activeUser.Roles.Clear();

			foreach (RepeaterItem ri in rptRoles.Items)
			{	
				// HACK: RoleId is stored in the ViewState because the repeater doesn't have a DataKeys property.
				// Another HACK: we're only using the role id's to save database roundtrips.
				CheckBox chkRole = (CheckBox)ri.FindControl("chkRole");
				if (chkRole.Checked)
				{
					int roleId = (int)this.ViewState[ri.ClientID];
                    Role role = UserService.GetRoleById(roleId);
					this._activeUser.Roles.Add(role);
				}
			}

			// Check if the Adminstrator role is not accidently deleted for the superuser.
			string adminRole = Config.GetConfiguration()["AdministratorRole"];
			if (this._activeUser.UserName == Config.GetConfiguration()["SuperUser"]
				&& ! this._activeUser.IsInRole(adminRole))
			{
				throw new Exception(String.Format("The user '{0}' has to have the '{1}' role."
					, this._activeUser.UserName, adminRole));
			}
		}

        protected void SaveUser()
		{
			try
			{
				SetRoles();
				if (this._activeUser.Id == -1)
				{
                    UserService.CreateUser(this._activeUser);
					Context.Response.Redirect("UserEdit.aspx?UserId=" + this._activeUser.Id + "&message=User created successfully");
				}
				else
				{
                    UserService.UpdateUser(this._activeUser);
					ShowMessage("User saved");
				}
			}
			catch (Exception ex)
			{
				ShowError(ex.Message);
			}				
		}

        protected void BtnCancelClick(object sender, System.EventArgs e)
		{
			Context.Response.Redirect("Users.aspx");
		}

        protected void RptRolesItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			Role role = e.Item.DataItem as Role;
			if (role != null)
			{
				CheckBox chkRole = (CheckBox)e.Item.FindControl("chkRole");
				chkRole.Checked = this._activeUser.IsInRole(role);
				// Add RoleId to the ViewState with the ClientID of the repeateritem as key.
				this.ViewState[e.Item.ClientID] = role.Id;
			}
		}

        protected void BtnSaveClick(object sender, System.EventArgs e)
		{
			if (this.IsValid)
			{
				if (this._activeUser.Id == -1)
				{
					_activeUser.UserName = txtUsername.Text;
				}
				else
				{
					_activeUser.UserName = lblUsername.Text;
				}
				if (txtFirstname.Text.Length > 0)
				{
					_activeUser.FirstName = txtFirstname.Text;
				}
				if (txtLastname.Text.Length > 0)
				{
					_activeUser.LastName = txtLastname.Text;
				}
				_activeUser.Email = txtEmail.Text;

				_activeUser.Website = txtWebsite.Text;

                ////Add Site to user is selected
                //if(int.Parse(this.ddlWebsite.SelectedValue) > 0)
                //this._activeUser.Site = this._siteService.GetSiteById(int.Parse(this.ddlWebsite.SelectedValue));

                if (_activeUser.Id > 0)
                {
                    foreach (ListItem s in cblUserSitesList.Items)
                    {
                        Site site = base.SiteService.GetSiteById(int.Parse(s.Value));
                        if (s.Selected)
                        {
                            if (_activeUser.Sites != null && !_activeUser.Sites.Contains(site))
                            {
                                _activeUser.Sites.Add(site);
                            }
                        }
                        else
                        {
                            if (_activeUser.Sites != null && _activeUser.Sites.Contains(site))
                            {
                                _activeUser.Sites.Remove(site);
                            }
                        }
                    }
                }
                else 
                {
                    foreach (ListItem s in cblUserSitesList.Items)
                    {
                        Site site = base.SiteService.GetSiteById(int.Parse(s.Value));
                        if (s.Selected)
                        {
                            _activeUser.Sites.Add(site);
                        }
                    }
                }

				_activeUser.IsActive = this.chkActive.Checked;
				_activeUser.TimeZone = Int32.Parse(this.ddlTimeZone.SelectedValue);

				if (this.txtPassword1.Text.Length > 0 && this.txtPassword2.Text.Length > 0)
				{
					try
					{
						_activeUser.Password = Core.Domain.User.HashPassword(this.txtPassword1.Text);
					}
					catch (Exception ex)
					{
						ShowError(ex.Message);
					}
				}

				if (this._activeUser.Id == -1 && this._activeUser.Password == null)
				{
					ShowError("Password is required");
				}
				else
				{
					SaveUser();
				}
			}
		}

		protected void BtnDeleteClick(object sender, System.EventArgs e)
		{
			if (this._activeUser.Id > 0)
			{
				if (this._activeUser.Id != ((Cuyahoga.Core.Domain.User)this.Page.User.Identity).Id)
				{
					if (this._activeUser.UserName != Config.GetConfiguration()["SuperUser"])
					{
						try
						{
                            UserService.DeleteUser(this._activeUser);
							Context.Response.Redirect("Users.aspx");
						}
						catch (Exception ex)
						{
							ShowError(ex.Message);
						}
					}
					else
					{
						ShowError("You can't delete the superuser.");
					}
				}
				else
				{
					ShowError("You can't delete yourself.");
				}
			}
		}

        protected void BtnBackClick(object sender, EventArgs e)
        {
            Context.Response.Redirect("Users.aspx");
        }

    }
}

using System;

using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.User
{

	/// <summary>
	///		Summary description for Profile.
	/// </summary>
	public partial class ViewProfile : BaseModuleControl
	{
		private ProfileModule _module;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			this._module = base.Module as ProfileModule;
			if (this._module.CurrentUserId > 0)
			{
				Cuyahoga.Core.Domain.User user = this._module.GetUserById(this._module.CurrentUserId);
				if (user != null)
				{
					this.litTitle.Text = String.Format(GetText("VIEWPROFILETITLE"), user.UserName);
					BindUser(user);
					if (!this.IsPostBack)
					{
						// Databind is required to bind the localized resources.
						this.DataBind();
					}
				}
				else
				{
					this.lblError.Text = String.Format(GetText("USERNOTFOUND"), this._module.CurrentUserId.ToString());
					this.lblError.Visible = true;
				}
			}
		}

		private void BindUser(Cuyahoga.Core.Domain.User user)
		{
			this.lblUsername.Text = user.UserName;
			this.lblFirstname.Text = user.FirstName;
			this.lblLastname.Text = user.LastName;
			if (!string.IsNullOrEmpty(user.Website))
			{
				this.hplWebsite.NavigateUrl = user.Website;
				this.hplWebsite.Text = user.Website;
			}
			this.lblRegisteredOn.Text = user.InsertTimestamp.ToShortDateString();
			if (user.LastLogin != null)
			{
				this.lblLastLogin.Text = user.LastLogin.ToString();
			}
		}

	}
}

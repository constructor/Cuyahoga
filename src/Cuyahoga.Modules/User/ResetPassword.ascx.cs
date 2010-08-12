using System;

using Cuyahoga.Core;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.User
{

	/// <summary>
	///		Summary description for ResetPassword.
	/// </summary>
	public partial class ResetPassword : BaseModuleControl
	{
		private ProfileModule _module;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this._module = base.Module as ProfileModule;

			if (! this.IsPostBack)
			{
				// Databind is required to bind the localized resources.
				this.DataBind();
			}
		}

		protected void BtnReset_Click(object sender, System.EventArgs e)
		{
			if (this.Page.IsValid)
			{
				try
				{
					this._module.ResetPassword(this.txtUsername.Text, this.txtEmail.Text);
					this.pnlReset.Visible = false;
					this.pnlConfirmation.Visible = true;
					this.lblConfirmation.Text = String.Format(GetText("RESETCONFIRMATION"), this.txtEmail.Text);
				}
				catch (EmailException)
				{
					this.lblError.Text = GetText("RESETEMAILERROR");
					this.lblError.Visible = true;
				}
				catch (Exception ex)
				{
					this.lblError.Text = ex.Message;
					this.lblError.Visible = true;
				}
			}
		}
	}
}

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Cuyahoga.Web.UI;

namespace Cuyahoga.Web
{
    public partial class Profile : GeneralPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void InitPage()
        {
            // Load the right user control based on the pathinfo.
            string pathInfo = HttpContext.Current.Request.PathInfo;
            string[] parameters = Util.UrlHelper.GetParamsFromPathInfo(pathInfo);
            if (parameters != null)
            {
                Control ctrl = null;
                string cmd = parameters[0];
                switch (cmd)
                {
                    case "view":
                        ctrl = this.LoadControl("~/Controls/ViewProfile.ascx");
                        base.Title = "View profile";
                        if (parameters.Length == 2)
                        {
                            // The second pathinfo parameter should be the UserId.
                            int userId = Int32.Parse(parameters[1]);
                            (ctrl as Cuyahoga.Web.Controls.ViewProfile).UserId = userId;
                        }
                        break;
                    case "edit":
                        ctrl = this.LoadControl("~/Controls/EditProfile.ascx");
                        base.Title = "Edit profile";
                        break;
                    case "register":
                        ctrl = this.LoadControl("~/Controls/Register.ascx");
                        base.Title = "Register";
                        break;
                    case "reset":
                        ctrl = this.LoadControl("~/Controls/ResetPassword.ascx");
                        base.Title = "Reset password";
                        break;
                    default:
                        throw new Exception("Invalid command found in pathinfo.");
                }

                this.pnlControl.Controls.Add(ctrl);
            }
        }
    }
}
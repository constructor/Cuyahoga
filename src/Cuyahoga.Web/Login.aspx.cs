using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using Cuyahoga.Web.HttpModules;
using Cuyahoga.Core;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Util;
using Cuyahoga.Core.Domain;
using log4net;

namespace Cuyahoga.Web.Admin
{
    public partial class Login2 : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(User));
        private IAuthenticationService _authenticationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._authenticationService = IoC.Resolve<IAuthenticationService>();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (AuthenticateUser(txtUsername.Text, txtPassword.Text, false))
            {
                AuthenticateUser(txtUsername.Text, txtPassword.Text, true);
                Context.Response.Redirect(FormsAuthentication.GetRedirectUrl(this.User.Identity.Name, false));
            }
            else
            {
                this.lblError.Text = "Invalid username or password.";
                this.lblError.Visible = true;
            }	
        }

        private bool AuthenticateUser(string username, string password, bool persistLogin)
        {
            try
            {
                User user =
                    this._authenticationService.AuthenticateUser(username, password, HttpContext.Current.Request.UserHostAddress);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        log.Warn(String.Format("Inactive user {0} tried to login.", user.UserName));
                        throw new AccessForbiddenException("The account is disabled.");
                    }
                    // Create the authentication ticket
                    HttpContext.Current.User = user;
                    FormsAuthentication.SetAuthCookie(user.Name, persistLogin);

                    return true;
                }
                else
                {
                    log.Warn(String.Format("Invalid username-password combination: {0}:{1}.", username, password));
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format("An error occured while logging in user from Login.aspx {0}.", username));
                lblError.Text = String.Format("Unable to log in user '{0}': " + ex.Message, username);
                return false;
            }
        }
    }
}

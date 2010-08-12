using System;
using System.Web;
using System.Web.Security;

using Cuyahoga.Web.Components;
using Cuyahoga.Core;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;

namespace Cuyahoga.Web.HttpModules
{
	/// <summary>
	/// HttpModule to extend Forms Authentication.
	/// </summary>
	public class AuthenticationModule : IHttpModule
	{
		private IUserService _userService;
        private IAuthenticationService authenticationService;

		public void Init(HttpApplication context)
		{
			this._userService = IoC.Resolve<IUserService>();
            this.authenticationService = IoC.Resolve<IAuthenticationService>();
			context.AuthenticateRequest += new EventHandler(Context_AuthenticateRequest);
		}

		public void Dispose()
		{
			// Nothing here	
		}

		private void Context_AuthenticateRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (app.Context.User != null && app.Context.User.Identity.IsAuthenticated)
			{
				// There is a logged-in user with a standard Forms Identity. Replace it with
				// Cuyahoga identity (the User class implements IIdentity). 				
				int userId = Int32.Parse(app.Context.User.Identity.Name);
				User cuyahogaUser = _userService.GetUserById(userId);
				cuyahogaUser.IsAuthenticated = true;
				CuyahogaContext.Current.SetUser(cuyahogaUser);
			}
		}

        /*For classic Cuyahoga*/
        /// <summary>
        /// Try to authenticate the user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="persistLogin"></param>
        /// <returns></returns>
        public bool AuthenticateUser(string username, string password, bool persistLogin)
        {
            try
            {
                User user = this.authenticationService.AuthenticateUser(username, password, HttpContext.Current.Request.UserHostAddress);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        throw new AccessForbiddenException("This user account is disabled.");
                    }
                    // Create the authentication ticket
                    HttpContext.Current.User = user;
                    FormsAuthentication.SetAuthCookie(user.Name, persistLogin);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch 
            {
                //throw new Exception(String.Format("Unable to log in user '{0}': " + ex.Message, username), ex);
                return false;
            }
        }
	}
}

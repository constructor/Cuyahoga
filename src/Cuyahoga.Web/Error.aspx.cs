using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using log4net;

using Cuyahoga.Core;
using Cuyahoga.Core.Util;

namespace Cuyahoga.Web
{
    public partial class Error : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Error));

        private void Page_Load(object sender, System.EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                log.Error("An unexpected error occured", ex);
                Exception innerException = ex.InnerException;
                if (innerException != null)
                {
                    if (innerException is SiteNullException)
                    {
                        HttpContext.Current.Response.StatusCode = 503;
                        this.lblTitle.Text = innerException.Message;
                        this.lblError.Text = "The url you entered is invalid or the site is down for maintenance.";
                    }
                    else if (innerException is NodeNullException || innerException is SectionNullException)
                    {
                        HttpContext.Current.Response.StatusCode = 404;
                        this.lblTitle.Text = "404 Page not found";
                        this.lblError.Text = "The requested page could not be found.";
                    }
                    else if (innerException is AccessForbiddenException)
                    {
                        bool redirectToLoginPage = Boolean.Parse(Config.GetConfiguration()["RedirectToLoginWhenAccessDenied"]);
                        if (redirectToLoginPage)
                        {
                            string returnUrl = "~/Login.aspx?ReturnUrl="
                                + HttpContext.Current.Server.UrlEncode(HttpContext.Current.Items["VirtualUrl"].ToString());
                            HttpContext.Current.Response.Redirect(returnUrl, true);
                        }
                        else
                        {
                            HttpContext.Current.Response.StatusCode = 403;
                            this.lblTitle.Text = "403 Access forbidden";
                            this.lblError.Text = "Access to the requested resource is forbidden.";
                        }
                    }
                    else
                    {
                        HttpContext.Current.Response.StatusCode = 500;
                        this.lblTitle.Text = "500 An error occured:";
                        this.lblError.Text = innerException.Message;
                    }
                }
                else
                {
                    HttpContext.Current.Response.StatusCode = 500;
                    this.lblTitle.Text = "500 An error occured:";
                    this.lblError.Text = ex.Message;
                }
            }
            else
            {
                this.lblTitle.Text = "Something strange happened...";
            }
        }

    }

}
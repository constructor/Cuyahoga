using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin.Controls
{
    public partial class NavigationBar : System.Web.UI.UserControl
    {

        private User _currentUser;

        protected User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        public NavigationBar()
        {
            CurrentUser = Context.User.Identity as User;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && Request.QueryString["SiteId"] != null && Request.QueryString["SiteId"] != "-1")
            {
                hplTemplates.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplUsers.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplModules.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplRoles.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplSections.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplCategories.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
                hplRebuild.NavigateUrl += "?SiteId=" + Request.QueryString["SiteId"].ToString();
            }
            else
            {
                hplSections.Visible = false;
                hplRoles.Visible = false;
                hplCategories.Visible = false;
                hplRebuild.Visible = false;
            }

            //// TODO: Alternative fine grained rights for admin
            //if (CurrentUser.HasRight("Access Admin"))//If user is admin of a specific site (not admin of all sites)
            //{
            //    plhAdminOnlyOptions.Visible = true;
            //    plhAdminOnlyOptionAddSite.Visible = CurrentUser.HasRight("Manage Site"); //false;
            //    pnlNewSite.Visible = CurrentUser.HasRight("Manage Site"); //false;

            //    hplSections.Enabled = CurrentUser.HasRight("Manage Sections");
            //    hplModules.Enabled = CurrentUser.HasRight("Manage Modules");
            //    hplTemplates.Enabled = CurrentUser.HasRight("Manage Templates");
            //    hplUsers.Enabled = CurrentUser.HasRight("Manage Users");
            //    hplRoles.Enabled = CurrentUser.HasRight("Global Permissions");
            //    hplNew.Enabled = CurrentUser.HasRight("Manage Site");
            //}

            if (CurrentUser.IsInRole("Site Administrator"))//If user is admin of a specific site (not admin of all sites)
            {
                plhAdminOnlyOptions.Visible = false;
            }
        }

    }
}
using System;
using System.Collections;
using System.Web.UI.WebControls;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;

namespace Cuyahoga.Web.Admin
{
    public partial class Users : AdminBasePage
    {

        protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Title = "Users";
			this.pgrUsers.CacheVaryByParams = new string[1] {this.txtUsername.UniqueID};

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["SiteId"] != null)
                {
                    int siteId = Convert.ToInt32(Request.QueryString["SiteId"]);
                    BindSiteUsers(siteId);
                    BindWebsites();
                    ddlWebsite.SelectedValue = Request.QueryString["SiteId"].ToString();
                }
                else 
                {
                    BindUsers();
                    BindWebsites();
                }
            }
		}

        protected void BindUsers()
		{
			GetUserData();
			this.rptUsers.DataBind();
		}

        protected void BindWebsites()
        {
            IList websites = SiteService.GetAllSites();
            this.ddlWebsite.DataSource = websites;
            this.ddlWebsite.DataBind();
        }

        protected void BindSiteUsers(Int32 siteId)
        {
            this.rptUsers.DataSource = base.UserService.GetUsersBySiteID(siteId);
            this.rptUsers.DataBind();
        }

        protected void GetUserData()
		{
			this.rptUsers.DataSource = base.CoreRepository.FindUsersByUsername(this.txtUsername.Text);
		}

        protected void BtnFindClick(object sender, System.EventArgs e)
		{
			BindUsers();	
		}

        protected void PgrUsersPageChanged(object sender, Cuyahoga.ServerControls.PageChangedEventArgs e)
		{
			//this.pgrUsers.CurrentPageIndex = e.CurrentPage;
			//BindUsers();
			this.rptUsers.DataBind();
		}

        protected void PgrUsersCacheEmpty(object sender, System.EventArgs e)
		{
			// The CacheEmpty event is raised when the pager can't find any cached data so 
			// the data has to be retrieved again and set as DataSource of the control that is
			// being paged.
			GetUserData();
		}

        protected void RptUsersItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			User user = e.Item.DataItem as User;

			if (user != null)
			{
				Label lblLastLogin = e.Item.FindControl("lblLastLogin") as Label;
				if (user.LastLogin != null)
				{
					lblLastLogin.Text = TimeZoneUtil.AdjustDateToUserTimeZone(user.LastLogin.Value, this.Page.User.Identity).ToString();
				}

				HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");

				// HACK: as long as ~/ doesn't work properly in mono we have to use a relative path from the Controls
				// directory due to the template construction.
				hplEdit.NavigateUrl = String.Format("~/Admin/UserEdit.aspx?UserId={0}", user.Id);
			}
		}

        protected void BtnNewClick(object sender, System.EventArgs e)
        {
			Context.Response.Redirect("UserEdit.aspx?UserId=-1&siteId=" + ddlWebsite.SelectedValue.ToString());
		}

        protected void DdlWebsiteSelectedIndexChanged(object sender, EventArgs e)
        {
            BindSiteUsers(Convert.ToInt32(ddlWebsite.SelectedValue));
        }
    }
}

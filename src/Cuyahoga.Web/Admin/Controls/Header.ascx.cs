using System;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Web.Admin.Controls
{
    public partial class Header : System.Web.UI.UserControl
    {
        private AdminBasePage _page;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this._page = (AdminBasePage)this.Page;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("This control requires a Page of the type AdminBasePage.", ex);
            }

            if (Context.Request.QueryString["NodeId"] != null && int.Parse(Context.Request.QueryString["NodeId"]) != -1)
            {
                int nodeId = int.Parse(Context.Request.QueryString["NodeId"]);
                _page.ActiveNode = _page.NodeService.GetNodeById(nodeId);
            }

            if (this._page.ActiveSection != null && this._page.ActiveSection.Node != null && this._page.ActiveSection.Id > 0)
            {
                this.hplSite.NavigateUrl = Util.UrlHelper.GetFullUrlFromSectionViaSite(this._page.ActiveSection);
            }
            else if (this._page.ActiveNode != null && this._page.ActiveNode.Id > 0)
            {
                this.hplSite.NavigateUrl = Util.UrlHelper.GetFullUrlFromNodeViaSite(this._page.ActiveNode);
            }
            else if (this._page.ActiveSite != null && this._page.ActiveSite.Id > 0)
            {
                this.hplSite.NavigateUrl = this._page.ActiveSite.SiteUrl;
            }
            else
            {
                this.hplSite.Visible = false;
            }

            this.lbtLogout.Visible = this.Page.User.Identity.IsAuthenticated;
        }

        protected void lbtLogout_Click(object sender, System.EventArgs e)
        {
            FormsAuthentication.SignOut();
            Context.Response.Redirect("/");
        }
    }
}
using System;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Web.UI;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin.Controls
{
    public partial class AdminTemplate : BasePageControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PreRender += new EventHandler(AdminTemplate_PreRender);
        }

        protected void AdminTemplate_PreRender(object sender, EventArgs e)
        {
            if (this.Page is AdminBasePage)
                this.PageTitleLabel.Text = this.PageTitle.Text;
        }
    }
}
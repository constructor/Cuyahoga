using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.Downloads.Web
{
	/// <summary>
	/// Summary description for EditDownloads.
	/// </summary>
	public class EditDownloads : ModuleAdminBasePage
	{
		private DownloadsModule _downloadsModule;

		protected System.Web.UI.WebControls.Repeater rptFiles;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnNew;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// The base page has already created the module, we only have to cast it here to the right type.
			this._downloadsModule = base.Module as DownloadsModule;
			this.btnNew.Attributes.Add("onclick", String.Format("document.location.href='EditFile.aspx{0}&FileId=-1'", base.GetBaseQueryString()));

			if (! this.IsPostBack)
			{
				BindFiles();
			}
		}

		private void BindFiles()
		{
			this.rptFiles.DataSource = this._downloadsModule.GetAllFiles();
			this.rptFiles.DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.rptFiles.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.rptFiles_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void rptFiles_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
            FileResource file = e.Item.DataItem as FileResource;
			
			Literal litDateModified = e.Item.FindControl("litDateModified") as Literal;
			if (litDateModified != null)
			{
				litDateModified.Text = TimeZoneUtil.AdjustDateToUserTimeZone((DateTime)file.PublishedAt, this.User.Identity).ToString();
			}

			HyperLink hplEdit = e.Item.FindControl("hpledit") as HyperLink;
			if (hplEdit != null)
			{
				hplEdit.NavigateUrl = String.Format("~/Modules/Downloads/EditFile.aspx{0}&FileId={1}", base.GetBaseQueryString(), file.Id);
			}
		}
	}
}

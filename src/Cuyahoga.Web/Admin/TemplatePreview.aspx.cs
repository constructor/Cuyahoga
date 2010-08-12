using System;
using System.Web;

using Cuyahoga.Core.Service;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Web.Admin
{
	/// <summary>
	/// Summary description for TemplatePreview.
	/// </summary>
	public class TemplatePreview : System.Web.UI.Page
	{
		private void InitTemplate()
		{
			if (Context.Request.QueryString["TemplateId"] != null)
			{
				int templateId = Int32.Parse(Context.Request.QueryString["TemplateId"]);
                ITemplateService templateService = Cuyahoga.Core.Util.IoC.Resolve<ITemplateService>();
                Template template = templateService.GetTemplateById(templateId);
				BaseTemplate templateControl = (BaseTemplate)this.LoadControl(UrlHelper.GetApplicationPath() + template.Path);
				string css = UrlHelper.GetApplicationPath() + template.BasePath + "/Css/" + template.Css;
				templateControl.RenderCssLinks(new string[1] {css});
				templateControl.InsertContainerButtons();
				this.Controls.Add(templateControl);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			InitTemplate();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
	}
}

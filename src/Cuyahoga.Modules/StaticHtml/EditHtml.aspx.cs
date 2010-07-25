using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Web.Util;
using Cuyahoga.Web.UI;
using Cuyahoga.Modules.StaticHtml;
using Cuyahoga.Core.Service.Content;

namespace Cuyahoga.Modules.StaticHtml
{
	/// <summary>
	/// Summary description for EditHtml.
	/// </summary>
	public partial class EditHtml : ModuleAdminBasePage
	{
		private StaticHtmlModule _module;

        protected void Page_Load(object sender, System.EventArgs e)
		{
            this.fckEditor.BasePath = this.Page.ResolveUrl("~/Support/FCKeditor/");
			this._module = base.Module as StaticHtmlModule;

			if (! this.IsPostBack)
			{
                StaticHtmlContent htmlContent = this._module.ContentItemService.FindContentItemsBySection(this._module.Section).FirstOrDefault() 
                                                ?? new StaticHtmlContent();

                if (htmlContent != null)
				{
                    tbTitle.Text = htmlContent.Title;
                    this.fckEditor.Value = htmlContent.Content;
				}
				else
				{
					this.fckEditor.Value = String.Empty;
				}
			}

            //To DISPLAY template styles in editor
            fckEditor.EditorAreaCSS = UrlHelper.GetApplicationPath() + this.Node.Template.BasePath + "/css/editor_" + this.Node.Template.Css;
            //To ADD template styles in editor DropDown List
            fckEditor.StylesXmlPath = UrlHelper.GetApplicationPath() + this.Node.Template.BasePath + "/css/fckstyles.xml";
        }

		private void SaveStaticHtml()
		{
			Cuyahoga.Core.Domain.User currentUser = (Cuyahoga.Core.Domain.User)Context.User.Identity;

            StaticHtmlContent htmlContent = this._module.ContentItemService.FindContentItemsBySection(this._module.Section).FirstOrDefault()
                                            ?? new StaticHtmlContent();

            if (htmlContent == null)
			{
				// New
                htmlContent = new StaticHtmlContent();
                htmlContent.Title = tbTitle.Text;
                htmlContent.Section = this._module.Section;
                htmlContent.CreatedBy = currentUser;
                htmlContent.ModifiedBy = currentUser;
			}
			else
			{
				// Exisiting
                htmlContent.Title = tbTitle.Text;
                htmlContent.ModifiedBy = currentUser;
                htmlContent.Section = this._module.Section;
			}

            htmlContent.Content = this.fckEditor.Value;
            this._module.SaveContent(htmlContent);	
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

		}
		#endregion

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				SaveStaticHtml();
				ShowMessage("Content saved.");
                Response.Redirect(UrlHelper.GetFullUrlFromSection(this.Section));
			}
			catch (Exception ex)
			{
				ShowException(ex);
			}
		}
	

	}
}

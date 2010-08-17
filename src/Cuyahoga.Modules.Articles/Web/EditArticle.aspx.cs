using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Util;
using Cuyahoga.Modules.Articles;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;
using Cuyahoga.Modules.Articles.Domain;

//using ArticleCategory = Cuyahoga.Modules.Articles.Domain.Category;

namespace Cuyahoga.Modules.Articles.Web
{

	/// <summary>
	/// Summary description for EditArticle.
	/// </summary>
	public partial class EditArticle : ModuleAdminBasePage
	{
		private Article _article;
		private ArticleModule _articleModule;

		protected System.Web.UI.WebControls.RequiredFieldValidator rfvDateOnline;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvDateOffline;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.fckContent.BasePath = this.Page.ResolveUrl("~/Support/FCKeditor/");

            //Add FCK Specific CSS and template references to the editor
            //RegisterStylesheet("CatModCSS", UrlHelper.GetApplicationPath() + "Modules/Articles/EditorCSS/style.css");

            //To DISPLAY template styles in editor
            fckContent.EditorAreaCSS = UrlHelper.GetApplicationPath() + this.Node.Template.BasePath + "/css/editor_" + this.Node.Template.Css;
            //To ADD template styles in editor DropDown List
            fckContent.StylesXmlPath = UrlHelper.GetApplicationPath() + this.Node.Template.BasePath + "/css/fckstyles.xml";

			this._articleModule = base.Module as ArticleModule;
			this.btnCancel.Attributes.Add("onclick", String.Format("document.location.href='AdminArticles.aspx{0}'", base.GetBaseQueryString()));

			if (! this.IsPostBack)
			{
				BindCategories();
			}

			if (Request.QueryString["ArticleId"] != null)
			{
				int articleId = Int32.Parse(Request.QueryString["ArticleId"]);
				if (articleId > 0)
				{
					this._article = this._articleModule.GetArticleById(articleId);
					if (! this.IsPostBack)
					{
						BindArticle();
					}
					this.btnDelete.Visible = true;
					this.btnDelete.Attributes.Add("onclick", "return confirm('Are you sure?');");
				}
			}

		}

		private void BindCategories()
		{
			this.ddlCategory.Items.Add(new ListItem("none", "0"));
			//IList categories = this._articleModule.GetAvailableCategories();
            IEnumerable<Category> categories = this._articleModule._categoryService.GetAllCategories(this._articleModule.Section.Site);

            foreach (Category c in categories.OrderBy(o => o.Path))
            {
                string indent = string.Empty;
                for (int i = 0; i < c.Level; i++)
                {
                    indent += ">";
                }
                if (c.Level < 1)
                {
                    this.ddlCategory.Items.Add(new ListItem(c.Name, c.Id.ToString()));
                }
                else 
                {
                    this.ddlCategory.Items.Add(new ListItem(indent + " " + c.Name, c.Id.ToString()));
                }
			}
		}

		private void BindArticle()
		{
			this.txtTitle.Text = this._article.Title;
			this.txtSummary.Text = this._article.Summary;
			this.fckContent.Value = this._article.Content;
			this.chkSyndicate.Checked = this._article.Syndicate;
			this.calDateOnline.SelectedDate = TimeZoneUtil.AdjustDateToUserTimeZone((DateTime)this._article.PublishedAt, this.User.Identity);
            this.calDateOffline.SelectedDate = TimeZoneUtil.AdjustDateToUserTimeZone((DateTime)this._article.PublishedUntil, this.User.Identity);
			if (this._article.Categories.Count > 0)
			{
                //This is rubbish:{Categories[0]} :) Will completely rework later
				ListItem li = this.ddlCategory.Items.FindByValue(this._article.Categories[0].Id.ToString());
				if (li != null)
				{
					li.Selected = true;
				}
			}
		}

		private void SaveArticle()
		{
			try
			{
				this._article.Title = this.txtTitle.Text;
                this._article.Section = this._articleModule.Section;

				if (this.txtSummary.Text.Length > 0)
				{
					this._article.Summary = this.txtSummary.Text;
				}
				else
				{
					this._article.Summary = null;
				}

				this._article.Content = this.fckContent.Value;

				if (this.txtCategory.Text.Length > 0)
				{
                    if (this.ddlCategory.SelectedIndex > 0)
                    {                        
                        //New subcategory
                        Category newCategory = new Category();
                        int parentid = int.Parse(ddlCategory.SelectedValue);

                        Category parentCat = _articleModule._categoryService.GetById(parentid);
                        newCategory.ParentCategory = parentCat;
                        newCategory.Path = string.Concat(parentCat.Path, _articleModule._categoryService.GetPathFragmentFromPosition(parentCat.ChildCategories.Count + 1));
                        newCategory.Position = _articleModule._categoryService.GetPositionFromPath(newCategory.Path);

                        newCategory.Name = this.txtCategory.Text;
                        newCategory.Site = this._articleModule.Section.Site;
                        this._articleModule._categoryService.CreateCategory(newCategory);

                        //If not multiple categories then clear collection first
                        //TODO: Make multiple categories article appearance
                        this._article.Categories.Clear();
                        this._article.Categories.Add(newCategory);
                    }
                    else 
                    {
                        //New root category
                        Category newCategory = new Category();
                        newCategory.ParentCategory = null;
                        IList<Category> rootCats = this._articleModule._categoryService.GetAllRootCategories(_articleModule.Section.Site);
                        newCategory.Path = this._articleModule._categoryService.GetPathFragmentFromPosition(rootCats.Count + 1);
                        newCategory.Position = _articleModule._categoryService.GetPositionFromPath(newCategory.Path);

                        newCategory.Name = this.txtCategory.Text;
                        newCategory.Site = this._articleModule.Section.Site;
                        this._articleModule._categoryService.CreateCategory(newCategory);

                        //If not multiple categories then clear collection first
                        //TODO: Make multiple categories article appearance
                        this._article.Categories.Clear();
                        this._article.Categories.Add(newCategory);
                    }
				}
                else if (this.ddlCategory.SelectedIndex > 0)
                {
                    //If not multiple categories then clear collection first
                    //TODO: Make multiple categories article appearance
                    this._article.Categories.Clear();
                    this._article.Categories.Add(this._articleModule.GetCategoryById(Int32.Parse(this.ddlCategory.SelectedValue)));
                }
                else 
                {
                    //None seleted so clear all
                    this._article.Categories.Clear();
                }

				this._article.Syndicate = this.chkSyndicate.Checked;
				if (this.calDateOnline.SelectedDate != DateTime.MinValue)
				{
                    this._article.PublishedAt = TimeZoneUtil.AdjustDateToServerTimeZone(this.calDateOnline.SelectedDate, this.User.Identity);
				}
				else
				{
					this._article.PublishedAt = DateTime.Now;
				}

				if (this.calDateOffline.SelectedDate != DateTime.MinValue)
				{
					this._article.PublishedUntil = TimeZoneUtil.AdjustDateToServerTimeZone(this.calDateOffline.SelectedDate, this.User.Identity);
				}
				else
				{
					this._article.PublishedUntil = DateTime.Now.AddYears(100);
				}
				this._article.ModifiedBy = (Cuyahoga.Core.Domain.User)this.User.Identity;
				this._articleModule._contentItemService.Save(this._article);

				Response.Redirect(String.Format("AdminArticles.aspx{0}", base.GetBaseQueryString()));
			}
			catch (Exception ex)
			{
				ShowException(ex);
			}
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
			if (this.IsValid)
			{
				if (this._article == null)
				{
					this._article = new Article();
					this._article.Section = this._articleModule.Section;
					this._article.CreatedBy = (Cuyahoga.Core.Domain.User)this.User.Identity;
				}
				SaveArticle();
			}
		}

		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			if (this._article != null)
			{
				try
				{
					this._articleModule._contentItemService.Delete(this._article);
					Response.Redirect(String.Format("AdminArticles.aspx{0}", base.GetBaseQueryString()));
				}
				catch (Exception ex)
				{
					ShowError(ex.Message);
				}
			}
			else
			{
				ShowError("No article found to delete");
			}
		}

	}
}

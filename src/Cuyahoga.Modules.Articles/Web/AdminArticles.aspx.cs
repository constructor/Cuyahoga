using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Cuyahoga.Core.Util;
using Cuyahoga.Core.Domain;
using Cuyahoga.Modules.Articles;
using Cuyahoga.Modules.Articles.Domain;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.Articles.Web
{
    public partial class AdminArticles : ModuleAdminBasePage
    {
        private ArticleModule _articleModule;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //The base page has already created the module, we only have to cast it here to the right type.
            this._articleModule = base.Module as ArticleModule;
            this.btnNew.Attributes.Add("onclick", String.Format("document.location.href='EditArticle.aspx{0}&ArticleId=-1'", base.GetBaseQueryString()));

            if (!this.IsPostBack)
            {
                //this.rptArticles.DataSource = this._articleModule.GetArticleList();
                //this.rptArticles.DataSource = this._articleModule._GetFullArticleList();
                this.rptArticles.DataSource = this._articleModule._contentItemService.FindContentItemsBySite(this._articleModule.Section.Site);
                this.rptArticles.DataBind();

                this.ddlArticleFilter.DataSource = this._articleModule._categoryService.GetAllCategories(this._articleModule.Section.Site);
                this.ddlArticleFilter.DataBind();
            }
        }

        protected string GetArtacleCatagories(string articleid)
        {
            string catagories = String.Empty;
            Article a = _articleModule.GetArticleById(int.Parse(articleid));
            for (int i = 0; i < a.Categories.Count; i++)
            {
                catagories += a.Categories[i].Name + ((i != a.Categories.Count - 1) ? " / " : ".");
            }
            return catagories;
        }


        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (ddlArticleFilter.SelectedValue != "All")
            {
                Category c = _articleModule._categoryService.GetById(int.Parse(ddlArticleFilter.SelectedValue));

                Cuyahoga.Core.Service.Content.ContentItemSortBy sortBy = (Cuyahoga.Core.Service.Content.ContentItemSortBy)Enum.Parse(typeof(Cuyahoga.Core.Service.Content.ContentItemSortBy), ddlSortBy.SelectedValue, true);
                Cuyahoga.Core.Service.Content.ContentItemSortDirection sortDirection = (Cuyahoga.Core.Service.Content.ContentItemSortDirection)Enum.Parse(typeof(Cuyahoga.Core.Service.Content.ContentItemSortDirection), ddlSortDirection.SelectedValue, true);

                this.rptArticles.DataSource = this._articleModule._contentItemService.FindVisibleContentItemsByCategory(c,
                    new Cuyahoga.Core.Service.Content.ContentItemQuerySettings(sortBy, sortDirection));

                this.rptArticles.DataBind();
            }
            else if(ddlArticleFilter.SelectedValue == "All")
            {
                Section s = _articleModule.Section;

                Cuyahoga.Core.Service.Content.ContentItemSortBy sortBy = (Cuyahoga.Core.Service.Content.ContentItemSortBy)Enum.Parse(typeof(Cuyahoga.Core.Service.Content.ContentItemSortBy), ddlSortBy.SelectedValue, true);
                Cuyahoga.Core.Service.Content.ContentItemSortDirection sortDirection = (Cuyahoga.Core.Service.Content.ContentItemSortDirection)Enum.Parse(typeof(Cuyahoga.Core.Service.Content.ContentItemSortDirection), ddlSortDirection.SelectedValue, true);

                this.rptArticles.DataSource = this._articleModule._contentItemService.FindVisibleContentItemsBySection(s,
                    new Cuyahoga.Core.Service.Content.ContentItemQuerySettings(sortBy, sortDirection));

                this.rptArticles.DataBind();
            }
            else
            {
                this.rptArticles.DataSource = this._articleModule.GetFullArticleList();
                this.rptArticles.DataBind();
            }
        }

        protected void rptArticles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Article article = e.Item.DataItem as Article;

            Literal litDateOnline = e.Item.FindControl("litDateOnline") as Literal;
            if (litDateOnline != null)
            {
                litDateOnline.Text = TimeZoneUtil.AdjustDateToUserTimeZone((DateTime)article.PublishedAt, this.User.Identity).ToString();
            }
            Literal litDateOffline = e.Item.FindControl("litDateOffline") as Literal;
            if (litDateOffline != null)
            {
                litDateOffline.Text = TimeZoneUtil.AdjustDateToUserTimeZone((DateTime)article.PublishedUntil, this.User.Identity).ToString();
            }

            HyperLink hplEdit = e.Item.FindControl("hpledit") as HyperLink;
            if (hplEdit != null)
            {
                hplEdit.NavigateUrl = String.Format("~/Modules/Articles/EditArticle.aspx{0}&ArticleId={1}", base.GetBaseQueryString(), article.Id);
            }
            HyperLink hplComments = e.Item.FindControl("hplComments") as HyperLink;
            if (hplComments != null)
            {
                hplComments.NavigateUrl = String.Format("~/Modules/Articles/AdminComments.aspx{0}&ArticleId={1}", base.GetBaseQueryString(), article.Id);
            }
        }

        protected void pgrArticles_PageChanged(object sender, Cuyahoga.ServerControls.PageChangedEventArgs e)
        {
            this.rptArticles.DataBind();
        }

        protected void pgrArticles_CacheEmpty(object sender, EventArgs e)
        {
            this.rptArticles.DataSource = this._articleModule._contentItemService.FindContentItemsBySite(this._articleModule.Section.Site);
        }

    }
}

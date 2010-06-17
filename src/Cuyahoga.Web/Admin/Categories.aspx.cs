using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.Content;

namespace Cuyahoga.Web.Admin
{
    public partial class Categories : AdminBasePage
    {
        private ICategoryService categoryService;

        public Categories() 
        {
            this.categoryService = this.Container.Resolve<ICategoryService>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Title = "Categories";

                BindRootCategories();
                BindCategories();

                btnNewRoot.NavigateUrl = String.Format("~/Admin/CategoryEdit.aspx?cid=0&pcid=-1&SiteId={0}", this.ActiveSite.Id.ToString());
            }
        }

        protected void BindRootCategories()
        {
            IList<Category> rootCategories = this.categoryService.GetAllRootCategories(base.ActiveSite);

            this.rdioListRoot.DataSource = rootCategories;
            this.rdioListRoot.DataTextField = "Name";
            this.rdioListRoot.DataValueField = "Path";
            this.rdioListRoot.DataBind();

            if (rootCategories.Count == 0)
            {
                this.litNoRoot.Visible = false;
            }
            else
            {
                this.litNoRoot.Visible = true;
                this.rdioListRoot.SelectedIndex = 0;
            }
        }

        private void BindCategories()
        {
            //IList<Category> categories = this.categoryService.GetByPathStartsWith(this.rdioListRoot.SelectedValue);
            IList<Category> categories = this.categoryService.GetByPathByParent(base.ActiveSite, this.rdioListRoot.SelectedValue);
            this.rptCategories.DataSource = categories;
            this.rptCategories.DataBind();

            this.ddlMoveCategories.DataSource = categories;
            this.ddlMoveCategories.DataTextField = "Name";
            this.ddlMoveCategories.DataValueField = "Path";
            this.ddlMoveCategories.DataBind();
        }

        private string GetSelectedCategoryPath()
        {
            foreach (RepeaterItem itm in this.rptCategories.Items)
            {
                RadioButton rdio = itm.FindControl("rdioBtnCategory") as RadioButton;
                if (rdio != null)
                {
                    if (rdio.Checked)
                    {
                        return rdio.InputAttributes["path"];
                    }
                }
            }
            return null;
        }

        protected void RdioListRootSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindCategories();
        }

        protected void RptCategoriesItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            Category cat = e.Item.DataItem as Category;
            if (cat != null)
            {
                RadioButton rdio = (RadioButton)e.Item.FindControl("rdioBtnCategory");
                rdio.InputAttributes.Add("path", cat.Path);
                rdio.InputAttributes.Add("name", cat.Name);

                Label lbl = (Label)e.Item.FindControl("lblCategory");
                lbl.Style.Add("padding-left", string.Format("{0}px", (cat.Level * 12)));
                lbl.Text = "|_";

                HyperLink hplAddChild = (HyperLink)e.Item.FindControl("hplAddChild");
                hplAddChild.NavigateUrl = String.Format("~/Admin/CategoryEdit.aspx?cid=0&pcid={0}&SiteId={1}", cat.Id, this.ActiveSite.Id.ToString());

                HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");
                hplEdit.NavigateUrl = String.Format("~/Admin/CategoryEdit.aspx?cid={0}&pcid={1}&SiteId={2}", cat.Id, (cat.ParentCategory != null ? cat.ParentCategory.Id : -1), this.ActiveSite.Id.ToString());

                //FIX: ASP.NET radio button bug (SetUniqueRadioButton('[repeater id].*[rdio button group name]', this)
                string script =
                   "SetUniqueRadioButton('rptCategories.*Categories',this)";
                rdio.Attributes.Add("onclick", script);
            }
        }

        protected void BtnNewRootClick(object sender, EventArgs e)
        {
            this.CreateNewCategory(true);
        }

        private void CreateNewCategory(bool isRoot)
        {
            string newPath = string.Empty;
            if (isRoot)
            {
                IList<Category> rootCats = this.categoryService.GetAllRootCategories(this.ActiveSite);
                newPath = this.categoryService.GetPathFragmentFromPosition(rootCats.Count + 1);
                Context.Response.Redirect(string.Format("CategoryEdit.aspx?cid=0&pcid=-1&path={0}&SiteId={1}", newPath, this.ActiveSite.Id.ToString()));
            }
            else
            {
                Category cat = this.categoryService.GetByExactPath(this.GetSelectedCategoryPath());
                if (cat != null)
                {
                    IList<Category> childCats = cat.ChildCategories;
                    newPath = string.Concat(cat.Path, this.categoryService.GetPathFragmentFromPosition(childCats.Count + 1));
                    Context.Response.Redirect(string.Format("CategoryEdit.aspx?cid=0&pcid={0}&path={1}&SiteId={2}", cat.Id, newPath, this.ActiveSite.Id.ToString()));
                }
                else ShowMessage("Plase select a category first");
            }
        }

        protected void BtnDeleteClick(object sender, EventArgs e)
        {
            string path = this.GetSelectedCategoryPath();
            //get all child categories
            IList<Category> catList = this.categoryService.GetByPathStartsWithAndSite(base.ActiveSite, path);
            if (catList.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(catList.Count);
                foreach (Category cat in catList)
                {
                    this.categoryService.DeleteCategory(cat);
                    sb.Append(cat.Name);
                    sb.Append(", ");
                }
                this.ShowMessage(string.Format("{0} Categories deleted: {1}", catList.Count, sb.ToString().TrimEnd(',', ' ')));
            }
            else ShowMessage("Please select a category first");
        }

        protected void BtnMoveClick(object sender, EventArgs e)
        {
            string moveTo = this.ddlMoveCategories.SelectedValue;
            string moveFrom = this.GetSelectedCategoryPath();

            if (moveTo != null && moveFrom != null)
            {
                if (moveFrom.Length == 5) ShowError("Can not move root");
                else if (moveFrom.Length == 10 && moveTo.Length == 5) ShowError("Already exists in destination");
                else
                {
                    //this.categoryService.MoveCategoryToNewPath(moveFrom, moveTo);
                    this.categoryService.MoveCategoryToNewPathAdmin(base.ActiveSite, this.rdioListRoot.SelectedValue, moveFrom, moveTo);
                    //this.Response.Redirect("~/Admin/Categories.aspx?SiteId=" + this.ActiveSite.Id.ToString());
                    BindCategories();
                }
            }
            else ShowError("Could not move categories");
        }

    }
}
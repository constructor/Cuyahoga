using System;
using System.Collections.Generic;
using System.Linq;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.Content;

namespace Cuyahoga.Web.Admin
{
    public partial class CategoryEdit : AdminBasePage
    {
        private ICategoryService _categoryService;
        private Category _category;

        private int CategoryId
        {
            get { return (int)ViewState["cid"]; }
            set { ViewState["cid"] = value; }
        }
        private int ParentCategoryId
        {
            get { return (int)ViewState["pcid"]; }
            set { ViewState["pcid"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
               _categoryService = Container.Resolve<ICategoryService>();

               CategoryId = int.Parse(Request.QueryString["cid"]);
               ParentCategoryId = int.Parse(Request.QueryString["pcid"]);

               if (Request.QueryString["SiteId"] != null)
               {
                   ActiveSite = base.SiteService.GetSiteById(Int32.Parse(Context.Request.QueryString["SiteId"]));
               }

               if (!IsPostBack)
               {
                   //edit modes:
                   if (CategoryId < 1)
                   {
                       Title = "Add Category";
                       btnDelete.Visible = false;
                       AddNewCategory();
                   }
                   else
                   {
                       Title = "Edit Category";
                       btnDelete.Enabled = true;
                       EditCategory();
                   }
               }

               btnDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this category?')");
        }

        protected void AddNewCategory()
        {
            if (ParentCategoryId == -1)
            {
                IList<Category> rootCats = _categoryService.GetAllRootCategories(ActiveSite);
                txtPath.Text = _categoryService.GetPathFragmentFromPosition(rootCats.Count + 1);
            }
            else
            {
                Category parentCat = _categoryService.GetById(ParentCategoryId);
                txtPath.Text = string.Concat(parentCat.Path, _categoryService.GetPathFragmentFromPosition(parentCat.ChildCategories.Count + 1));
            } 
        }

        protected void EditCategory()
        {
            _category = _categoryService.GetById(CategoryId);
            BindCategory();
        }

        protected void BindCategory()
        {
            txtDescription.Text = _category.Description;
            txtName.Text = _category.Name;
            txtPath.Text = _category.Path;
        }

        protected void BtnSaveClick(object sender, EventArgs e)
        {
            int existing = _categoryService.GetAllCategories(ActiveSite).Where(cat => cat.Name == txtName.Text).Count();
            if (existing < 1)
            {
                Exception exception = null;
                try
                {
                    string path = txtPath.Text;

                    if (CategoryId < 1) _category = new Category();
                    else _category = _categoryService.GetById(CategoryId);

                    if (ParentCategoryId != -1) //not is root category
                    {
                        _category.ParentCategory = _categoryService.GetById(ParentCategoryId);
                    }
                    _category.Site = ActiveSite;
                    _category.Description = txtDescription.Text;
                    _category.Name = txtName.Text;
                    _category.Path = path;
                    _category.Position = _categoryService.GetPositionFromPath(path);

                    if (_category.Id < 0)
                    {
                        _categoryService.CreateCategory(_category);
                    }
                    else
                    {
                        _categoryService.UpdateCategory(_category);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    ShowError("Could not save category. Details:<br />" + ex.ToString());
                }
                if (exception == null) Response.Redirect("~/Admin/Categories.aspx?SiteId=" + ActiveSite.Id.ToString());
            }
            else 
            {
                ShowError("A category of that name already exists. Categories must be uniquely named.");
            }
        }

        protected void BtnCancelClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Categories.aspx?SiteId=" + ActiveSite.Id.ToString());
        }

        protected void BtnDeleteClick(object sender, EventArgs e)
        {
            Exception exception = null;
            try
            {
                _categoryService.DeleteCategory(_categoryService.GetById(CategoryId));
            }
            catch (Exception ex)
            {
                exception = ex;
                ShowError("Could not delete category. Details:<br />" + ex.ToString());
            }
            if (exception == null) Response.Redirect("~/Admin/Categories.aspx?SiteId=" + ActiveSite.Id.ToString());
        }

    }
}

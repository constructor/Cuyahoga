using System.Collections.Generic;
using Cuyahoga.Core.Domain;

namespace Cuyahoga.Core.Service.Content
{
	public interface ICategoryService
	{
		/// <summary>
		/// Get by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Category GetCategoryById(int id);

		/// <summary>
		/// Gets all categories that have no parent category
		/// </summary>
		/// <returns></returns>
		IList<Category> GetAllRootCategories(Site site);

		/// <summary>
		/// Delete a single category.
		/// </summary>
		/// <param name="category"></param>
		void DeleteCategory(Category category);

		/// <summary>
		/// Get all categories ordered by hierarchy (via Path).
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		IEnumerable<Category> GetAllCategories(Site site);

		/// <summary>
		/// Create a new category.
		/// </summary>
		/// <param name="category"></param>
		void CreateCategory(Category category);

		/// <summary>
		/// Update an existing category.
		/// </summary>
		/// <param name="category"></param>
		void UpdateCategory(Category category);




        #region Classic admin 

            ///// <summary>
            ///// Moves on category specified by oldPath to newPath, correcting the parent category reference if necessary
            ///// </summary>
            ///// <param name="oldPath"></param>
            ///// <param name="newPath"></param>
            //void MoveCategoryToNewPath(string oldPath, string newPath);
            /// <summary>
            /// Moves on category specified by oldPath to newPath, correcting the parent category reference if necessary
            /// </summary>
            /// <param name="oldPath"></param>
            /// <param name="newPath"></param>
            void MoveCategoryToNewPathAdmin(Site site, string rootpath, string oldPath, string newPath);

            /// <summary>
            /// Gets the position from the last path fragment
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            int GetPositionFromPath(string path);

            /// <summary>
            /// Gets the position from the supplied path fragment
            /// </summary>
            /// <param name="pathFragment"></param>
            /// <returns></returns>
            int GetPositionFromPathFragment(string pathFragment);

            /// <summary>
            /// Gets the path fragment for a given position
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            string GetPathFragmentFromPosition(int position);











            /// <summary>
            /// Gets all categories that start with the specified path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            IList<Category> GetByPathStartsWith(string path);
            /// <summary>
            /// Gets all categories that start with the specified path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            IList<Category> GetByPathStartsWithAndSite(Site site, string path);

            /// <summary>
            /// Gets all categories that start with the specified path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            IList<Category> GetByPathByParent(Site site, string path);

            /// <summary>
            /// Get by id
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            Category GetById(int id);

            /// <summary>
            /// Gets one category matching the supplied path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            Category GetByExactPath(string path);
            /// <summary>
            /// Gets one category matching the supplied path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            Category GetByExactPathAndSite(Site site, string path);

        #endregion
    }
}

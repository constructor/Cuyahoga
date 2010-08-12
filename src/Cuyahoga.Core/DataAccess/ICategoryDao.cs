using System;
using System.Collections.Generic;
using System.Text;

using Cuyahoga.Core.Domain;

namespace Cuyahoga.Core.DataAccess
{
    public interface ICategoryDao
    {
        /// <summary>
        /// Get all categories that have no parent category for a given site.
        /// </summary>
        /// <returns></returns>
        IList<Category> GetAllRootCategories(Site site);

		/// <summary>
		/// Get all categories for a given site.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
    	IEnumerable<Category> GetAllCategories(Site site);


        #region Classic admin

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

            /// <summary>
            /// Gets categories by the specified partial category path, ordered by path
            /// </summary>
            /// <param name="path"></param>
            IList<Category> GetByPathByParent(Site site, string path);

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Cuyahoga.Core.Domain;
using NHibernate;
using Castle.Facilities.NHibernateIntegration;

namespace Cuyahoga.Core.DataAccess
{
    class CategoryDao : ICategoryDao
    {
        private readonly ISessionManager sessionManager;

        public CategoryDao(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        /// <summary>
        /// Gets all root categories, ordered by path for a given site
        /// </summary>
        /// <returns></returns>
        public IList<Category> GetAllRootCategories(Site site)
        {
            ISession session = this.sessionManager.OpenSession();
            string hql = "from Cuyahoga.Core.Domain.Category c where c.Site = :site and c.ParentCategory is null order by c.Path asc";
            IQuery query = session.CreateQuery(hql);
        	query.SetParameter("site", site);
            return query.List<Category>();
        }

    	public IEnumerable<Category> GetAllCategories(Site site)
    	{
			ISession session = this.sessionManager.OpenSession();
			string hql = "from Cuyahoga.Core.Domain.Category c where c.Site = :site order by c.Path asc";
			IQuery query = session.CreateQuery(hql);
			query.SetParameter("site", site);
			return query.List<Category>();
        }

        #region Classic admin methods

            /// <summary>
            /// Gets categories by the specified partial category path, ordered by path
            /// </summary>
            /// <param name="path"></param>
            public IList<Category> GetByPathStartsWith(string path)
            {
                ISession session = this.sessionManager.OpenSession();
                string hql = "from Cuyahoga.Core.Domain.Category c where c.Path like :path order by c.Path asc";
                IQuery query = session.CreateQuery(hql);
                query.SetString("path", string.Concat(path, "%"));
                return query.List<Category>();
            }
            /// <summary>
            /// Gets categories by the specified partial category path, ordered by path
            /// </summary>
            /// <param name="path"></param>
            public IList<Category> GetByPathStartsWithAndSite(Site site, string path)
            {
                ISession session = this.sessionManager.OpenSession();
                string hql = "from Cuyahoga.Core.Domain.Category c where c.Site = :site and c.Path like :path order by c.Path asc";
                IQuery query = session.CreateQuery(hql);
                query.SetEntity("site", site);
                query.SetString("path", string.Concat(path, "%"));
                return query.List<Category>();
            }

            /// <summary>
            /// Gets categories by the specified partial category path, ordered by path
            /// </summary>
            /// <param name="path"></param>
            public IList<Category> GetByPathByParent(Site site, string path)
            {
                ISession session = this.sessionManager.OpenSession();
                string hql = "from Cuyahoga.Core.Domain.Category c where c.Site = :site and c.Path like :path order by c.Path asc";
                IQuery query = session.CreateQuery(hql);
                query.SetEntity("site", site);
                query.SetString("path", string.Concat(path, "%"));
                return query.List<Category>();
            }

            /// <summary>
            /// Gets a category by the specified category path
            /// </summary>
            /// <param name="path"></param>
            public Category GetByExactPath(string path)
            {
                ISession session = this.sessionManager.OpenSession();
                string hql = "from Cuyahoga.Core.Domain.Category c where c.Path = :path";
                IQuery query = session.CreateQuery(hql);
                query.SetString("path", path);
                return query.UniqueResult<Category>();
            }

            /// <summary>
            /// Gets a category by the specified category path
            /// </summary>
            /// <param name="path"></param>
            public Category GetByExactPathAndSite(Site site, string path)
            {
                ISession session = this.sessionManager.OpenSession();
                string hql = "from Cuyahoga.Core.Domain.Category c where c.Site = :site and c.Path = :path";
                IQuery query = session.CreateQuery(hql);
                query.SetEntity("site", site);
                query.SetString("path", path);
                return query.UniqueResult<Category>();
            }

        #endregion
    }
}

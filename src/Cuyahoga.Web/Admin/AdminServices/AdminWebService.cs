using System;
using System.Web;
using System.Web.Services;

using Castle.Windsor;
using Cuyahoga.Web.Util;
using Cuyahoga.Core.Util;

using Cuyahoga.Core.DataAccess;
using Cuyahoga.Core.Service;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Service.Files;

namespace Cuyahoga.Web.Admin.AdminServices
{
    public class AdminWebService : System.Web.Services.WebService
    {
        #region private
            private IWindsorContainer _cuyahogaContainer;
            private ICommonDao _commonDao;
            private ISiteService _siteService;
            private ISectionService _sectionService;
            private INodeService _nodeService;
            private IUserService _userService;
        #endregion private

        #region Constructor
            public AdminWebService()
            {
                this._cuyahogaContainer = IoC.Container;
                this._commonDao = CuyahogaContainer.Resolve<ICommonDao>();
                this._siteService = CuyahogaContainer.Resolve<ISiteService>();
                this._sectionService = CuyahogaContainer.Resolve<ISectionService>();
                this._nodeService = CuyahogaContainer.Resolve<INodeService>();
                this._userService = CuyahogaContainer.Resolve<IUserService>();
            }
        #endregion

        #region protected
            protected IWindsorContainer CuyahogaContainer
            {
                get { return this._cuyahogaContainer; }
            }
            protected ISiteService SiteService
            {
                get
                {
                    return _siteService;
                }
            }
            protected ISectionService SectionService
            {
                get
                {
                    return _sectionService;
                }
            }
            protected INodeService NodeService
            {
                get
                {
                    return _nodeService;
                }
            }
            protected IUserService UserService
            {
                get
                {
                    return _userService;
                }
            }
            protected ICommonDao CommonDao
            {
                get
                {
                    return _commonDao;
                }
            }
        #endregion public

    }
}

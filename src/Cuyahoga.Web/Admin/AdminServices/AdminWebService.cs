using System;
using System.Web;
using System.Web.Services;

using Castle.Windsor;
using Cuyahoga.Web.Util;
using Cuyahoga.Core.Util;

using Cuyahoga.Core.Service;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Service.Files;

namespace Cuyahoga.Web.Admin.AdminServices
{
    public class AdminWebService : System.Web.Services.WebService
    {
        #region Constructor
        public AdminWebService()
            {
                this._cuyahogaContainer = IoC.Container;
                this._siteService = CuyahogaContainer.Resolve<ISiteService>();
                this._nodeService = CuyahogaContainer.Resolve<INodeService>();
                this._userService = CuyahogaContainer.Resolve<IUserService>();
            }
        #endregion

        #region private
            private IWindsorContainer _cuyahogaContainer;
            private ISiteService _siteService;
            private INodeService _nodeService;
            private IUserService _userService;
        #endregion private

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
            protected CoreRepository CoreRepository
            {
                get
                {
                    return HttpContext.Current.Items["CoreRepository"] as CoreRepository;
                }
            }
        #endregion public

    }
}

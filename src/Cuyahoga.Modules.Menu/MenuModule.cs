using System;
using System.Collections;
using System.Web;
using System.Collections.Generic;

using NHibernate;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

using Cuyahoga.Core;
using Cuyahoga.Web.Util;
using Cuyahoga.Core.Util;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Search;
using Cuyahoga.Core.Service;
using Cuyahoga.Core.Service.Files;
using Cuyahoga.Core.Communication;

namespace Cuyahoga.Modules.Menu
{
    /// <summary>
    /// Menu rendering module
    /// </summary>
    [Transactional]
    public class MenuModule : ModuleBase, INHibernateModule
    {
        private int _firstLevel;
        private int _lastLevel;
        private bool _childrenwhenclicked;
        private bool _requiresjQuery;
        public TypeRender _typerender;
        private ISessionManager _sessionManager;
     
        #region Properties
            /// <summary>
            /// Property FirstLevel (int)
            /// </summary>
            public int FirstLevel
            {
                get { return this._firstLevel; }
            }

            /// <summary>
            /// Property LastLevel (int)
            /// </summary>
            public int LastLevel
            {
                get { return this._lastLevel; }
            }

            /// <summary>
            /// Show the children of the active node 'clicked' only. (bool)
            /// </summary>
            public bool ChildrenWhenClicked
            {
                get { return this._childrenwhenclicked; }
            }

            /// <summary>
            /// Show if the menu requires jQuery to be registered to the page (bool)
            /// </summary>
            public bool RequiresjQuery()
            {
                return _requiresjQuery;
            }
        #endregion

        #region Constructor
            public MenuModule(ISessionManager sessionManager)
		    {
                this._sessionManager = sessionManager;

            }
        #endregion

        public override void ReadSectionSettings()
        {
            base.ReadSectionSettings();
            try
            {
                this._firstLevel = Convert.ToInt16(base.Section.Settings["FIRST_LEVEL"]);
                this._lastLevel = Convert.ToInt16(base.Section.Settings["LAST_LEVEL"]);
                this._childrenwhenclicked = Convert.ToBoolean(base.Section.Settings["CHILDREN_ACTNODE"]);
                this._requiresjQuery = Convert.ToBoolean(base.Section.Settings["REQUIRES_JQUERY"]);
                this._typerender = (TypeRender)Enum.Parse(typeof(TypeRender), base.Section.Settings["TYPE_RENDER"].ToString());
            }
            catch
            {
              
            }
        }

    }

    #region enum

    public enum TypeRender
    {
        /// <summary>
        /// Render NavigationTree.
        /// </summary>
        NavigationTree,
        /// <summary>
        /// Sort by DateCreated.
        /// </summary>
        NavigationBreadcrumb,
        /// <summary>
        /// Sort by DateModified.
        /// </summary>
    }
    #endregion

}
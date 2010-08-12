using System;
using Cuyahoga.Core;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service;
using Castle.Windsor;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.Util;
using log4net;
using CuyahogaUser = Cuyahoga.Core.Domain.User;

namespace Cuyahoga.Web.UI
{
	/// <summary>
	/// Base class for all aspx pages in Cuyahoga (public and admin).
	/// </summary>
	public class CuyahogaPage : System.Web.UI.Page, ICuyahogaPage
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof (CuyahogaPage));
		
        private IWindsorContainer _container;

		/// <summary>
		/// A reference to the Windsor container that can be used as a Service Locator for service classes.
		/// </summary>
		protected IWindsorContainer Container
		{
			get { return this._container; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public CuyahogaPage()
		{
            this._container = IoC.Container;
		}

		protected override void OnInit(EventArgs e)
		{
			// Set the CuyahogaContext
			ICuyahogaContext cuyahogaContext = Container.Resolve<ICuyahogaContext>();
			if (User.Identity.IsAuthenticated && this.User is CuyahogaUser)
			{
				cuyahogaContext.SetUser((CuyahogaUser)this.User);
			}
			ISiteService siteService = Container.Resolve<ISiteService>();
			try
			{
				Site currentSite = siteService.GetSiteBySiteUrl(UrlUtil.GetSiteUrl());
				cuyahogaContext.SetSite(currentSite);
				cuyahogaContext.PhysicalSiteDataDirectory = Server.MapPath(currentSite.SiteDataDirectory);
			}
			catch (Exception ex)
			{
				logger.Error("An unexpected error occured while setting the current site context.", ex);
			}
			base.OnInit(e);
		}
	}
}

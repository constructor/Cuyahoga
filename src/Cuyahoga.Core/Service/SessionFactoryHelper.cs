using System;
using System.Reflection;

using NHibernate;
using NHibernate.Cfg;
using Castle.MicroKernel;

namespace Cuyahoga.Core.Service
{
	/// <summary>
	/// Provides utility methods to maintain the NHibernate SessionFactory.
	/// </summary>
	public class SessionFactoryHelper
	{
		private IKernel _kernel;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="kernel"></param>
		public SessionFactoryHelper(IKernel kernel)
		{
			this._kernel = kernel;
		}

		/// <summary>
		/// Add a new assembly to the configuration and build a new SessionFactory.
		/// </summary>
		/// <param name="assembly"></param>
		public void AddAssembly(Assembly assembly)
		{
			Configuration nhConfiguration = this._kernel[typeof(Configuration)] as Configuration;
			//has to have the second argument (true) for skipping ordering
            nhConfiguration.AddAssembly(assembly);
			ISessionFactory newSessionFactory = nhConfiguration.BuildSessionFactory();
			ReplaceSessionFactory(newSessionFactory);
		}

        private void ReplaceSessionFactory(ISessionFactory nhSessionFactory)
        {
            this._kernel.RemoveComponent("nhibernate.factory");
            this._kernel.AddComponentInstance("nhibernate.factory", nhSessionFactory);
        }
	}
}

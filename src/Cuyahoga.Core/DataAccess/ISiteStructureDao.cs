using System;
using System.Collections;
using System.Collections.Generic;
using Cuyahoga.Core.Domain;

namespace Cuyahoga.Core.DataAccess
{
	/// <summary>
	/// Provides data access for site structure components.
	/// </summary>
	public interface ISiteStructureDao
	{
		/// <summary>
		/// Get a single Site object based on the root url.
		/// </summary>
		/// <param name="siteUrl"></param>
		/// <returns></returns>
		Site GetSiteBySiteUrl(string siteUrl);

		/// <summary>
		/// Get a single SiteAlias objects based on the root url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		SiteAlias GetSiteAliasByUrl(string url);

		/// <summary>
		/// Get all SiteAlias objects that belong to a given site.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		IList GetSiteAliasesBySite(Site site);

		/// <summary>
		/// Get the root nodes for a given site.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		IList<Node> GetRootNodes(Site site);

		/// <summary>
		/// Get the root node for the given culture and site.
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="site"></param>
		/// <returns></returns>
		Node GetRootNodeByCultureAndSite(string culture, Site site);

		/// <summary>
		/// Get a node by short description and site.
		/// </summary>
		/// <param name="shortDescription"></param>
		/// <param name="site"></param>
		/// <returns></returns>
		Node GetNodeByShortDescriptionAndSite(string shortDescription, Site site);

		/// <summary>
		/// Get all nodes that are connected to a given template.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		IList GetNodesByTemplate(Template template);

		/// <summary>
		/// Get all sections that belong to a given node, sorted by placeholder and position.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		IList<Section> GetSortedSectionsByNode(Node node);

		/// <summary>
		/// Get all orphaned sections (not related to any node).
		/// </summary>
		/// <returns></returns>
		IList<Section> GetUnconnectedSections();

		/// <summary>
		/// Get all orphaned sections (not related to any node) by a given site.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		IList<Section> GetUnconnectedSectionsBySite(Site site);

		/// <summary>
		/// Get all templates where the given section is attached to.
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		IList<Template> GetTemplatesBySection(Section section);

		/// <summary>
		/// Get all sections that are related to the given module type.
		/// </summary>
		/// <param name="moduleType"></param>
		/// <returns></returns>
		IList<Section> GetSectionsByModuleType(ModuleType moduleType);

		/// <summary>
		/// Get all sections that are related to the given module type and site.
		/// </summary>
		/// <param name="moduleType"></param>
		/// <param name="site"></param>
		/// <returns></returns>
		IList<Section> GetSectionsByModuleTypeAndSite(ModuleType moduleType, Site site);

		/// <summary>
		/// Get all sections that are related to the given module types.
		/// </summary>
		/// <param name="moduleTypes"></param>
		/// <returns></returns>
		IList GetSectionsByModuleTypes(IList moduleTypes);

		/// <summary>
		/// Get all module types that are currently in use (that have related sections) 
		/// in the Cuyahoga installation.
		/// </summary>
		/// <returns></returns>
		IList<ModuleType> GetAllModuleTypesInUse();

		/// <summary>
		/// Save a site instance.
		/// </summary>
		/// <param name="site"></param>
		void SaveSite(Site site);

		/// <summary>
		/// Delete a site instance.
		/// </summary>
		/// <param name="site"></param>
		void DeleteSite(Site site);

        /// <summary>
        /// Delete a site templates.
        /// </summary>
        /// <param name="site"></param>
        void DeleteSiteTemplates(Site site);

		/// <summary>
		/// Save a site alias instance.
		/// </summary>
		/// <param name="siteAlias"></param>
		void SaveSiteAlias(SiteAlias siteAlias);

		/// <summary>
		/// Delete a site alias instance.
		/// </summary>
		/// <param name="siteAlias"></param>
		void DeleteSiteAlias(SiteAlias siteAlias);

        //Custom
        /// <summary>
        /// Apply a template to all nodes in a site
        /// </summary>
        /// <param name="template">The template that will be applied to all the nodes.</param>
        /// <param name="site">The site to which the template will be applied.</param>
        void ApplyTemplateAllNodesInSite(Cuyahoga.Core.Domain.Template template, Cuyahoga.Core.Domain.Site site);

        /// <summary>
        /// Get all nodes of a site
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        IList GetNodesBySite(Site site);
	}
}

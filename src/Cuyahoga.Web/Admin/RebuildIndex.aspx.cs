using System;
using System.Collections;
using System.Collections.Generic;

using Cuyahoga.Web.Admin.UI;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.SiteStructure;
using Cuyahoga.Core.Service.Search;
using Cuyahoga.Core.Service.Content;
using Cuyahoga.Core.Search;

using log4net;

namespace Cuyahoga.Web.Admin
{
    public partial class RebuildIndex : AdminBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RebuildIndex));
        
        private IModuleTypeService _moduleTypeService;
        private ISearchService _searchService;
        private IContentItemService<ContentItem> _contentItemService;

        private Cuyahoga.Core.Service.ICuyahogaContextProvider _cuyahogaContextProvider;
        private Cuyahoga.Core.ICuyahogaContext _cuyahogaContext;

		/// <summary>
		/// Constructor.
		/// </summary>
		public RebuildIndex()
		{
            this._moduleTypeService = Container.Resolve<IModuleTypeService>();
            this._searchService = Container.Resolve<ISearchService>();
            this._contentItemService = Container.Resolve<IContentItemService<ContentItem>>();

            this._cuyahogaContext = Cuyahoga.Core.Util.IoC.Container.Resolve<Cuyahoga.Core.ICuyahogaContext>();
            this._cuyahogaContextProvider = Cuyahoga.Core.Util.IoC.Container.Resolve<Cuyahoga.Core.Service.ICuyahogaContextProvider>();
        }
	
		protected void Page_Load(object sender, System.EventArgs e)
        {
			this.Title = "Rebuild fulltext index";
			if (! this.IsPostBack)
			{
				this.btnRebuild.Attributes.Add("onclick", "this.disabled='true';document.getElementById('pleasewait').style.display = 'block';" + GetPostBackEventReference(btnRebuild).ToString());
				// Make sure all modules are loaded when we enter this page. We can't have a changing
				// module configuration while rebuilding the full-text index.
                EnsureModulesAreLoaded();
			}
			else
			{
				//BuildIndex();
				this.lblMessage.Visible = true;
			}
		}

        protected void EnsureModulesAreLoaded()
        {
            IList<ModuleType> currentModuleTypes = this._moduleTypeService.GetAllModuleTypesInUse();

            foreach (ModuleType moduleType in currentModuleTypes)
            {
                // Just load every module. Just by loading it once, we can be sure that we won't
                // run into strange surprises after pushing the 'Rebuild Index' button becasue
                // some module weren't loaded or configured.
                ModuleBase module = base.ModuleLoader.GetModuleFromType(moduleType);
            }
        }

        protected void BuildIndex()
        {
            //only one rebuild at a time allowed
            //this._searchService.StartRebuildingIndex();

            this.BuildIndexBySites();

            //this._searchService.EndRebuildingIndex();
        }

        protected void BuildIndexBySites()
        {
            IList sites = base.CoreRepository.GetAll(typeof(Site));
            foreach (Site site in sites)
            {
                foreach (Node node in site.RootNodes)
                {
                    try
                    {
                        BuildIndexByNode(node);
                    }
                    catch (Exception ex)
                    {
                        log.Error(String.Format("Indexing contents of Node {0} - {1} failed.", node.Id, node.Title), ex);
                    }
                }
            }
        }

        protected void BuildIndexByCurrentSite()
        {
            foreach (Node node in base.ActiveSite.RootNodes)
            {
                try
                {
                    BuildIndexByNode(node);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Indexing contents of Node {0} - {1} failed.", node.Id, node.Title), ex);
                }
            }
        }

        protected void BuildIndexByNode(Node node)
        {
            foreach (Section section in node.Sections)
            {
                //handle ContentItems
                IList<ContentItem> contentItems = this._contentItemService.FindContentItemsBySection(section);
                try
                {
                    foreach (ContentItem contentItem in contentItems)
                    {
                        if (contentItem is ISearchableContent)
                        {
                            this._searchService.UpdateContent(contentItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Indexing ContentItems of Section {0} - {1} failed.", section.Id, section.Title), ex);
                }

                //handle SearchContents
                ModuleBase module = null;
                try
                {
                    module = base.ModuleLoader.GetModuleFromSection(section);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Unable to create Module for Section {0} - {1}.", section.Id, section.Title), ex);
                }

                if (module is ISearchable)
                {
                    ISearchable searchableModule = (ISearchable)module;
                    try
                    {
                        List<SearchContent> searchContents = new List<SearchContent>(searchableModule.GetAllSearchableContent());
                        this._searchService.UpdateContent(searchContents);
                    }
                    catch (Exception ex)
                    {
                        log.Error(String.Format("Indexing SearchContents of Section {0} - {1} failed.", section.Id, section.Title), ex);
                    }
                }
            }
            foreach (Node childNode in node.ChildNodes)
            {
                BuildIndexByNode(childNode);
            }
        }

        protected void btnRebuild_Click(object sender, EventArgs e)
        {
            BuildIndexByCurrentSite();
        }

    }
}

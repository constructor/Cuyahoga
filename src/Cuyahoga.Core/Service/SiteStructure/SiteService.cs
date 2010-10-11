using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Castle.Services.Transaction;
using Cuyahoga.Core.Service.Files;
using log4net;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.DataAccess;

namespace Cuyahoga.Core.Service.SiteStructure
{
	/// <summary>
	/// Provides functionality to manage site instances.
	/// </summary>
	[Transactional]
	public class SiteService : ISiteService
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SiteService));
		private ISiteStructureDao _siteStructureDao;
		private ICommonDao _commonDao;
		private IFileService _fileService;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="siteStructureDao"></param>
		/// <param name="commonDao"></param>
		/// <param name="fileService"></param>
		public SiteService(ISiteStructureDao siteStructureDao, 
            ICommonDao commonDao, 
            IFileService fileService,
            ITemplateService templateService)
		{
			this._siteStructureDao = siteStructureDao;
			this._commonDao = commonDao;
			this._fileService = fileService;
		}

		#region ISiteService Members

		public Site GetSiteById(int siteId)
		{
			return (Site)this._commonDao.GetObjectById(typeof(Site), siteId);
		}

		public Site GetSiteBySiteUrl(string siteUrl)
		{
			Site site = this._siteStructureDao.GetSiteBySiteUrl(siteUrl);
			// Try to resolve the site via SiteAlias
			if (site == null)
			{
				SiteAlias sa = this._siteStructureDao.GetSiteAliasByUrl(siteUrl);
				if (sa != null)
				{
					site = sa.Site;
				}
			}
			return site;
		}

		public SiteAlias GetSiteAliasById(int siteAliasId)
		{
			return (SiteAlias)this._commonDao.GetObjectById(typeof(SiteAlias), siteAliasId);
		}

		public SiteAlias GetSiteAliasByUrl(string url)
		{
			return this._siteStructureDao.GetSiteAliasByUrl(url);
		}

		public IList GetSiteAliasesBySite(Site site)
		{
			return this._siteStructureDao.GetSiteAliasesBySite(site);
		}

		public IList GetAllSites()
		{
			return this._commonDao.GetAll(typeof(Site));
		}

        [Transaction(TransactionMode.RequiresNew)]
        public virtual void CreateSite(Site site, string siteDataRoot, IList<Template> templatesToCopy, string systemTemplatesDirectory)
        {
            try
            {
                this._commonDao.Flush();//TEST 

                // 1. Add global roles to site
                IList<Role> roles = this._commonDao.GetAll<Role>();
                foreach (Role role in roles)
                {
                    if (role.IsGlobal)
                    {
                        site.Roles.Add(role);
                    }
                }

                // 2. Save site in database
                //this._commonDao.SaveObject(site);
                this.SaveSite(site);

                // 3. Create SiteData folder structure
                if (!this._fileService.CheckIfDirectoryIsWritable(siteDataRoot))
                {
                    throw new IOException(string.Format("Unable to create the site because the directory {0} is not writable.", siteDataRoot));
                }

                string siteDataPhysicalDirectory = Path.Combine(siteDataRoot, site.Id.ToString());
                this._fileService.CreateDirectory(siteDataPhysicalDirectory);
                this._fileService.CreateDirectory(Path.Combine(siteDataPhysicalDirectory, "file"));
                this._fileService.CreateDirectory(Path.Combine(siteDataPhysicalDirectory, "image"));
                this._fileService.CreateDirectory(Path.Combine(siteDataPhysicalDirectory, "flash"));
                this._fileService.CreateDirectory(Path.Combine(siteDataPhysicalDirectory, "media"));
                this._fileService.CreateDirectory(Path.Combine(siteDataPhysicalDirectory, "downloads"));
                this._fileService.CreateDirectory(Path.Combine(siteDataPhysicalDirectory, "index"));

                string siteTemplatesDirectory = Path.Combine(siteDataPhysicalDirectory, "templates");
                this._fileService.CreateDirectory(siteTemplatesDirectory);

                // 4. Copy templates
                IList<string> templateDirectoriesToCopy = new List<string>();
                foreach (Template template in templatesToCopy)
                {
                    string templateDirectoryName = template.BasePath.Substring(template.BasePath.IndexOf("/") + 1);
                    if (!templateDirectoriesToCopy.Contains(templateDirectoryName))
                    {
                        templateDirectoriesToCopy.Add(templateDirectoryName);
                    }
                    Template newTemplate = template.GetCopy();
                    this._commonDao.SaveObject(newTemplate);
                    newTemplate.Site = site;

                    this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Site.Templates", site.Id);
                    this._commonDao.RemoveQueryFromCache("Templates");


                    //Also copy the sections that are assigned to this template
                    try
                    {
                        foreach (KeyValuePair<string, Section> entry in template.Sections)
                        {
                            Section s = entry.Value as Section;
                            newTemplate.Sections.Add(entry.Key, s);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("An unexpected error occured while creating a new site.", ex);
                        throw;
                    }

                    //Custom Added (without '/')
                    newTemplate.BasePath = "SiteData/" + site.Id.ToString() + "/templates/" + templateDirectoryName;

                    site.Templates.Add(newTemplate);

                    //If the default template name is the same as the update to copied version
                    if (site.DefaultTemplate.Name == newTemplate.Name)
                    {
                        site.DefaultTemplate = newTemplate;
                    }

                    //Save newTemplate
                    this._commonDao.SaveOrUpdateObject(newTemplate);
                    //Save the site
                    this._commonDao.SaveOrUpdateObject(site);
                }

                //Copy the templates
                foreach (string templateDirectory in templateDirectoriesToCopy)
                {
                    string sourceDir = Path.Combine(systemTemplatesDirectory, templateDirectory);
                    string targetDir = Path.Combine(siteTemplatesDirectory, templateDirectory);
                    
                    this._fileService.CopyDirectoryContents(sourceDir, targetDir);
                }

            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occured while creating a new site.", ex);
                throw;
            }
        }

		[Transaction(TransactionMode.RequiresNew)]
		public virtual void SaveSite(Site site)
		{
			try
			{
				// We need to use a specific DAO to also enable clearing the query cache.
				this._siteStructureDao.SaveSite(site);
			}
			catch (Exception ex)
			{
				log.Error("Error saving site", ex);
				throw;
			}
		}

		[Transaction(TransactionMode.RequiresNew)]
		public virtual void DeleteSite(Site site)
		{
            if (site.RootNodes.Count > 0)
			{
				throw new Exception("Can't delete a site when there are still related nodes. Please delete all nodes before deleting an entire site.");
			}
			else
			{
				IList aliases = this._siteStructureDao.GetSiteAliasesBySite(site);
				if (aliases.Count > 0)
				{
					throw new Exception("Unable to delete a site when a site has related aliases.");
				}
				else
				{
					try
					{
                        string sitedatafolder = System.Web.HttpContext.Current.Server.MapPath(site.SiteDataDirectory);

                        // We need to use a specific DAO to also enable clearing the query cache.
                        this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Site.Templates", site.Id);
                        this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Site.RootCategories", site.Id);
                        this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Site.RootNodes", site.Id);
                        this._commonDao.RemoveQueryFromCache("Templates");

                        this._siteStructureDao.DeleteSite(site);
                        
                        //Delete Templates folder first
                        if (Directory.Exists(sitedatafolder + "Templates"))
                            this._fileService.DeleteDirectory(sitedatafolder + "Templates");

                        //if (Directory.Exists(sitedatafolder))
                        //    this._fileService.DeleteDirectory(sitedatafolder);

					}
					catch (Exception ex)
					{
						log.Error("Error deleting site", ex);
						throw;
					}
				}				
			}
		}

		[Transaction(TransactionMode.RequiresNew)]
		public virtual void SaveSiteAlias(SiteAlias siteAlias)
		{
			try
			{
				// We need to use a specific DAO to also enable clearing the query cache.
				this._siteStructureDao.SaveSiteAlias(siteAlias);
			}
			catch (Exception ex)
			{
				log.Error("Error saving site alias", ex);
				throw;
			}
		}

		[Transaction(TransactionMode.RequiresNew)]
		public virtual void DeleteSiteAlias(SiteAlias siteAlias)
		{
			try
			{
				// We need to use a specific DAO to also enable clearing the query cache.
				this._siteStructureDao.DeleteSiteAlias(siteAlias);
			}
			catch (Exception ex)
			{
				log.Error("Error deleting site alias", ex);
				throw;
			}
		}

		#endregion
	}
}

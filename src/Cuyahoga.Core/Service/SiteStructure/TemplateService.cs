using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Services.Transaction;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.DataAccess;
using Cuyahoga.Core.Service.Files;
using ICSharpCode.SharpZipLib.Zip;
using NHibernate.Criterion;

namespace Cuyahoga.Core.Service.SiteStructure
{
	/// <summary>
	/// Default implementation of ITemplateService.
	/// </summary>
	[Transactional]
	public class TemplateService : ITemplateService
	{
		private static readonly ICollection<string> AllowedDirectories = new[] { "css/", "images/", "javascript/", "js/" };
		private static readonly ICollection<string> AllowedExtensions = new[] { ".ascx", ".css", ".gif", ".png", ".jpg", ".js", ".swf", ".xml", ".txt" };

		private readonly ICommonDao _commonDao;
		private IFileService _fileService;
        private ISiteStructureDao _siteStructureDao;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="commonDao"></param>
		/// <param name="fileService"></param>
        public TemplateService(ICommonDao commonDao, IFileService fileService, ISiteStructureDao siteStructureDao)
		{
			this._commonDao = commonDao;
			_fileService = fileService;
            _siteStructureDao = siteStructureDao;
		}

		#region ITemplateService Members

		public IList GetAllTemplates()
		{
			return this._commonDao.GetAll(typeof(Template), "Name");
		}

		public IList<Template> GetAllSystemTemplates()
		{
			DetachedCriteria crit = DetachedCriteria.For(typeof(Template))
				.Add(Expression.IsNull("Site"));
			return this._commonDao.GetAllByCriteria<Template>(crit);
		}

		public Template GetTemplateById(int templateId)
		{
			return (Template)this._commonDao.GetObjectById(typeof(Template), templateId);
		}

		public IList<Template> GetAllTemplatesBySite(Site site)
		{
			DetachedCriteria crit = DetachedCriteria.For(typeof(Template))
				.Add(Expression.Eq("Site", site))
				.AddOrder(Order.Asc("Name"));
			return this._commonDao.GetAllByCriteria<Template>(crit);
		}

		[Transaction(TransactionMode.RequiresNew)]
		public void SaveTemplate(Template template)
		{
			this._commonDao.SaveOrUpdateObject(template);
		}

		[Transaction(TransactionMode.RequiresNew)]
		public void DeleteTemplate(Template template)
		{
			this._commonDao.DeleteObject(template);
		}

		[Transaction(TransactionMode.RequiresNew)]
		public void ExtractTemplatePackage(string packageFilePath, Stream packageStream)
		{
            ZipFile templatesArchive = new ZipFile(packageStream);

			// The template dir is the name of the zip package by convention.
			string templateDir = Path.GetFileNameWithoutExtension(packageFilePath);
			string physicalTemplatesDirectory = Path.GetDirectoryName(packageFilePath);
			string physicalTargetTemplateDir = Path.Combine(physicalTemplatesDirectory, templateDir);

            bool templateValid = false;

            bool invalidDirectories = false;
            bool invalidExtension = false;
            bool invalidAscx = false;
            bool deprecatedControls = false;
            bool invalidCss = false;
            bool validfckstyles = false;

            // Do simple validation tests first
            foreach (ZipEntry zipEntry in templatesArchive)
            {
                if (zipEntry.IsDirectory)
                {
                    if (!AllowedDirectories.Contains(zipEntry.Name.ToLower()))
                    {
                        invalidDirectories = true;
                    }
                }
                if (zipEntry.IsFile)
                {
                    string targetFilePath = Path.Combine(physicalTargetTemplateDir, zipEntry.Name);
                    string extension = Path.GetExtension(targetFilePath);
                    // Check allowed extensions.
                    if (!AllowedExtensions.Contains(extension))
                    {
                        invalidExtension = true;
                    }
                    // ascx controls should be in the root of the template dir
                    if (extension == ".ascx")
                    {
                        if (!(Path.GetDirectoryName(targetFilePath).EndsWith(templateDir)))
                        {
                            invalidAscx = true;
                        }

                        //Check for deprecated controls
                        System.IO.StreamReader sr = new System.IO.StreamReader(templatesArchive.GetInputStream(zipEntry));
                        string input = null;
                        while ((input = sr.ReadLine()) != null)
                        {
                            if (input.Contains("~/Controls/Navigation/NavigationLevelZeroOne.ascx")
                                || input.Contains("~/Controls/Navigation/NavigationLevelTwo.ascx")
                                || input.Contains("<uc1:navigation")
                                || input.Contains("<uc2:subnavigation")
                                )
                            {
                                deprecatedControls = true;
                                break;
                            }
                        }
                    }
                    // css files should be in the css subdirectory
                    if (extension == ".css" && !(Path.GetDirectoryName(targetFilePath).ToLower().EndsWith(@"\css")))
                    {
                        invalidCss = true;
                    }
                    // fckstyles.xml file should be in the css subdirectory
                    if (Path.GetFileName(zipEntry.Name).ToLower() == "fckstyles.xml" && (Path.GetDirectoryName(targetFilePath).ToLower().EndsWith(@"\css")))
                    {
                        validfckstyles = true;
                    }

                }
            }

            //Evaluate tests
            templateValid = (!invalidDirectories
                && !invalidExtension
                && !invalidAscx
                && !deprecatedControls
                && !invalidCss
                && validfckstyles);

            // Extract template if tests passed
            if (templateValid)
            {
                if (!Directory.Exists(physicalTargetTemplateDir))
                {
                    this._fileService.CreateDirectory(physicalTargetTemplateDir);
                }
                foreach (ZipEntry zipEntry in templatesArchive)
                {
                    if (zipEntry.IsDirectory)
                    {
                        this._fileService.CreateDirectory(Path.Combine(physicalTargetTemplateDir, zipEntry.Name.Replace("/", String.Empty)));
                    }
                    if (zipEntry.IsFile)
                    {
                        string targetFilePath = Path.Combine(physicalTargetTemplateDir, zipEntry.Name);
                        string extension = Path.GetExtension(targetFilePath);
                        this._fileService.WriteFile(targetFilePath, templatesArchive.GetInputStream(zipEntry));
                    }
                }
            }
            else 
            {
                if (invalidDirectories)
                {
                    throw new InvalidPackageException("InvalidDirectoryInPackageFoundException");
                }
                if (invalidExtension)
                {
                    throw new InvalidPackageException("InvalidAscxLocationException");
                }
                if (invalidAscx)
                {
                    throw new InvalidPackageException("InvalidAscxLocationException");
                }
                if (deprecatedControls)
                {
                    throw new InvalidPackageException("ContainsDeprecatedNavigationControlsException");
                }
                if (invalidCss)
                {
                    throw new InvalidPackageException("InvalidCssLocationException");
                }
                if (!validfckstyles)
                {
                    throw new InvalidPackageException("NoValidFckStylesException");
                }
            }

		}

		[Transaction(TransactionMode.Requires)]
		public void AttachSectionToTemplate(Section section, Template template, string placeholder)
		{
			// First test if the section is already attached. If so, remove.
			if (template.Sections.Any(s => s.Value == section))
			{
				RemoveSectionFromTemplate(section, template);
			}
			// Add the section to the template
			template.Sections.Add(placeholder, section);
			this._commonDao.UpdateObject(template);
			// Invalidate cache 
			this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Template.Sections", section.Id);
		}

		[Transaction(TransactionMode.Requires)]
		public void RemoveSectionFromTemplate(Section section, Template template)
		{
			string placeholder = template.Sections.Where(s => s.Value == section).Select(s => s.Key).SingleOrDefault();
			if (placeholder != null)
			{
				template.Sections.Remove(placeholder);
				this._commonDao.UpdateObject(template);
				// Invalidate cache 
				this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Template.Sections", section.Id);
			}
		}

        //Custom Added
        public IList<Template> GetUnassignedTemplates()
        {
            DetachedCriteria crit = DetachedCriteria.For(typeof(Template))
                .Add(Expression.IsNull("Site"))
                .AddOrder(Order.Asc("Name"));
            return this._commonDao.GetAllByCriteria<Template>(crit);
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void DeleteSiteTemplates(Site site)
        {
            this._siteStructureDao.DeleteSiteTemplates(site);
        }

		#endregion
	}
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;

using NHibernate;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

using Cuyahoga.Core;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.Content;
using Cuyahoga.Core.Service.Files;

namespace Cuyahoga.Modules.Downloads
{
    /// <summary>
    /// The Downloads Module provides file downloads for users.
    /// </summary>
    [Transactional]
    public class DownloadsModule : ModuleBase, INHibernateModule
    {
        private string _physicalDir;
        private bool _showPublisher;
        private bool _showDateModified;
        private bool _showNumberOfDownloads;
        private int _currentFileId;
        private DownloadsModuleActions _currentAction;
        private ISessionManager _sessionManager;
        private readonly IContentItemService<FileResource> _contentItemService;
        private IFileService _fileService;
        public IFileResourceService _fileResourceService;

        #region properties

        /// <summary>
        /// The physical directory where the files are located.
        /// </summary>
        /// <remarks>
        /// If there is no setting for the physical directory, the default of
        /// Application_Root/files is used.
        ///	</remarks>
        public string FileDir
        {
            get
            {
                if (this._physicalDir == null)
                {
                    this._physicalDir = HttpContext.Current.Server.MapPath("~/files");
                    CheckPhysicalDirectory(this._physicalDir);
                }
                return this._physicalDir;
            }
            set
            {
                this._physicalDir = value;
                CheckPhysicalDirectory(this._physicalDir);
            }
        }

        /// <summary>
        /// The Id of the file that is to be downloaded.
        /// </summary>
        public int CurrentFileId
        {
            get { return this._currentFileId; }
        }

        /// <summary>
        /// A specific action that has to be done by the module.
        /// </summary>
        public DownloadsModuleActions CurrentAction
        {
            get { return this._currentAction; }
        }

        /// <summary>
        /// Show the name of the user who published the file?
        /// </summary>
        public bool ShowPublisher
        {
            get { return this._showPublisher; }
        }

        /// <summary>
        /// Show the date and time when the file was last updated?
        /// </summary>
        public bool ShowDateModified
        {
            get { return this._showDateModified; }
        }

        /// <summary>
        /// Show the number of downloads?
        /// </summary>
        public bool ShowNumberOfDownloads
        {
            get { return this._showNumberOfDownloads; }
        }

        #endregion

        /// <summary>
        /// DownloadsModule constructor.
        /// </summary>
        /// <param name="fileService">FileService dependency.</param>
        /// <param name="sessionManager">NHibernate session manager dependency.</param>
        public DownloadsModule(IFileService fileService,IFileResourceService fileResourceService, ISessionManager sessionManager, IContentItemService<FileResource> contentItemService)
        {
            this._sessionManager = sessionManager;
            this._fileService = fileService;
            this._contentItemService = contentItemService;
            this._fileResourceService = fileResourceService;
        }

        public override void ReadSectionSettings()
        {
            base.ReadSectionSettings();
            // Set dynamic module settings
            //string physicalDir = Convert.ToString(base.Section.Settings["PHYSICAL_DIR"]);
            string physicalDir = HttpContext.Current.Server.MapPath("~/SiteData/" + this.Section.Node.Site.Id.ToString() + "/downloads/");

            if (physicalDir != String.Empty)
            {
                this._physicalDir = physicalDir;
            }

            this._showPublisher = Convert.ToBoolean(base.Section.Settings["SHOW_PUBLISHER"]);
            this._showDateModified = Convert.ToBoolean(base.Section.Settings["SHOW_DATE"]);
            this._showNumberOfDownloads = Convert.ToBoolean(base.Section.Settings["SHOW_NUMBER_OF_DOWNLOADS"]);
        }

        /// <summary>
        /// Validate module settings.
        /// </summary>
        public override void ValidateSectionSettings()
        {
            // Check if the virtual directory exists.
            // We need to do this based on the section setting because it might be possible that the related section 
            // isn't saved yet.
            if (base.Section != null)
            {
                string physicalDir = Convert.ToString(base.Section.Settings["PHYSICAL_DIR"]);
                if (!String.IsNullOrEmpty(physicalDir))
                {
                    CheckPhysicalDirectory(physicalDir);
                }
            }
            base.ValidateSectionSettings();
        }

        /// <summary>
        /// Get the a file as stream.
        /// </summary>
        /// <returns></returns>
        public System.IO.Stream GetFileAsStream(FileResource file)
        {
            User currentUser = HttpContext.Current.User as User;
            bool viewAllow = false;
            foreach (Role cur in currentUser.Roles) 
            {
                viewAllow = file.ViewRoles.Contains(cur) ? true : false;
            }

            if (viewAllow)
            {
                string physicalFilePath = System.IO.Path.Combine(this.FileDir, file.PhysicalFilePath);
                if (System.IO.File.Exists(physicalFilePath))
                {
                    return this._fileService.ReadFile(physicalFilePath);
                }
                else
                {
                    throw new System.IO.FileNotFoundException("File not found", physicalFilePath);
                }
            }
            else
            {
                throw new AccessForbiddenException("The current user has no permission to download the file.");
            }
        }

        /// <summary>
        /// Retrieve the meta-information of all files that belong to this module.
        /// </summary>
        /// <returns></returns>
        public virtual IList<FileResource> GetAllFiles()
        {
            return this._contentItemService.FindVisibleContentItemsBySection(this.Section, new ContentItemQuerySettings(ContentItemSortBy.PublishedAt, ContentItemSortDirection.DESC));
        }

        /// <summary>
        /// Get the meta-information of a single file.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileResource GetFileById(int fileId)
        {
            try
            {
                return _contentItemService.GetById(fileId);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get File with identifier: " + fileId.ToString(), ex);
            }
        }

        /// <summary>
        /// Insert or update the meta-information of a file and update the file contents at the 
        /// same time in one transaction.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileContents"></param>
        [Transaction(TransactionMode.RequiresNew)]
        public virtual void SaveFile(FileResource file, System.IO.Stream fileContents)
        {
            // Save physical file.
            string fullFilePath = file.PhysicalFilePath + file.FileName;
            this._fileService.WriteFile(fullFilePath, fileContents);
            // Save meta-information.
            SaveFileInfo(file);
        }

        /// <summary>
        /// Insert or update the meta-information of a file.
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public virtual void SaveFileInfo(FileResource file)
        {
            //When saving meta-information use the ContentItem service
            _contentItemService.Save(file);
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        [Transaction(TransactionMode.RequiresNew)]
        public virtual void DeleteFile(FileResource file)
        {
            // Delete physical file.
            //string physicalFilePath = file.PhysicalFilePath;
            string physicalFilePath = System.IO.Path.Combine(file.PhysicalFilePath, file.FileName);
            this._fileService.DeleteFile(physicalFilePath);
            // Delete meta information.
            ISession session = this._sessionManager.OpenSession();
            session.Delete(file);
        }

        /// <summary>
        /// Increase the number of downloads for the given file.
        /// </summary>
        /// <param name="file"></param>
        public virtual void IncreaseNrOfDownloads(FileResource file)
        {
            //If a person who is not authenticated downloads 
            //how do we save the update when ModifiedBy is required?
            //Will only increment for authenticated users
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                file.DownloadCount++;
                file.ModifiedBy = HttpContext.Current.User.Identity as User;
                file.ModifiedAt = DateTime.Now;

                SaveFileInfo(file);
            }
        }

        /// <summary>
        /// Parse the pathinfo.
        /// </summary>
        protected override void ParsePathInfo()
        {
            base.ParsePathInfo();
            if (base.ModuleParams != null)
            {
                if (base.ModuleParams.Length == 3)
                {
                    // First argument is the module action and the second is the Id of the file.
                    try
                    {
                        this._currentAction = (DownloadsModuleActions)Enum.Parse(typeof(DownloadsModuleActions)
                            , base.ModuleParams[0], true);
                        this._currentFileId = Int32.Parse(base.ModuleParams[1]);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new Exception("Error when parsing module action: " + base.ModuleParams[0], ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error when parsing module parameters: " + base.ModulePathInfo, ex);
                    }
                }
            }
        }

        private void CheckPhysicalDirectory(string physicalDir)
        {
            // Check existence
            if (!System.IO.Directory.Exists(physicalDir))
            {
                throw new Exception(String.Format("The Downloads module didn't find the physical directory {0} on the server.", physicalDir));
            }

            // Check if the diretory is writable
            if (!this._fileService.CheckIfDirectoryIsWritable(physicalDir))
            {
                throw new Exception(String.Format("The physical directory {0} is not writable.", physicalDir));
            }
        }
    }

    public enum DownloadsModuleActions
    {
        Download
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Castle.Services.Transaction;

using Cuyahoga.Core.Service.Files;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.DataAccess;
using ICSharpCode.SharpZipLib.Zip;

namespace Cuyahoga.Core.Service.SiteStructure
{
    [Transactional]
    public class ModuleTypeService : IModuleTypeService
    {
        private readonly ISiteStructureDao _siteStructureDao;
        private readonly ICommonDao _commonDao;
        private IFileService _fileService;

        private static readonly ICollection<string> RequiredDirectories = new[] { "bin/" };
        private static readonly ICollection<string> AllowedExtensions = new[] { ".ascx", ".aspx", ".css", ".dll", ".gif", ".png", ".jpg", ".js", ".swf", ".sql", ".xml", ".txt" };
        private static readonly ICollection<string> InstallDirs = new[] { @"Install/Database/mssql2000/", @"Install/Database/mysql/", @"Install/Database/postgresql/" };

        public ModuleTypeService(ISiteStructureDao siteStructureDao, ICommonDao commonDao, IFileService fileService)
        {
            this._siteStructureDao = siteStructureDao;
            this._commonDao = commonDao;
            _fileService = fileService;
        }

        #region IModuleService Members

        public IList<ModuleType> GetAllModuleTypesInUse()
        {
            return this._siteStructureDao.GetAllModuleTypesInUse();
        }

        public IList<ModuleType> GetAllModuleTypes()
        {
        	return this._commonDao.GetAll<ModuleType>();
        }

        public ModuleType GetModuleById(int moduleTypeId)
        {
            return this._commonDao.GetObjectById(typeof(ModuleType), moduleTypeId) as ModuleType;
        }

    	public ModuleType GetModuleByName(string moduleName)
    	{
    		return (ModuleType) this._commonDao.GetObjectByDescription(typeof (ModuleType), "Name", moduleName);
    	}

    	[Transaction(TransactionMode.RequiresNew)]
        public void SaveModuleType(ModuleType moduleType)
        {
            this._commonDao.SaveObject(moduleType);
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void SaveOrUpdateModuleType(ModuleType moduleType)
        {
            this._commonDao.SaveOrUpdateObject(moduleType);
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void DeleteModuleType(ModuleType moduleType)
        {
            this._commonDao.DeleteObject(moduleType);
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void ExtractModulePackage(string packageFilePath, Stream packageStream)
        {
            string moduleDir = Path.GetFileNameWithoutExtension(packageFilePath);
            string physicalModulesDirectory = Path.GetDirectoryName(packageFilePath);
            string physicalTargetModulesDir = Path.Combine(physicalModulesDirectory, moduleDir);
            string binaryDir = System.Web.HttpContext.Current.Server.MapPath("~/"); // The zip file has the bin folder path in the zipEntry

            bool isValidPack = false;

            bool hasBin = false;
            bool hasValidDll = false;
            bool hasAsx = false;
            bool passedExtCheck = true;
            bool foundInstallScripts = false;

            ZipFile moduleArchive = new ZipFile(packageStream);
            foreach (ZipEntry zipEntry in moduleArchive)
            {
                string targetFilePath = Path.Combine(physicalTargetModulesDir, zipEntry.Name);
                string extension = Path.GetExtension(targetFilePath);

                if (zipEntry.IsDirectory)
                {
                    // check has bin directory
                    if (zipEntry.Name.Equals("bin/")) 
                        hasBin = true;
                }

                if (zipEntry.IsFile)
                { 
                    // check file extension allowed
                    if (!AllowedExtensions.Contains(extension))
				    {
                        passedExtCheck = false;
				    }
				    // ascx control should be in the root of the template dir
                    string ttfp = Path.GetDirectoryName(targetFilePath);
                    if (extension == ".ascx" && (ttfp.Substring(ttfp.Length - moduleDir.Length, moduleDir.Length).Equals(moduleDir)))
                    {
                        hasAsx = true;
                    }
                    // has got at least one install script
                    if (extension == ".sql")
                    {
                        foreach (string installDir in InstallDirs)
                        {
                            string tfp = Path.GetDirectoryName(targetFilePath).ToLower();
                            string idir = Path.GetDirectoryName(installDir).ToLower();
                            if (tfp.IndexOf(idir) > -1)
                            {
                                foundInstallScripts = true;
                            }
                        }
                    }
                    // .dll filename obays convention
                    if (extension == ".dll")
                    {
                        string dllname = string.Format("bin/Cuyahoga.Modules.{0}.dll", moduleDir).ToLower();
                        hasValidDll = dllname.Equals(zipEntry.Name.ToLower());
                    }

				}
            }

            //Evaluate basic tests
            isValidPack = (hasBin 
                && hasAsx 
                && passedExtCheck 
                && foundInstallScripts 
                && hasValidDll);

            //If Passed basic tests
            if (isValidPack)
            {
                if (!Directory.Exists(physicalTargetModulesDir))
                {
                    this._fileService.CreateDirectory(physicalTargetModulesDir);
                }
                // Extract files
                foreach (ZipEntry zipEntry in moduleArchive)
                {
                    if (zipEntry.IsDirectory)
                    {
                        if (!zipEntry.Name.Equals("bin/"))
                            this._fileService.CreateDirectory(Path.Combine(physicalTargetModulesDir, zipEntry.Name));
                    }
                    if (zipEntry.IsFile)
                    {
                        string targetFilePath = Path.Combine(physicalTargetModulesDir, zipEntry.Name);
                        string extension = Path.GetExtension(targetFilePath);

                        if (!extension.Equals(".dll"))
                        {
                            //Not a .dll so unpack to module folder
                            this._fileService.WriteFile(targetFilePath, moduleArchive.GetInputStream(zipEntry));
                        }
                        else
                        {
                            //A .dll so unpack to bin folder
                            targetFilePath = Path.Combine(binaryDir, zipEntry.Name);
                            this._fileService.WriteFile(targetFilePath, moduleArchive.GetInputStream(zipEntry));
                        }
                    }
                }
            }
            else 
            {
                if(hasBin == false)
                    throw new InvalidPackageException("ModuleBinaryNotFoundException");

                if(hasAsx == false)
                    throw new InvalidPackageException("ControlInRootNotFoundException");

                if(passedExtCheck == false)
                    throw new InvalidPackageException("InvalidFileExtensionInPackageFoundException");

                if(foundInstallScripts == false)
                    throw new InvalidPackageException("InvalidInstallScriptsInPackageFoundException");

                if (hasValidDll == false)
                    throw new InvalidPackageException("InvalidDLLNameInPackageFoundException");
            }

        }

        #endregion
    }
}

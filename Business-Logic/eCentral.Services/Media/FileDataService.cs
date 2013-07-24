using System;
using System.IO;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain.Media;
using eCentral.Services.Configuration;
using eCentral.Services.Events;
using eCentral.Services.Logging;

namespace eCentral.Services.Media
{
    /// <summary>
    /// File data service
    /// </summary>
    public partial class FileDataService : IFileDataService
    {
        #region Fields

        private static readonly object s_lock = new object();

        private readonly IRepository<FileMetaData> fileRepository;
        private readonly ISettingService settingService;
        private readonly IWebHelper webHelper;
        private readonly ILogger logger;
        private readonly IEventPublisher eventPublisher;
        private readonly MediaSettings mediaSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fileRepository">File repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="logger">Logger</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="mediaSettings">Media settings</param>
        public FileDataService(IRepository<FileMetaData> fileRepository,           
            ISettingService settingService, IWebHelper webHelper,
            ILogger logger, IEventPublisher eventPublisher,
            MediaSettings mediaSettings)
        {
            this.fileRepository             = fileRepository;
            this.settingService           = settingService;
            this.webHelper                = webHelper;
            this.logger                   = logger;
            this.eventPublisher           = eventPublisher;
            this.mediaSettings            = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        public virtual string GetDefaultUrl(FileType defaultFileType)
        {
            string defaultfileName = string.Empty;
            switch (defaultFileType)
            {
                case FileType.CompanyLogo:
                    defaultfileName = settingService.GetByKey("Media.DefaultCompanyLogo", "default.gif");
                    break;
            }

            string relPath = "{0}{1}{2}".FormatWith(
                webHelper.AbsoluteWebRoot.ToString(), this.VirtualPath(defaultFileType) , defaultfileName);

            return relPath;
        }

        /// <summary>
        /// Loads a file
        /// </summary>
        /// <param name="fileId">FileMetaData identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>binary data</returns>
        public virtual byte[] LoadFromFile(Guid fileId, string mimeType, FileType fileType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string localFilename = string.Format("{0}.{1}", fileId.ToString(), lastPart);
            if (!File.Exists(Path.Combine(LocalPath(fileType), localFilename)))
                return new byte[0];
            return File.ReadAllBytes(Path.Combine(LocalPath(fileType), localFilename));
        }

        /// <summary>
        /// Gets the loaded binary depending on storage settings
        /// </summary>
        /// <param name="FileMetaData">FileMetaData</param>
        /// <returns>File binary</returns>
        public virtual byte[] LoadBinary(FileMetaData fileData)
        {
            return LoadBinary(fileData, this.StoreInDb);
        }

        /// <summary>
        /// Gets the loaded binary depending on storage settings
        /// </summary>
        /// <param name="FileMetaData">FileMetaData</param>
        /// <param name="fromDb">Load from database; otherwise, from file system</param>
        /// <returns>File binary</returns>
        public virtual byte[] LoadBinary(FileMetaData fileData, bool fromDb)
        {
            if (fileData == null)
                throw new ArgumentNullException("fileData");

            byte[] result = null;
            if (fromDb)
                result = fileData.BinaryData;
            else
                result = LoadFromFile(fileData.RowId, fileData.MimeType, (FileType)fileData.FileType);
            return result;
        }

        /// <summary>
        /// Get a file URL
        /// </summary>
        /// <param name="fileId">FileMetaData identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns>URL</returns>
        public virtual string GetFileUrl(Guid fileId, bool showDefaultPicture = true)
        {
            var fileData = GetFileById(fileId);
            return GetFileUrl(fileData, showDefaultPicture);
        }

        /// <summary>
        /// Get a file URL
        /// </summary>
        /// <param name="fileData">FileMetaData instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns>URL</returns>
        public virtual string GetFileUrl(FileMetaData fileData, bool showDefaultPicture = true)
        {
            string url = string.Empty;
            byte[] dataBinary = null;
            if (fileData != null)
                dataBinary = LoadBinary(fileData);
            if (fileData == null || dataBinary == null || dataBinary.Length == 0)
            {
                if (showDefaultPicture)
                {
                    url = GetDefaultUrl( (FileType)fileData.FileType );
                }
                return url;
            }

            string lastPart = GetFileExtensionFromMimeType(fileData.MimeType);
            string localFilename;
            if (fileData.IsNew)
            {
                //we do not validate binary here to ensure that no exception ("Parameter is not valid") will be thrown
                fileData = UpdateFile(fileData.RowId,
                    dataBinary,
                    fileData.MimeType,
                    fileData.SeoFilename, 
                    false);
            }
            lock (s_lock)
            {
                string seoFileName = fileData.SeoFilename;
                localFilename = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}.{2}", fileData.RowId.ToString(), seoFileName, lastPart) :
                        string.Format("{0}.{1}", fileData.RowId.ToString(), lastPart);
            }
            
            url = "{0}{1}{2}".FormatWith(
                    webHelper.AbsoluteWebRoot.ToString(), this.VirtualPath((FileType)fileData.FileType), localFilename);
            return url;
        }

        /// <summary>
        /// Get a file local path
        /// </summary>
        /// <param name="FileMetaData">FileMetaData instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public virtual string GetLocalPath(FileMetaData fileData, bool showDefaultPicture = true)
        {
            string url = GetFileUrl(fileData, showDefaultPicture);
            if (String.IsNullOrEmpty(url))
                return String.Empty;
            else
                return Path.Combine(this.LocalPath((FileType) fileData.FileType ), Path.GetFileName(url));
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="fileData">FileMetaData</param>
        public virtual void DeleteFile(FileMetaData fileData)
        {
            if (fileData == null)
                throw new ArgumentNullException("fileData");

            //delete from file system
            if (!this.StoreInDb)
                DeleteOnFileSystem(fileData);

            //delete from database
            fileRepository.Delete(fileData);

            //event notification
            eventPublisher.EntityDeleted(fileData);
        }

        /// <summary>
        /// Gets a collection of files
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        public virtual IPagedList<FileMetaData> GetFiles(int pageIndex, int pageSize)
        {
            var query = from f in fileRepository.Table
                        select f;
            var files = new PagedList<FileMetaData>(query, pageIndex, pageSize);
            return files;
        }

        /// <summary>
        /// Inserts a file
        /// </summary>
        /// <param name="dataBinary">The file binary</param>
        /// <param name="mimeType">The file MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the file is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided binary</param>
        /// <returns>Picture</returns>
        public virtual FileMetaData InsertFile(byte[] dataBinary, string mimeType, string seoFilename, bool isNew, FileType fileType)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            var fileData = new FileMetaData()
            {
                BinaryData = this.StoreInDb ? dataBinary : new byte[0],
                MimeType = mimeType,
                SeoFilename = seoFilename,
                FileType = (int)fileType,
                IsNew = isNew,
            };
            fileRepository.Insert(fileData);

            if (!this.StoreInDb)
                SaveInFile(fileData.RowId, dataBinary, mimeType, fileType);

            //event notification
            eventPublisher.EntityInserted(fileData);

            return fileData;
        }

        /// <summary>
        /// Updates the file
        /// </summary>
        /// <param name="fileId">The file identifier</param>
        /// <param name="pictureBinary">The file binary</param>
        /// <param name="mimeType">The file MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>Picture</returns>
        public virtual FileMetaData UpdateFile(Guid fileId, byte[] dataBinary, string mimeType, string seoFilename, bool isNew)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            var fileData = GetFileById(fileId);
            if (fileData == null)
                return null;

            fileData.BinaryData = (this.StoreInDb ? dataBinary : new byte[0]);
            fileData.MimeType = mimeType;
            fileData.SeoFilename = seoFilename;
            fileData.IsNew = isNew;

            fileRepository.Update(fileData);

            if (!this.StoreInDb)
                SaveInFile(fileData.RowId, dataBinary, mimeType, (FileType)fileData.FileType);

            //event notification
            eventPublisher.EntityUpdated(fileData);

            return fileData;
        }

        /// <summary>
        /// Updates a SEO filename of a file
        /// </summary>
        /// <param name="fileId">The file identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>FileMetaData</returns>
        public virtual FileMetaData SetSeoFilename(Guid fileId, string seoFilename)
        {
            var fileData = GetFileById(fileId);
            if (fileData == null)
                throw new ArgumentException("No file found with the specified id");

            //update if it has been changed
            if (seoFilename != fileData.SeoFilename)
            {
                //update picture
                fileData = UpdateFile(fileData.RowId, LoadBinary(fileData), fileData.MimeType, seoFilename, true);
            }
            return fileData;
        }

        /// <summary>
        /// Gets a file
        /// </summary>
        /// <param name="fileId">FileMetaData identifier</param>
        /// <returns>FileMetaData</returns>
        public virtual FileMetaData GetFileById(Guid fileId)
        {
            if (fileId.IsEmpty())
                return null;

            var fileData = fileRepository.GetById(fileId);
            return fileData;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Delete a file on the physcial system
        /// </summary>
        /// <param name="fileData">FileMetaData</param>
        protected virtual void DeleteOnFileSystem(FileMetaData fileData)
        {
            if (fileData == null)
                throw new ArgumentNullException("fileData");

            string lastPart = GetFileExtensionFromMimeType(fileData.MimeType);
            string localFilename = string.Format("{0}.{1}", fileData.RowId.ToString(), lastPart);
            string localFilepath = Path.Combine(this.LocalPath((FileType)fileData.FileType), localFilename);
            if (File.Exists(localFilepath))
            {
                File.Delete(localFilepath);
            }
        }

        /// <summary>
        /// Save file on physical system
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="dataBinary">File binary</param>
        /// <param name="mimeType">MIME type</param>
        protected virtual void SaveInFile(Guid fileId, byte[] dataBinary, string mimeType, FileType fileType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string localFilename = string.Format("{0}.{1}", fileId.ToString(), lastPart);
            File.WriteAllBytes(Path.Combine(this.LocalPath(fileType), localFilename), dataBinary);
        }

        /// <summary>
        /// Returns the file extension from mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>File extension</returns>
        protected virtual string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            return lastPart;
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Gets the local image path
        /// </summary>
        protected virtual string LocalPath( FileType fileType)
        {
            return webHelper.MapPath("~/{0}"
                .FormatWith(VirtualPath(fileType)));
        }

        /// <summary>
        /// Gets the local image path
        /// </summary>
        protected virtual string VirtualPath( FileType fileType)
        {
            var path = string.Empty;

            switch ( fileType)
            {
                case FileType.CompanyLogo:
                    path = "images/companies/";
                    break;
            }

            return "library/user-content/{0}"
                .FormatWith(path);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        public virtual bool StoreInDb
        {
            get
            {
                return settingService.GetByKey<bool>("Media.Images.StoreInDB", false);
            }
            set
            {
                //check whether it's a new value
                if (this.StoreInDb != value)
                {
                    //save the new setting value
                    settingService.Set<bool>("Media.Images.StoreInDB", value);

                    //update all file data objects
                    var files = this.GetFiles(0, int.MaxValue);
                    foreach (var file in files)
                    {
                        var dataBinary = LoadBinary(file, !value);

                        //delete from file system
                        if (value)
                            DeleteFile(file);

                        //just update a (all required logic is in UpdatePicture method)
                        UpdateFile(file.RowId,
                                      dataBinary,
                                      file.MimeType,
                                      file.SeoFilename,
                                      true);
                        //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                    }
                }
            }
        }

        #endregion 
    }
}

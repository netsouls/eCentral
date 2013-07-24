using System;
using eCentral.Core;
using eCentral.Core.Domain.Media;

namespace eCentral.Services.Media
{
    /// <summary>
    /// Picture service interface
    /// </summary>
    public partial interface IFileDataService
    {
        #region Methods

        /// <summary>
        /// Gets the default URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        string GetDefaultUrl(FileType defaultFileType);

        /// <summary>
        /// Loads a file
        /// </summary>
        /// <param name="fileId">FileMetaData identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>binary data</returns>
        byte[] LoadFromFile(Guid fileId, string mimeType, FileType fileType);

        /// <summary>
        /// Gets the loaded binary depending on storage settings
        /// </summary>
        /// <param name="FileMetaData">FileMetaData</param>
        /// <returns>File binary</returns>
        byte[] LoadBinary(FileMetaData fileData);

        /// <summary>
        /// Gets the loaded binary depending on storage settings
        /// </summary>
        /// <param name="FileMetaData">FileMetaData</param>
        /// <param name="fromDb">Load from database; otherwise, from file system</param>
        /// <returns>File binary</returns>
        byte[] LoadBinary(FileMetaData fileData, bool fromDb);

        /// <summary>
        /// Get a file URL
        /// </summary>
        /// <param name="fileId">FileMetaData identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns>URL</returns>
        string GetFileUrl(Guid fileId, bool showDefaultPicture = true);

        /// <summary>
        /// Get a file URL
        /// </summary>
        /// <param name="fileData">FileMetaData instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns>URL</returns>
        string GetFileUrl(FileMetaData fileData, bool showDefaultPicture = true);

        /// <summary>
        /// Get a file local path
        /// </summary>
        /// <param name="FileMetaData">FileMetaData instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        string GetLocalPath(FileMetaData fileData, bool showDefaultPicture = true);

        /// <summary>
        /// Gets a file
        /// </summary>
        /// <param name="fileId">FileMetaData identifier</param>
        /// <returns>FileMetaData</returns>
        FileMetaData GetFileById(Guid fileId);

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="fileData">FileMetaData</param>
        void DeleteFile(FileMetaData fileData);

        /// <summary>
        /// Gets a collection of files
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        IPagedList<FileMetaData> GetFiles(int pageIndex, int pageSize);

        /// <summary>
        /// Inserts a file
        /// </summary>
        /// <param name="dataBinary">The file binary</param>
        /// <param name="mimeType">The file MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the file is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided binary</param>
        /// <returns>Picture</returns>
        FileMetaData InsertFile(byte[] dataBinary, string mimeType, string seoFilename, bool isNew, FileType defaultFileType);

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
        FileMetaData UpdateFile(Guid fileId, byte[] dataBinary, string mimeType, string seoFilename, bool isNew);

        /// <summary>
        /// Updates a SEO filename of a file
        /// </summary>
        /// <param name="fileId">The file identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>FileMetaData</returns>
        FileMetaData SetSeoFilename(Guid fileId, string seoFilename);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        bool StoreInDb { get; set; }

        #endregion
    }
}

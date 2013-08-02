using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using eCentral.Core.Domain.Media;
using eCentral.Core.Domain.Users;
using eCentral.Services.Media;
using eCentral.Web.Framework.Controllers;

namespace eCentral.Web.Controllers
{
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    public partial class MediaController : BaseController
    {
        private readonly IFileDataService fileDataService;
        
        public MediaController(IFileDataService fileDataService)
        {
            this.fileDataService = fileDataService;
        }

        [HttpPost]
        public ActionResult AsyncUpload(int fileType)
        {
            Stream stream = null;
            var fileName = "";
            var contentType = "";

            HttpPostedFileBase httpPostedFile = Request.Files[0];
            if (httpPostedFile == null)
                throw new ArgumentException("No file uploaded");

            stream = httpPostedFile.InputStream;
            fileName = Path.GetFileName(httpPostedFile.FileName);
            contentType = httpPostedFile.ContentType;

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);
            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = "image/tiff";
                        break;
                    default:
                        break;
                }
            }

            var fileData = fileDataService.InsertFile(fileBinary, contentType, null, true, (FileType)fileType );

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                fileId = fileData.RowId.ToString(),
                fileUrl = fileDataService.GetFileUrl(fileData)
            },
                "text/plain");
        }
    }
}

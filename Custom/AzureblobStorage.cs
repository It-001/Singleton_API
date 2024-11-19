using Sandboxable.Microsoft.WindowsAzure.Storage;
using Sandboxable.Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

namespace ClinicalTrialAPI.Custom
{
    public class AzureblobStorage
    {
        #region upload file on azure blob server
        static void UploadStart(object args)
        {
            UploadFilesEntity _eE = (UploadFilesEntity)args;
            UploadNowFile(_eE);
        }
        public void UploadFilesAzureCCDA(string ContainerName, byte[] imageData, string RestUrl, string FileExtension)
        {
            Thread thrd = new Thread(new ParameterizedThreadStart(UploadStart));
            UploadFilesEntity _eE = new UploadFilesEntity();
            _eE.ContainerName = ContainerName;
            _eE.imagedata = imageData;
            _eE.RestUrl = RestUrl;
            _eE.ContentType = ContentTypeFun(FileExtension);
            thrd.Start(_eE);
        }

        public string UploadFilesAzureCCD( byte[] Data, string RestUrl, string FileExtension)
        {
            string ContainerName = System.Web.Configuration.WebConfigurationManager.AppSettings["BlobContainer"].ToString();
            Thread thrd = new Thread(new ParameterizedThreadStart(UploadStart));
            UploadFilesEntity _eE = new UploadFilesEntity();
            _eE.ContainerName = ContainerName;
            _eE.imagedata = Data;
            _eE.RestUrl = RestUrl;
            _eE.ContentType = ContentTypeFun(FileExtension);
            thrd.Start(_eE);
            return ContainerName + "/" + RestUrl;
        }

        private static void UploadNowFile(UploadFilesEntity _eE)
        {
            try
            {
                
                string connection = ConfigurationManager.ConnectionStrings["storageblob"].ConnectionString;
                CloudStorageAccount sa = CloudStorageAccount.Parse(connection);
                CloudBlobClient blob = sa.CreateCloudBlobClient();
                CloudBlobContainer container = blob.GetContainerReference(_eE.ContainerName);
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                var guidPart = System.Guid.NewGuid().ToString().Substring(0, 6);
                CloudBlockBlob blockBlobdoctor = container.GetBlockBlobReference(_eE.RestUrl);
                blockBlobdoctor.Properties.ContentType = _eE.ContentType;
                blockBlobdoctor.UploadFromByteArray(_eE.imagedata, 0, _eE.imagedata.Length);
            }
            catch (Exception ex)
            {
            }

        }

        #region contentType

        public string ContentTypeFun(string FileExtension)
        {
            string Contenttype = string.Empty;
            if (FileExtension == ".jpg")
                Contenttype = "image/jpg";
            else if (FileExtension == ".pdf")
                Contenttype = "application/pdf";
            else if (FileExtension == ".jpeg")
                Contenttype = "image/jpeg";
            else if (FileExtension == ".png")
                Contenttype = "image/png";
            else if (FileExtension == ".gif")
                Contenttype = "image/gif";
            else if (FileExtension == ".txt")
                Contenttype = "text/plain";
            else if (FileExtension == ".doc")
                Contenttype = "application/msword";
            else if (FileExtension == ".docx")
                Contenttype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            else if (FileExtension == ".xls" || FileExtension == ".xlt" || FileExtension == ".xlm")
                Contenttype = "application/vnd.ms-excel";
            else if (FileExtension == ".xlsx")
                Contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return Contenttype;
        }
        #endregion
        #endregion
    }

    public class UploadFilesEntity
    {
        public string ContainerName { get; set; }
        public byte[] imagedata { get; set; }
        public string RestUrl { get; set; }
        public string ContentType { get; set; }

    }

}
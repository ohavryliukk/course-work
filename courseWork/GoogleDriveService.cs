using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace courseWork
{
    class GoogleDriveService
    {
        private UserCredential credential;
        private DriveService driveService;

        public GoogleDriveService(string jsonSecretPath, string appName)
        {
            GetCredential(jsonSecretPath);
            CreateDriveService(appName);
        }

        private void GetCredential(string clientSecretPath)
        {
            using (var filestream = new FileStream(clientSecretPath,
                FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(filestream).Secrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("DriveCommandLineSample")).Result;
            }
        }

        private void CreateDriveService(string applicationName)
        {
            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }

        private void uploadDocumentToDrive(ref byte[] file, string title, string mimetype)
        {
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = title;
            body.MimeType = mimetype;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(file))
            {
                if (driveService != null)
                {
                    FilesResource.InsertMediaUpload request = driveService.Files.Insert(body, stream, mimetype);
                    request.Upload();
                    Google.Apis.Drive.v2.Data.File uploadedFile = request.ResponseBody;
                    System.Diagnostics.Debug.WriteLine("Uploaded file: {0} with ID: {1}",
                        uploadedFile.Title,
                        uploadedFile.Id);
                }
            }
        }

        public bool UploadFile(ref byte[] file, string title, string mimetype)
        {
            bool uploaded = false;
            if (credential != null)
            {
                uploadDocumentToDrive(ref file,title, mimetype);
                uploaded = true;
            }
            return uploaded;
        }      
    }
}

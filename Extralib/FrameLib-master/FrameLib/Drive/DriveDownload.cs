using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace FrameLib6.Drive
{
    public class DriveDownload
    {

        public static string DriveDownloadFile(string authPath, string saveFilePath, string name)
        {
            string fullFilePath = null;
            try
            {

                // Create Drive API service.
                UserCredential credential;
                string[] Scopes = { DriveService.Scope.Drive };
                using (var fileStream = new FileStream(authPath, FileMode.Open, FileAccess.Read))
                {
                    string credentialPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(fileStream).Secrets,
                         Scopes,
                         "user",
                         CancellationToken.None,
                         new FileDataStore(credentialPath, true)).Result;
               
                }
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                });
                var listSearchFiles = DriveList.ListFiles(service, new DriveList.FilesListOptionalParms() {Q = $"name contains '{name}'" });

                string saveTo = saveFilePath; //"D:/testmagik"

                var request = service.Files.Get(listSearchFiles.Files[0].Id);
                string fileName = request.Execute().Name;
                string fileExtension = Path.GetExtension(fileName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string uniqueFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";
                fullFilePath = Path.Combine(saveTo, uniqueFileName);

                var stream = new FileStream(fullFilePath, FileMode.CreateNew, FileAccess.Write);

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                    progress =>
                    {
                        switch (progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                {
                                    Console.WriteLine($"{progress.BytesDownloaded * 0.001} KB");
                                    break;
                                }
                            case DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    stream.Flush();
                                    stream.Close();
                                    break;
                                }
                            case DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    break;
                                }
                        }
                    };
                request.Download(stream);
                stream.Close();
                return fullFilePath;

            }
            catch (Exception e)
            {
                // TODO(developer) - handle error appropriately
                if (e is AggregateException)
                {
                    Console.WriteLine("Credential Not found");
                }
                else
                {
                    throw;
                }
            }
            return fullFilePath;
        }

    }
}

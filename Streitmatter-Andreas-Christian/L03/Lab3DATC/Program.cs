using System;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Lab3DATC
{
    class Program
    {
        private static DriveService _service;
        private static string _token;
        static void Main(string[] args)
        {
            Initialize();
        }
        static void Initialize()
        {
            string[] scopes = new string[]{
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };
            var clientID = "59239618615-5lhuggm2g2sojutv6027jc860uj41aep.apps.googleusercontent.com";   
            var clientSecret = "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";

            var credential= GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId=clientID,
                    ClientSecret=clientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,

                new FileDataStore("Daimto.GoogleDrive.Auth.Store3")
            ).Result;

            _service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            _token=credential.Token.AccessToken;

            Console.Write("Token: " + credential.Token.AccessToken);

            GetMyFiles();
        }
        static void GetMyFiles()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents");
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + _token);

            using (var response = request.GetResponse())
            {
                using (Stream data = response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    string text= reader.ReadToEnd();
                    var myData=JObject.Parse(text);
                    foreach (var file in myData["files"])
                    {
                        // Console.WriteLine(file);
                        if (file["mimeType"].ToString() != "application/vnd.google-apps.folder")
                        {
                            Console.WriteLine("File name: " + file["name"]);
                        }
                    }
                }
            }
        }
    }
}

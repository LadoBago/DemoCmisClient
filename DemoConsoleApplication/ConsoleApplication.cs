using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PortCMIS;
using PortCMIS.Client;
using PortCMIS.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DemoConsoleApplication
{
    public class ConsoleApplication : IApplication
    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ISession _cmisSession;

        public ConsoleApplication(IHostEnvironment environment,
            IConfiguration configuration,
            ISession cmisSession
        )
        {
            _environment = environment;
            _configuration = configuration;
            _cmisSession = cmisSession;
        }
        public async Task Run()
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(_cmisSession.RepositoryInfo.ToString());

            DirectoryInfo dInfo = new DirectoryInfo("UploadFiles");
            var rootFolder = _cmisSession.GetRootFolder();

            Console.WriteLine($"Refreshing folder {rootFolder.Name}");
            rootFolder.Refresh();

            foreach (var child in rootFolder.GetChildren())
            {
                Console.WriteLine($"Deleting from folder {rootFolder.Name} -> {child.ObjectType.DisplayName}:{child.Name}");
                child.Delete();
            }

            foreach (var fInfo in dInfo.GetFiles())
            {
                var contentStream = _cmisSession.ObjectFactory.CreateContentStream(filename: fInfo.Name, fInfo.Length, "text/plain; charset=UTF-8", fInfo.OpenRead());
                const string objectType_Document = "cmis:document";
                var properties = new Dictionary<string, object>
                {
                    [PropertyIds.Name] = fInfo.Name,
                    [PropertyIds.ObjectTypeId] = objectType_Document
                };

                Console.WriteLine($"Uploading to folder {rootFolder.Name} -> {objectType_Document}:{fInfo.Name}");
                var newDoc = rootFolder.CreateDocument(properties, contentStream, VersioningState.None);
            }

            await Task.CompletedTask;
        }

    }
}

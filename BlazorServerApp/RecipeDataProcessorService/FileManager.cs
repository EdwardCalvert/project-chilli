using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MD5;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using DataLibrary;

namespace BlazorServerApp.proccessService
{
    public class FileManager :IFileManger
    {
        private const string rootPath = "wwwroot\\FileManager";
        private IWebHostEnvironment _environment;
        private IDataAccess _dataAccess;


        public FileManager(IWebHostEnvironment Environment, IDataAccess dataAccess)
        {
            _environment = Environment;
            _dataAccess = dataAccess;
            if(!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public async Task<(int,string)> InsertFile(IBrowserFile file, int maxFileSizeInBytes)
        {
            try
            {
                string fileAsString;
                using (StreamReader streamReader = new StreamReader(file.OpenReadStream(maxFileSizeInBytes)))
                {
                    fileAsString = await streamReader.ReadToEndAsync();
                }
                CustomMD5 customMD5 = new CustomMD5();
                string md5AsHex = customMD5.Run(fileAsString);
                var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads",
                        md5AsHex);
                if (Directory.Exists(path))
                {
                    return (898,""); 
                }
                Directory.CreateDirectory(path);
                await using FileStream fs = new(Path.Combine(path, file.Name), FileMode.Create);
                await file.OpenReadStream(maxFileSizeInBytes).CopyToAsync(fs);
                return (1, md5AsHex);
                        
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return (-1,"");
            }
            }



        public string GetFilePath(string MD5Hash) {
            return "";
        }


        public void CreateTemporaryFile()
        {

        }
    }

    public interface IFileManger
    {
        public string GetFilePath(string MD5Hash);
        public Task<(int,string)> InsertFile(IBrowserFile browserFiles, int maxFileSizeInBytes);
        public const long MAXFILESIZE = 1024 * 15;
        public const int MAXALLOWEDFILES = 3;
    }
}

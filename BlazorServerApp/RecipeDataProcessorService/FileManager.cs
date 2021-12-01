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
        private IWebHostEnvironment _environment;
        private IDataAccess _dataAccess;


        public FileManager(IWebHostEnvironment Environment, IDataAccess dataAccess)
        {
            _environment = Environment;
            _dataAccess = dataAccess;
            if(!Directory.Exists(Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads")))
            {
                Directory.CreateDirectory(Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads"));
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
                var path = AbsolutFilePathFromHash(md5AsHex);
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

        private string AbsolutFilePathFromHash(string MD5Hash)
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads", MD5Hash);
        }

        public string GetFilePath(string MD5Hash) 
        {
            string[] files = Directory.GetFiles(AbsolutFilePathFromHash(MD5Hash));
            if(files.Length == 1)
            {
                return files[0];
            }
            else
            {
                throw new Exception("Catastrophic error: File folder integrity lost");
            }
        }

        public void DeleteFile(string MD5Hash)
        {
            string path = AbsolutFilePathFromHash(MD5Hash);
            if (Directory.Exists(path))
            {
                foreach(string filePath in Directory.GetFiles(path))
                {
                    File.Delete(filePath);
                }
                Directory.Delete(path);
            }
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
        public void DeleteFile(string MD5Hash);
    }
}

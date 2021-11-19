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

namespace BlazorServerApp.Data
{
    public class FileManager :IFileManger
    {
        private const string rootPath = "wwwroot\\FileManager";
        private IWebHostEnvironment _environment;
        private IDataAccess _dataAccess;

        public const long MAXFILESIZE = 1024 * 15*1024;
        public const int MAXALLOWEDFILES = 3;

        public FileManager(IWebHostEnvironment Environment, IDataAccess dataAccess)
        {
            _environment = Environment;
            _dataAccess = dataAccess;
            if(!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public async Task<int> InsertFile(IBrowserFile file, uint recipeID)
        {
            try
            {
                string fileAsString;
                using (StreamReader streamReader = new StreamReader(file.OpenReadStream(MAXFILESIZE)))
                {
                    fileAsString = await streamReader.ReadToEndAsync();
                }
                CustomMD5 customMD5 = new CustomMD5();
                string md5AsHex = customMD5.Run(fileAsString);
                var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads",
                        md5AsHex);
                if (Directory.Exists(path))
                {
                    //throw new Exception("File already uploaded");
                    return 898; //Code for file uploaded previously.
                }
                Directory.CreateDirectory(path);
                await using FileStream fs = new(Path.Combine(path, file.Name), FileMode.Create);
                await file.OpenReadStream(MAXFILESIZE).CopyToAsync(fs);
            return 1;
                        
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return -1;
            }
            }
                //var trustedFileNameForFileStorage = ;

                //var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads",
                //        trustedFileNameForFileStorage);

                //await using FileStream fs = new(path, FileMode.Create);
                //await file.OpenReadStream(maxFileSize).CopyToAsync(fs);



        public string GetFilePath(uint RecipeID) {
            return "";
        }


        public void CreateTemporaryFile()
        {

        }
    }

    public interface IFileManger
    {
        public string GetFilePath(uint RecipeID);
        public Task<int> InsertFile(IBrowserFile browserFiles, uint RecipeID);
        public const long MAXFILESIZE = 1024 * 15;
        public const int MAXALLOWEDFILES = 3;
    }
}

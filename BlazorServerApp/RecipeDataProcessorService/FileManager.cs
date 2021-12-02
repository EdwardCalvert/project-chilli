using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MD5;
using BlazorServerApp.Models;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using DataLibrary;

namespace BlazorServerApp.proccessService
{
    public class FileManager : IFileManger
    {
        private IWebHostEnvironment _environment;
        private IRecipeDataLoader _dataLoader;


        public FileManager(IWebHostEnvironment Environment, IRecipeDataLoader dataLoader)
        {
            _environment = Environment;
            _dataLoader = dataLoader;
            if (!Directory.Exists(Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads")))
            {
                Directory.CreateDirectory(Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads"));
            }
        }

        public async Task<(int, string)> InsertFile(IBrowserFile file, int maxFileSizeInBytes)
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
                    return (898, "");
                }
                Directory.CreateDirectory(path);
                await using FileStream fs = new(Path.Combine(path, file.Name), FileMode.Create);
                await file.OpenReadStream(maxFileSizeInBytes).CopyToAsync(fs);
                return (1, md5AsHex);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return (-1, "");
            }
        }

        private string AbsolutFilePathFromHash(string MD5Hash)
        {
            return Path.Combine(AbsoluteRoot(), MD5Hash);
        }

        private string AbsoluteRoot()
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads");
        }

        public string GetFilePath(string MD5Hash)
        {
            string[] files = Directory.GetFiles(AbsolutFilePathFromHash(MD5Hash));
            if (files.Length == 1)
            {
                return files[0];
            }
            else
            {
                throw new Exception("Catastrophic error: File folder integrity lost");
            }
        }

        public string GetFileName(string MD5Hash)
        {
            string absoulutePath = GetFilePath(MD5Hash);
            Uri uri1 = new Uri(AbsolutFilePathFromHash(MD5Hash) + "\\");
            Uri uri2 = new Uri(absoulutePath);
            return uri1.MakeRelativeUri(uri2).ToString();
        }

        public async Task<string> GetURL(uint recipeID)
        {
            FileManagerModel fileManagerModel = await _dataLoader.GetFile(recipeID);
            if (fileManagerModel != null)
            {
                string absoulutePath = GetFilePath(fileManagerModel.FileID);
                Console.WriteLine(absoulutePath);
                Uri uri1 = new Uri(AbsoluteRoot());
                Uri uri2 = new Uri(absoulutePath);
                return uri1.MakeRelativeUri(uri2).ToString();
            }
            return null;
        }
        public void DeleteFile(string MD5Hash)
        {
            string path = AbsolutFilePathFromHash(MD5Hash);
            if (Directory.Exists(path))
            {
                foreach (string filePath in Directory.GetFiles(path))
                {
                    File.Delete(filePath);
                }
                Directory.Delete(path);
            }
        }

        public async Task CreateFileToRecipeRelationship(uint RecipeID, string MD5Hash)
        {
            FileManagerModel fileManagerModel = new();
            fileManagerModel.DateUploaded = DateTime.Now;
            fileManagerModel.LastAccessed = DateTime.Now;
            fileManagerModel.RecipeID = RecipeID;
            fileManagerModel.NumberOfViews = 1;
            fileManagerModel.FileID = MD5Hash;
            await _dataLoader.InsertFile(fileManagerModel);
        }
    }

    public interface IFileManger
    {
        public string GetFilePath(string MD5Hash);
        public Task<(int,string)> InsertFile(IBrowserFile browserFiles, int maxFileSizeInBytes);
        public const long MAXFILESIZE = 1024 * 15;
        public const int MAXALLOWEDFILES = 3;
        public void DeleteFile(string MD5Hash);
        public Task CreateFileToRecipeRelationship(uint RecipeID, string MD5Hash);
        public Task<string> GetURL(uint recipeID);
        public string GetFileName(string MD5Hash);
    }
}

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
        private const string pdfName = "print.pdf";
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

        public string RelativeFolderURL(string MD5)
        {
            return AbsolutFilePathFromHash( MD5);
        }

        private string AbsoluteRoot()
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot", "unsafe_uploads");
        }


        public string GetPdfPath(string MD5Hash)
        {
            return GetURLFromAbsolutePath(getPdfOrDocx(MD5Hash,false));
        }

        public string GetFilePath(string MD5Hash)
        {
            return getPdfOrDocx(MD5Hash, true);
        }

        private string getPdfOrDocx(string MD5Hash, bool docx)
        {
            string ending = docx ? ".docx" : ".pdf";
            string path = AbsolutFilePathFromHash(MD5Hash);
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                if (files.Length == 1 )
                {
                    if(files[0].EndsWith(ending))
                        return files[0];
                    else
                    {
                        throw new Exception($"A {ending} could not be found in the given directory. (out of 1 file)");
                    }
                }
                else if(files.Length == 2)
                {
                    if (files[0].EndsWith(ending))
                        return files[0];
                    else if (files[1].EndsWith(ending))
                    {
                        return files[1];
                    }
                    else
                    {
                        throw new Exception($"A {ending} could not be found in the given directory. (out of 2 file)");
                    }
                }
                else
                {
                    throw new Exception("Catastrophic error: File folder integrity lost");
                }
            }
            throw new Exception("It is likely that you deleted a file that was queued for processing, or previously on disk.");
        }

        public async Task<List<FileInfo>> GetAllFilesStoredOnDisk()
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach(string path in Directory.GetDirectories(AbsoluteRoot()))
            {
                string[] file = Directory.GetFiles(path);
                if(file.Length >= 1)
                {
                    FileInfo fileInfo = new FileInfo(file[0]);
                    files.Add(fileInfo);
                }
            }
            return files;
        }

        public string GetURLFromAbsolutePath(string absoulutePath)
        {
            Uri uri1 = new Uri(AbsoluteRoot());
            Uri uri2 = new Uri(absoulutePath);
            return uri1.MakeRelativeUri(uri2).ToString();
        }

        public string GetFileName(string MD5Hash)
        {
            string absoulutePath = GetFilePath(MD5Hash);
            FileInfo info = new FileInfo(absoulutePath);
            return info.Name;
        }
        public async Task<string> GetMD5FromRecipeID(uint RecipeID)
        {
            FileManagerModel model = await _dataLoader.GetFile(RecipeID);
            if (model != null)
            {
                return model.FileID;
            }
            else { return null; }
        }

        public async Task<string> GetURL(uint recipeID)
        {
            FileManagerModel fileManagerModel = await _dataLoader.GetFile(recipeID);
            if (fileManagerModel != null)
            {
                string absoulutePath = GetFilePath(fileManagerModel.FileID);
                return GetURLFromAbsolutePath(absoulutePath);
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
        /// <summary>
        /// </summary>
        /// <param name="browserFile"></param>
        /// <param name="maxFileSizeInBytes"></param>
        /// <returns>A tuple of integer, and MD5 Hash indicating the success of the insert</returns>
        public Task<(int,string)> InsertFile(IBrowserFile browserFile, int maxFileSizeInBytes);
        public const long MAXFILESIZE = 1024 * 15;
        public const int MAXALLOWEDFILES = 3;
        public void DeleteFile(string MD5Hash);
        public Task CreateFileToRecipeRelationship(uint RecipeID, string MD5Hash);
        public Task<string> GetURL(uint recipeID);
        public string GetFileName(string MD5Hash);
        public  Task<List<FileInfo>> GetAllFilesStoredOnDisk();
        public string GetURLFromAbsolutePath(string absoulutePath);
        public Task<string> GetMD5FromRecipeID(uint RecipeID);
        public string GetPdfPath(string MD5Hash);
        public string RelativeFolderURL(string MD5);
    }
}

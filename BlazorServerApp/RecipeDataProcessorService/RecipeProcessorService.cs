using BlazorServerApp.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorServerApp.proccessService
{
    public class RecipeProcessorService : IRecipeProcessorService
    {
        private readonly Timer _timer;
        private readonly CircularQueue<string> _recipesToProcess = new CircularQueue<string>(500);
        private readonly IFileManger _fileManager;
        private readonly IRecipeDataLoader _recipeDataLoader;

        public RecipeProcessorService(IFileManger fileManger, IRecipeDataLoader recipeDataLoader)
        {
            _fileManager = fileManger;
            _recipeDataLoader = recipeDataLoader;
            //_timer = new Timer(60000) { AutoReset = true };
            //_timer.Enabled = true;
            //_timer.Elapsed += TimerElapsed;
        }

        

        public int MaximumSingleFileSizeInBytes { get; } = 200000;

        public async Task<ResultCode> QueueBrowserFilesForProcessing(ResultCode resultCode)
        {
            List<IBrowserFile> validFiles = resultCode.GetIBrowserFiles(556);
            if (validFiles != null && validFiles.Count < _recipesToProcess.GetCapacity())
            {
                foreach (IBrowserFile browserFile in validFiles)
                {
                    (int intResult, string MD5Hash) = await _fileManager.InsertFile(browserFile, MaximumSingleFileSizeInBytes);
                    if (intResult == 1) //Only if the method was successful will a file be inserted
                    {
                        _recipesToProcess.EnqueueItem(MD5Hash);
                    }
                    resultCode.AddIBrowserFile(intResult, browserFile);
                }
            }
            else
            {
                resultCode.AddIBrowserFiles(670, validFiles);
            }
            return resultCode;
        }

        public bool FilesAreQueued()
        {
            return !_recipesToProcess.QueueIsEmpty();
        }

        public int GetCurrentQueueCapacity()
        {
            return _recipesToProcess.GetCapacity();
        }

        private async Task SortOutFiles()
        {
            if (!_recipesToProcess.QueueIsEmpty())
            {
                string MD5ToProcess = _recipesToProcess.DequeueItem();

                Recipe recipe = new Recipe();
                recipe.RecipeName = "Demo recipe for testing";
                recipe.Description = "this was created to show the result of the Processing service";
                recipe.MealType = "Service test";
                uint recipeID = await _recipeDataLoader.InsertRecipeAndRelatedFields(recipe);

                FileManagerModel model = new FileManagerModel();
                model.FileID = MD5ToProcess;
                model.DateUploaded = DateTime.Now;
                model.NumberOfViews = 0;
                model.RecipeID = recipeID;
            }
        }

        public int GetNumberOfItemsInQueue()
        {
            return _recipesToProcess.Count();
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("RecipeProcessorService timer elapsed");
            await SortOutFiles();
        }

        public string PeekNextDocument()
        {
            return _recipesToProcess.PeekItem();
        }
    }

    public interface IRecipeProcessorService
    {
        public int GetCurrentQueueCapacity();

        public Task<ResultCode> QueueBrowserFilesForProcessing(ResultCode browserFiles);

        public int MaximumSingleFileSizeInBytes { get; }
        public bool FilesAreQueued();
        public string PeekNextDocument();
        public int GetNumberOfItemsInQueue();
    }

    public class ResultCode
    {
        /// <summary>
        /// Dictionary of result codes, for understanding processing
        /// </summary>
        public static Dictionary<int, string> RESULTCODES { get; } = new Dictionary<int, string>()
        {
            {1,"Successfully proccessed" },
            {670,"More files provided than can be queued" },
            {-1,"Unknown exception occured while processing" },
            {898,"A file with the same conent exists on the server (may have a different file name)" },
            {556,"Files deemed acceptible for hashing" },
            {302, "One of the files you uploaded exceeded the maximum file size" },
            {675, "Invalid file type- only word documents are expected" },
        };

        private readonly Dictionary<int, List<IBrowserFile>> _dictionary = new Dictionary<int, List<IBrowserFile>>();

        public void Clear()
        {
            _dictionary.Clear();
        }

        public int GetCount(int code)
        {
            if (_dictionary.ContainsKey(code))
            {
                return _dictionary[code].Count;
            }
            else
            {
                return 0;
            }
        }

        public void AddIBrowserFile(int code, IBrowserFile browserFile)
        {
            if (!_dictionary.ContainsKey(code))
            {
                _dictionary.Add(code, new List<IBrowserFile>());
            }
            _dictionary[code].Add(browserFile);
        }

        public void AddIBrowserFiles(int code, List<IBrowserFile> browserFile)
        {
            if (!_dictionary.ContainsKey(code))
            {
                _dictionary.Add(code, new List<IBrowserFile>());
            }
            _dictionary[code].AddRange(browserFile);
        }

        public List<IBrowserFile> GetIBrowserFiles(int code)
        {
            if (_dictionary.ContainsKey(code))
            {
                return _dictionary[code];
            }
            else
            {
                return null;
            }
        }

        public Dictionary<int, List<IBrowserFile>> GetKeyValuePairs()
        {
            return _dictionary;
        }

        public bool ContainsErrors()
        {
            if (_dictionary.Count > 0)
            {
                Dictionary<int, List<IBrowserFile>>.KeyCollection keys = _dictionary.Keys;
                foreach (int code in keys)
                {
                    if (code != 1 || code != 556)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
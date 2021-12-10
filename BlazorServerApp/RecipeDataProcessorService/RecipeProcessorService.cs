using BlazorServerApp.DocxReader;
using BlazorServerApp.Models;
using BlazorServerApp.TextProcessor;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorServerApp.proccessService
{
    public struct preProcessingContainer
    {
        public Task<Recipe> Recipe;
        public string DoccumentText;
    }

    public class RecipeProcessorService : IRecipeProcessorService
    {
        public const int maxiumumCapacity = 500;
        private CircularQueue<string> _recipesToProcess = new CircularQueue<string>(maxiumumCapacity);
        private readonly IFileManger _fileManager;
        private readonly IRecipeDataLoader _recipeDataLoader;
        private IDocxReader _docxReader;
        private ConcurrentDictionary<string, preProcessingContainer> preProcessRecipes = new();
        private ITextProcessor _textProcessor;

        public RecipeProcessorService(IFileManger fileManger, IRecipeDataLoader recipeDataLoader, IDocxReader docxReader, ITextProcessor text)
        {
            _fileManager = fileManger;
            _recipeDataLoader = recipeDataLoader;
            _docxReader = docxReader;
            _textProcessor = text;
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
            Task.Run(() => PreProcessRecipes()); //Expected result - want this to be done in background!
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

        public async Task PreProcessRecipes()
        {
            Console.WriteLine("Pre-processing started ....");
            if (GetNumberOfItemsInQueue() > 0)
            {
                foreach (string MD5 in _recipesToProcess.GetQueueAsList())
                {
                    if (!preProcessRecipes.ContainsKey(MD5))
                    {
                        preProcessingContainer container = new();
                        container.DoccumentText = await DocxToText(MD5);
                        container.Recipe = _textProcessor.CreateRecipe(container.DoccumentText);
                        preProcessRecipes.TryAdd(MD5, container);
                        await Task.Run(() => container.Recipe);
                        //await Task.Delay(10);
                    }
                }
            }
        }

        public int GetNumberOfItemsInQueue()
        {
            return _recipesToProcess.Count();
        }

        public async Task<ProcessorResult> PeekNextRecipe()
        {
            string nextItem = _recipesToProcess.PeekItem();
            if (nextItem != default(string))
            {
                if (preProcessRecipes.ContainsKey(nextItem))
                {
                    preProcessingContainer container = preProcessRecipes[nextItem];
                    Console.WriteLine("Contains key");
                    return new ProcessorResult(await container.Recipe, nextItem, container.DoccumentText); ;
                }
                else
                {
                    Console.WriteLine($"No key found, {nextItem}, {preProcessRecipes}");
                    string documentAsText = await DocxToText(nextItem);
                    return new ProcessorResult(await _textProcessor.CreateRecipe(documentAsText), nextItem, documentAsText);
                }
            }
            return new ProcessorResult(null, null, null);
        }

        public async Task<string> DocxToText(string MD5Hash)
        {
            return await _docxReader.GetTextAsync(_fileManager.GetFilePath(MD5Hash));
        }

        public async Task DeleteFile(string MD5Hash)
        {
            await Task.Run(() => _fileManager.DeleteFile(MD5Hash));
        }

        public void Dequeue()
        {
            string MD5 = _recipesToProcess.DequeueItem();
            if (preProcessRecipes.ContainsKey(MD5))
            {
                preProcessingContainer preProcessingContainer = new preProcessingContainer();
                if (preProcessRecipes.TryGetValue(MD5, out preProcessingContainer))
                {
                    preProcessRecipes.TryRemove(MD5, out preProcessingContainer);
                }
            }
        }

        public async Task InsertRecipeAndFileToDB(Recipe recipe, string MD5)
        {
            uint recipeID = await _recipeDataLoader.InsertRecipeAndRelatedFields(recipe);
            await _fileManager.CreateFileToRecipeRelationship(recipeID, MD5);
        }

        /// <summary>
        /// Used incase of catasrophic errors, to prevent the bulk uploader from ever being locked out.
        /// </summary>
        public async void Clear()
        {
            while (!_recipesToProcess.QueueIsEmpty())
            {
                string MD5 = _recipesToProcess.DequeueItem();
                _fileManager.DeleteFile(MD5);
                uint? recipeID = await _recipeDataLoader.DeleteOnlyFile(MD5); // This seems like a very edge case, but you never know!
                if (recipeID != null)
                {
                    await _recipeDataLoader.DeleteRecipeAndRelatedValues((uint)recipeID);
                }
            }

            _recipesToProcess = new CircularQueue<string>(maxiumumCapacity);
        }
    }

    public class ProcessorResult
    {
        public ProcessorResult()
        {
        }

        public ProcessorResult(Recipe recipe, string md5, string doccumentText)
        {
            MD5 = md5;
            Recipe = recipe;
            DocumentText = doccumentText;
        }

        public Recipe Recipe { get; set; }
        public string MD5 { get; set; }
        public string DocumentText { get; set; }
    }

    public interface IRecipeProcessorService
    {
        public int GetCurrentQueueCapacity();

        public Task<ResultCode> QueueBrowserFilesForProcessing(ResultCode browserFiles);

        public int MaximumSingleFileSizeInBytes { get; }

        public bool FilesAreQueued();

        //public string PeekNextDocument();
        public int GetNumberOfItemsInQueue();

        //public Task<string> DocxToText(string MD5Hash);
        public Task<ProcessorResult> PeekNextRecipe();

        public Task DeleteFile(string MD5Hash);

        public void Dequeue();

        public Task InsertRecipeAndFileToDB(Recipe recipe, string MD5);

        public void Clear();
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
            {302, "A file you uploaded exceeded the maximum file size" },
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
                    if (code != 1 && code != 556)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

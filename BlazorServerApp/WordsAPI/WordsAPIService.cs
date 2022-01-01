using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BlazorServerApp.Extensions;
using BlazorServerApp.Models;
using System.Collections.Concurrent;

namespace BlazorServerApp.WordsAPI
{
    public class WordsAPIService : IWordsAPIService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IRecipeDataLoader _dataLoader;
        private int remainingCallouts = 2000;
        private int deleteSchedulle = 90000;
        private int deleteAttempts = 5;
        private bool serviceEnabled = true;
        private ConcurrentDictionary<string, Task<TypeOf>> taskDictionary = new();
        public WordsAPIService(IHttpClientFactory httpClientFactory, IConfiguration config,IRecipeDataLoader dataAccess)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _dataLoader = dataAccess;
        }
        
        public async Task<TypeOf> CallCachedAPI(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            if (searchTerm.Length>1)
            {
                List<SearchQuery> queries = await _dataLoader.FindWordsAPISearch(searchTerm); 
                if (queries.Count>= 1)
                {
                    return JsonConvert.DeserializeObject<TypeOf>(queries[0].ResultAsJSON);
                }
                else
                {
                    
                    if (taskDictionary.ContainsKey(searchTerm))
                    {
                        try
                        {
                            return await taskDictionary[searchTerm];
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"Error {e.Message} encountered for \"{searchTerm}\". Null being returned consequently");
                            return null;
                        }

                    }
                    else
                    {

                        Task<TypeOf> task = CallAPI(searchTerm);
                        bool succsessfulAdding = taskDictionary.TryAdd(searchTerm, task);

                        if (!succsessfulAdding)
                        {
                            Console.WriteLine($"error while adding \"{searchTerm}\" to dictionary. Most likely already there: {taskDictionary.ContainsKey(searchTerm)}");

                        }
                        Task.Run(()=>ScheduleDeletionOfTaskItem(searchTerm, 1));
                        return await task;
                    }
                    
                }
            }
            return null;
        }

        private async Task ScheduleDeletionOfTaskItem(string searchTerm,int count)
        {
            await Task.Delay(deleteSchedulle);
            if (taskDictionary.ContainsKey(searchTerm) && count<deleteAttempts)
            {
                bool suucess = taskDictionary.TryRemove(searchTerm,out _);
                if (!suucess)
                {
                    Console.WriteLine($"Error while attempting to remove item from dictionary. Attempt: {count} for search term \"{searchTerm}\"");
                    ScheduleDeletionOfTaskItem(searchTerm, count++);
                }
                else
                {
                    Console.WriteLine($"Scheduled deletion of \"{searchTerm}\" complete. {taskDictionary.Count} items remain in task dictionary");
                }
            }
        }

        private async Task<TypeOf> CallAPI(string word)

        {
            if(word == null)
            {
                return null;
            }
            if (remainingCallouts > 10 && serviceEnabled)
            {
                var client = _httpClientFactory.CreateClient("WordsAPIClient");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://wordsapiv1.p.rapidapi.com/words/{word}/typeOf"),
                    Headers =
    {
        { "x-rapidapi-host", "wordsapiv1.p.rapidapi.com" },
        { "x-rapidapi-key", _config.GetConnectionString("WordsAPI") },
    },
                };
                using (var response = await client.SendAsync(request))
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                     remainingCallouts =int.Parse(response.Headers.GetValues("x-ratelimit-requests-remaining").FirstOrDefault())-1;
                    if (response.IsSuccessStatusCode)
                    {
                        SearchQuery searchQuery = new();
                        searchQuery.SearchTerm = word;
                        searchQuery.ResultAsJSON = jsonResult;
                        List<SearchQuery> queries  = await _dataLoader.FindWordsAPISearch(word);
                        if (queries == null || queries.Count == 0)
                        {
                            try
                            {
                                await _dataLoader.InsertSearchQuery(searchQuery);
                            }
                            catch
                            {
                                Console.WriteLine($"Error while inserting {searchQuery.SearchTerm} into database. Likely that it was allready in there!");
                            }
                            return JsonConvert.DeserializeObject<TypeOf>(jsonResult);
                        }
                        else
                        {
                            return JsonConvert.DeserializeObject<TypeOf>(queries[0].ResultAsJSON); // Naiively assume only one result.
                        }
                        
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else
                    {
                        throw new Exception("Error while attempting to connect to API");
                    }
                }
            }
            else
            {
                throw new Exception("API Limit has been exhausted. Please try again later.");
            }
            

        }

       



    }
}

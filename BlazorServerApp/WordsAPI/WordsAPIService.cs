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

namespace BlazorServerApp.WordsAPI
{
    public class WordsAPIService : IWordsAPIService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IRecipeDataLoader _dataLoader;
        private int remainingCallouts = 2000;
        private bool serviceEnabled = true;




        public WordsAPIService(IHttpClientFactory httpClientFactory, IConfiguration config,IRecipeDataLoader dataAccess)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _dataLoader = dataAccess;
        }
        
        public async Task<TypeOf> CallCachedAPI(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            if (!await _dataLoader.ContainsStopWord(searchTerm))
            {
                List<SearchQuery> queries = await _dataLoader.FindWordsAPISearch(searchTerm);
                if (queries.Count == 1)
                {
                    return JsonConvert.DeserializeObject<TypeOf>(queries[0].ResultAsJSON);
                }
                else
                {
                    TypeOf type = await CallAPI(searchTerm);
                    SearchQuery searchQuery = new();
                    searchQuery.SearchTerm = searchTerm;
                    searchQuery.ResultAsJSON = JsonConvert.SerializeObject(type);
                    await _dataLoader.InsertSearchQuery(searchQuery);
                    return type;
                }
            }
            return null;
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
                     remainingCallouts =int.Parse(response.Headers.GetValues("x-ratelimit-requests-remaining").FirstOrDefault())-1;

                    
                    if (response.IsSuccessStatusCode)
                    {

                        return JsonConvert.DeserializeObject<TypeOf>(await response.Content.ReadAsStringAsync());
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

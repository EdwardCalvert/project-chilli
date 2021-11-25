using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorServerApp.WordsAPI
{
    public class WordsAPIService : IWordsAPIService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private int remainingCallouts = 2000;
        private bool serviceEnabled = true;




        public WordsAPIService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<TypeOf> CallAPI(string word)

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

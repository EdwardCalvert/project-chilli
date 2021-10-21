using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorServerApp.Extensions
{
    public static class DistributedCacheExtensioins
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordID, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
            //Gives a default value, if absolute expire Time is null.
            //So if you put an item in the cache, it will only last for one minute
            options.SlidingExpiration = unusedExpireTime;
            //If you don't use the cached item, within the sliding exiriation time, it fetches new data

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordID, jsonData, options);
        }


        /// <summary>
        /// Checks the redis DB for the given ID. If it is null, the default for that type is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="recordId"></param>
        /// <returns>The object at index recordId</returns>
        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);
            if(jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}

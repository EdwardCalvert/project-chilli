using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalyst;
using Catalyst.Models;
using Microsoft.Extensions.Logging;
using Mosaik.Core;
namespace TestingCatalyst
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Catalyst.Models.English.Register(); //You need to pre-register each language (and install the respective NuGet Packages)

            await DoStuff();
        }

        public static async Task DoStuff()
        {
            Storage.Current = new DiskStorage("catalyst-models");
            var nlp = await Pipeline.ForAsync(Language.English);
            var doc = new Document("The quick brown fox jumps over the lazy dog", Language.English);
            nlp.ProcessSingle(doc);
            Console.WriteLine(doc.ToJson());
            
        }
    }
}

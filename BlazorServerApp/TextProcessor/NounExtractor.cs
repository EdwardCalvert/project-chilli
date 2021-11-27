using Catalyst;
using Catalyst.Models;
using Mosaik.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Version = Mosaik.Core.Version;
using P = Catalyst.PatternUnitPrototype;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BlazorServerApp.TextProcessor
{
    public class NounExtractor :INounExtractor

    {
        private Pipeline nlp;

        public NounExtractor()
        {
            Catalyst.Models.English.Register();
            Storage.Current = new DiskStorage("catalyst-models");
            
        }

        public  async Task<List<string>> ExtractNouns(string wallOfText)
        {
            nlp = await Pipeline.ForAsync(Language.English);
            string processedDoc = wallOfText.Replace(".", ".  ").Replace("\t", " ");
            var doc = new Document(processedDoc, Language.English);
             nlp.ProcessSingle(doc);
            //Console.WriteLine(doc.TokenizedValue());
            List<string> noungs = new();
            foreach (List<TokenData> tokenDatas in doc.TokensData)
            {
                Console.WriteLine("---------------------------------------NEW SENTENCE");

                foreach (TokenData data in tokenDatas)
                {
                    if (data.Tag == PartOfSpeech.NOUN)
                    {
                        noungs.Add(doc.Value.Slice(data.LowerBound, data.UpperBound + 1));
                        Console.WriteLine("Noun: " + doc.Value.Slice(data.LowerBound, data.UpperBound + 1));
                    }
                }
            }
            return noungs;
        }


    }

    public interface INounExtractor
    {
        public Task<List<string>> ExtractNouns(string wallOfText);
    }

    public static class stringExtension
    {
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }
    }
}


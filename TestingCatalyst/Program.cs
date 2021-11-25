
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
namespace TestingCatalyst

{
    class Program
    {
        static async Task Main(string[] args)
        {
            Catalyst.Models.English.Register(); //You need to pre-register each language (and install the respective NuGet Packages)

            await DoStuff();
        }

        public const string AppleAmberWithoutIngredients = @"APPLE AMBER
Serves 4


Method
1.	Prepare an oven, Gas 3 or 170°C.Grease the sides of the oven proof dish.
2.	Peel and slice the .Put into a medium saucepan with 2 tbsp of water and cook over low heat until soft.   
3.	Separate the eggs (break the egg onto a saucer and place an egg cup over the yolk, pour the white into a large bowl).    
4.	Add the egg yolks to the cooked apple and stir well.
5.	Put the apple into the oven proof dish.
6.	Whisk egg whites until stiff.Add half the sugar and whisk again until mixture stands in peaks.
7.	Fold in the rest of the sugar and spoon or pipe onto the apple.  
8.	Place in oven and bake for 30 mins.until golden brown.

Note: 
•	it is important to make sure that the meringue is well baked to kill salmonella bacteria.  If possible check with a temperature probe.
•	separate eggs carefully because the slightest trace of yolk will prevent the egg whites from whisking. 

Variations:
•	use other varieties of fruit, for example rhubarb or plums.
•	could be cooked in a pastry case.
 
";

         const string LotsOfEquipment = @"Apple corer To remove the core and pips from apples and similar fruits		
Apple cutter		To cut apple and similar fruits easily while simultaneously removing the core and pips.	Cf. peeler	
Baster		Used during cooking to cover meat in its own juices or with a sauce.	An implement resembling a simple pipette, consisting of a tube to hold the liquid, and a rubber top which makes use of a partial vacuum to control the liquid's intake and release. The process of drizzling the liquid over meat is called basting – when a pastry brush is used in place of a baster, it is known as a basting brush.	
Beanpot		A deep, wide-bellied, short-necked vessel used to cook bean-based dishes	Beanpots are typically made of ceramic, though pots made of other materials, like cast iron, can also be found. The relatively narrow mouth of the beanpot minimizes evaporation and heat loss, while the deep, wide, thick-walled body of the pot facilitates long, slow cooking times. They are typically glazed both inside and out, and so cannot be used for clay pot cooking.";

        public static async Task DoStuff()
        {
            string processedDoc = AppleAmberWithoutIngredients.Replace(".", ".  ").Replace(",", ",  ").Replace("\t"," ");

            Storage.Current = new DiskStorage("catalyst-models");
            var nlp = await Pipeline.ForAsync(Language.English);
            var doc = new Document(processedDoc, Language.English);
            nlp.ProcessSingle(doc);
            Console.WriteLine(doc.TokenizedValue());
            List<string> noungs = new();
            foreach (List<TokenData> tokenDatas in doc.TokensData)
            {
                Console.WriteLine("---------------------------------------NEW SENTENCE");
                PartOfSpeech previousTag = PartOfSpeech.NONE;

               foreach(TokenData data in tokenDatas)
                {
                    if(previousTag == PartOfSpeech.ADJ && data.Tag == PartOfSpeech.NOUN)
                    {
                        Console.WriteLine("Noun phrase");
                    }
                    if(data.Tag == PartOfSpeech.NOUN)
                    {
                        noungs.Add(doc.Value.Slice(data.LowerBound, data.UpperBound + 1));
                    }
                    Console.WriteLine(data.Tag);
                    Console.WriteLine(doc.Value.Slice(data.LowerBound,data.UpperBound+1));
                    Console.WriteLine();
                    previousTag = data.Tag;
                }
            }

            foreach(string s in noungs)
            {
                Console.WriteLine(s);
            }
        }

        
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

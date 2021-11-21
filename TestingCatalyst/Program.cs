
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

        public const string AppleAmber = @"APPLE AMBER
Serves 4

 Ingredients 
 750g cooking apples 
 100g caster sugar  
 2 eggs

ovenproof dish

Method
1.	Prepare an oven, Gas 3 or 170°C.Grease the sides of the oven proof dish.
2.	Peel and slice the apples.Put into a medium saucepan with 2 tbsp of water and cook over low heat until soft.   
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

        public static async Task DoStuff()
        {
            while (true)
            {
                string text = Console.ReadLine();
                Storage.Current = new DiskStorage("catalyst-models");
                var nlp = await Pipeline.ForAsync(Language.English);
                var doc = new Document(text, Language.English);
                nlp.ProcessSingle(doc);
                Console.WriteLine(doc.ToJson());
            }

            //Console.WriteLine("Loading models... This might take a bit longer the first time you run this sample, as the models have to be downloaded from the online repository");
            //var nlp3 = await Pipeline.ForAsync(Language.English);
            //nlp.Add(await AveragePerceptronEntityRecognizer.FromStoreAsync(language: Language.English, version: Version.Latest, tag: "WikiNER"));
            //var isApattern = new PatternSpotter(Language.English, 0, tag: "is-a-pattern", captureTag: "IsA");
            //isApattern.NewPattern(
            //    "Is+Noun",
            //    mp => mp.Add(
            //        new PatternUnit(P.Single().WithToken("is").WithPOS(PartOfSpeech.VERB)),
            //        new PatternUnit(P.Multiple().WithPOS(PartOfSpeech.NOUN, PartOfSpeech.PROPN, PartOfSpeech.AUX, PartOfSpeech.DET, PartOfSpeech.ADJ))
            //));
            //nlp3.Add(isApattern);
            //Console.WriteLine(nlp3);
            //Console.WriteLine(doc.Value);
            //Console.WriteLine(doc.TokensData);
            //Console.WriteLine(doc.TokenizedValue());
            //Console.WriteLine(doc.EntityData);
            ////Console.WriteLine(doc.)
            //foreach (KeyValuePair<long,Dictionary<string,string>> keyValuePair in doc.TokenMetadata) {

            //    Console.WriteLine(keyValuePair.Key);
            //    foreach(KeyValuePair<string,string> keyValuePair1 in keyValuePair.Value)
            //    {
            //        Console.WriteLine(keyValuePair1.Key);
            //        Console.WriteLine(keyValuePair1.Value);
            //    }
            //}


            //Console.WriteLine(doc.ToJson());


            //    var nlp2 = await Pipeline.ForAsync(Language.English);
            //    var ft = new FastText(Language.English, 0, "wiki-word2vec");
            //    ft.Data.Type = FastText.ModelType.CBow;
            //    ft.Data.Loss = FastText.LossType.NegativeSampling;
            //    ft.Train(nlp.Process(GetDocs()));
            //    ft.StoreAsync();
            //}
        }
    }
}

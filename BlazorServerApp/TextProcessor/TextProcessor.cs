using BlazorServerApp.Models;
using BlazorServerApp.WordsAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlazorServerApp.TextProcessor
{
    public class TextProcessor : ITextProcessor
    {
        private INounExtractor _nounExtractor;
        private IWordsAPIService _wordsAPIService { get; set; }
        private IRecipeDataLoader _dataLoader;

        public TextProcessor(INounExtractor nounExtractor, IWordsAPIService wordsAPIService, IRecipeDataLoader dataLoader)
        {
            _nounExtractor = nounExtractor;
            _dataLoader = dataLoader;
            _wordsAPIService = wordsAPIService;
        }



        private static int extractMaximumNumber(String str)
        {
            int num = 0, res = 0;

            // Start traversing the given string
            for (int i = 0; i < str.Length; i++)
            {
                // If a numeric value comes, start
                // converting it into an integer
                // till there are consecutive
                // numeric digits
                if (char.IsDigit(str[i]))
                    num = num * 10 + (str[i] - '0');

                // Update maximum value
                else
                {
                    res = Math.Max(res, num);

                    // Reset the number
                    num = 0;
                }
            }

            // Return maximum value
            return Math.Max(res, num);
        }

        private static double ConvertSimpleFractionToDouble(string input)
        {
            input = input.Trim();
            string[] fraction = input.Split("/");
            int numerator = TextProcessor.extractMaximumNumber(fraction[0]);
            int denominator = TextProcessor.extractMaximumNumber(fraction[1]);
            return (numerator / denominator);
        }

        

        private async Task<string> RemoveUneccessaryLinesFromDescription(string description)
        {
            string previousLine = "";
            string newDescription = "";
            description = description.Replace("Ingredients", "").Replace("Method", "");
            foreach (string loopLine in description.Split("\n"))
            {
                string line = loopLine.Trim();
                if (previousLine.Length <= 2 && line.Length <= 2)
                {
                    previousLine = line;
                }
                else
                {
                    if (line.Length > 2)
                    {
                        newDescription += line + "\n";
                        previousLine = line;
                    }
                    else
                    {
                        previousLine = loopLine;
                    }
                }
            }
            return newDescription;
        }


        public const string SPOONS = "((rounded|heaped|level) )?((tea|desert|table)? ?(spoonful|spoon|tbsp|tsp))s?";
        public const string UNITS = "(((fluid ounce|ounce|oz|kilograms|cup|gram|pint|pound|quart|kg|g|ml|fl\\.|gal\\.|lb\\.|pt|qt\\.)s?)|(" + SPOONS + "))"; //sort by length- ie teaspoons before teaspoon
        public const string NUMBERSWITHFRACTIONS = "([0-9]|½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)";
        public const string TIMEWORDS = "(minutes|minute|mins|min|hours|hour|seconds|second|days|day)";
        public const string INGREDIENTSWITHCOUNTREGEX = BULLETPOINT + "[0-9]{0,3} ?" + NUMBERSWITHFRACTIONS + "(\\.|\\/| |-)?[0-9]{0,6} ?" + UNITS + " ([^\\.\\;\n\t](?!(and|or))){3,200}";
        public const string INGREDIENTSWITHOUTUNITREGEX = BULLETPOINT + "[1-9][0-9]? ((?!" + TIMEWORDS + ")[A-z]| |,|\\([^\r\t\\(\\)\n]{2,100}\\)|-){3,150}[a-z]";
        public const string UNITSFROMTEXT = "(?<=" + NUMBERSWITHFRACTIONS + ") ?" + UNITS + " ";
        public const string STOPWORDLOOKBEHIND = "(?!( and| or))";
        public const string BULLETPOINT = "(•\t)?";
        public const string METHODREGEX = @"[1-9][0-9]{0,3}\.(\t| ){0,4}[^\t\n\r]{10,500}";
        public const string SERVINGREGEX = "((serves|makes) ?[1-9][0-9]? ?(to|-)? ?[1-9]?[0-9]?)|([1-9][0-9]? ?(to|-)? ?[1-9]?[0-9]? ?(servings|serving|portions|portion))";

        public const string TIMEREGEX = "([1-9][0-9]?( (to|-|—) ))?[1-9][0-9]? " + TIMEWORDS;

        public static int GetServingCount(string inputText)
        {
            //https://regexr.com/69vg0
            MatchCollection servingRegularExpression = Regex.Matches(inputText, SERVINGREGEX, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            if (servingRegularExpression.Count == 1)
            {
                return extractMaximumNumber(servingRegularExpression[0].Value);
            }
            return 0;
        }

        public static List<Method> GetMethods(string inputText)
        {
            List<Method> methods = new();
            //Go to https://regexr.com/69vqp to understand how this works
            MatchCollection methodMatchCollection = Regex.Matches(inputText, METHODREGEX, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach (Match match in methodMatchCollection)
            {
                Method method = new Method();
                method.MethodText = Regex.Replace(match.Value, @"[1-9][0-9]{0,3}\.(\t| )", "").Trim();
                methods.Add(method);
            }
            return methods;
        }

        public async Task<List<Ingredient>> GetIngredientsWithUnits(string inputText, bool insertIngredientOnEmptyResult)
        {
            List<Ingredient> ingredients = new();
            //Go to https://regexr.com/69vn4 for a break down.
            MatchCollection ingredientsWithUnitsRegex = Regex.Matches(inputText, INGREDIENTSWITHCOUNTREGEX, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach(Match match in ingredientsWithUnitsRegex)
            {
                Ingredient ingredient = new(_wordsAPIService,_nounExtractor);
                ingredient.IngredientName = match.Value.Trim();
                ingredients.Add(ingredient);
            }
            
            return ingredients; 
        }

        public static string GetDifficulty(List<Method> methods)
        {
            if (methods.Count < 5)
            {
                return "Easy";
            }
            else if (methods.Count > 10)
            {
                return "Hard";
            }
            else
            {
                return "Medium";
            }
        }

        /// <summary>
        /// Method that extracts ingredients from a recipe, without a unit. It has a habit of picking up any text that is followed by  a number, so make sure to give it clean text.
        /// </summary>
        /// <param name="textAreaWithoutTimes"></param>
        /// <param name="dataLoader"></param>
        /// <param name="wordsAPIService"></param>
        /// <returns></returns>
        public async Task<List<Ingredient>> GetIngredientsWithoutUnit(string textAreaWithoutTimes, bool insertIngredientOnEmptyResult)
        {
            List<Ingredient> ingredientInRecipes = new();
            // + UNITS + "|"
            MatchCollection ingredientsWithNumber = Regex.Matches(textAreaWithoutTimes, INGREDIENTSWITHOUTUNITREGEX, RegexOptions.Compiled, new TimeSpan(10000000));
            foreach (Match match in ingredientsWithNumber)
            {
                Ingredient ingredient = new(_wordsAPIService,_nounExtractor);
                ingredient.IngredientName = match.Value.Trim();
                ingredientInRecipes.Add(ingredient);
            }
            return ingredientInRecipes;
        }

        public async Task<string> CreateDescription(string inputText)
        {
            inputText = Regex.Replace(inputText, SERVINGREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            inputText = Regex.Replace(inputText, INGREDIENTSWITHCOUNTREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            inputText = Regex.Replace(inputText, METHODREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            inputText = Regex.Replace(inputText, SERVINGREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            return await RemoveUneccessaryLinesFromDescription(inputText);
        }
        public string GetTitle(string input)
        {
            foreach (string line in input.Split("\n"))
            {
                if (line != null && line.Length > 2)
                    return line;
            }
            return input.Split("\n")[0];
        }

        public async Task<Recipe> CreateRecipe(string inputText)
        {
            Console.WriteLine("Processing started");
            if (inputText.Length < 50)
            {
                throw new Exception("A Recipe must contain more than 50 characters.");
            }
            string description;
            Recipe newRecipe = new Recipe();
            newRecipe.RecipeName = GetTitle(inputText);
            description = inputText.Replace(newRecipe.RecipeName, "");
            newRecipe.Description = await Task.Run(() => CreateDescription(description));
            string possibleNouns = newRecipe.Description;
            foreach (Method s in newRecipe.Method)
            {
                possibleNouns += s.MethodText + "\n ";
            }
            
            //Dependent on description and method being complete!
            //Task<List<Equipment>> getEquipment = Task.Run(() => GetEquipmentID(possibleNouns));

            newRecipe.Servings = await Task.Run(() => GetServingCount(inputText));

            newRecipe.Method = await Task.Run(() => GetMethods(inputText));

            newRecipe.Difficulty = await Task.Run(() => GetDifficulty(newRecipe.Method));
            Task<List<Ingredient>> getIngredientsWithUnits = Task.Run(() => GetIngredientsWithUnits(inputText, true));
            Task<List<Ingredient>> getIngredientsWithoutUnits = Task.Run(() => GetIngredientsWithoutUnit(newRecipe.Description, true));

            await Task.Delay(10);
            newRecipe.Ingredients.AddRange(await getIngredientsWithUnits);
            newRecipe.Ingredients.AddRange(await getIngredientsWithoutUnits);
            Console.WriteLine("Processing finished");

            return newRecipe;
        }

        public static bool DoesEquipmentAreadyHaveID(List<Equipment> equipment, string equipmentName)
        {
            foreach (Equipment equipment1 in equipment)
            {
                if (equipment1.EquipmentName == equipmentName)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<List<Equipment>> GetEquipmentID(string descriptonToSearch)
        {
            List<Equipment> EquipmentIDs = new();
            List<string> nouns = await _nounExtractor.ExtractNouns(descriptonToSearch);

            var bag = new ConcurrentBag<object>();
            var tasks = nouns.Select(async noun =>
            {
                // some pre stuff
                TypeOf response = await _wordsAPIService.CallCachedAPI(noun);
                bag.Add(response);
                // some post stuff
            });
            await Task.WhenAll(tasks);
            var count = bag.Count;
            foreach (TypeOf typeOf in bag.ToArray())
            {

                bool insertMade = false;
                if (typeOf != null && typeOf.typeOf != null && (typeOf.typeOf.Contains("equipment") || typeOf.typeOf.Contains("kitchen appliance") || typeOf.typeOf.Contains("utensil") || typeOf.typeOf.Contains("electronic equipment") || typeOf.typeOf.Contains("mixer")))
                {
                    string noun = typeOf.word;
                    IEnumerable<Equipment> equipment = await _dataLoader.FindEquipmentLike(noun);
                    if (equipment.Any())
                    {
                        foreach (Equipment equipment1 in equipment)
                            if (noun == equipment1.EquipmentName) /*TextProcessor.LevenshteinDistance(noun, equipment1.EquipmentName) <= TextProcessor.CalculateLevenshteinThreshold(noun) && !insertMade)*/
                            {
                                if (!DoesEquipmentAreadyHaveID(EquipmentIDs, equipment1.EquipmentName))
                                {
                                    EquipmentIDs.Add(equipment1);
                                    insertMade = true;
                                }
                                else
                                {
                                    insertMade = true;
                                }
                            }
                    }
                    if (!insertMade)
                    {
                        Equipment equipment1 = new Equipment();
                        equipment1.EquipmentName = noun;
                        equipment1.EquipmentID = await _dataLoader.InsertEquipment(equipment1);
                        EquipmentIDs.Add(equipment1);
                    }
                }
            }
            return EquipmentIDs;
        }

        public async Task<uint?> FindIngredientInDatabase(string ingredientName)
        {
            //Find any existing ingredients
            IEnumerable<Ingredient> ingredientsInDB = await _dataLoader.FindIngredients(ingredientName);

            if (ingredientsInDB.Any())
            {
                foreach (Ingredient userDefinedIngredient in ingredientsInDB)
                {
                    if (ingredientName == userDefinedIngredient.IngredientName)/*TextProcessor.LevenshteinDistance(ingredientName, userDefinedIngredient.IngredientName) <= TextProcessor.CalculateLevenshteinThreshold(ingredientName))*/ // i.e. only find a ingredient that is very similar to the cureent ingedient. The threshold is designed to prevent erreoneous results at the low end e.g. eggs => egg etc, but having a small ammount of flexibility at the top end.
                    {
                        return userDefinedIngredient.IngredientID;
                    }
                }
            }
            return null;
        }

    }

    public interface ITextProcessor
    {
        public Task<Recipe> CreateRecipe(string inputText);
    }
}

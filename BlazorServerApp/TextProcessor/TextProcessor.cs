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
        private IWordsAPIService _wordsAPIService;
        private IRecipeDataLoader _dataLoader;

        public TextProcessor(INounExtractor nounExtractor, IWordsAPIService wordsAPIService, IRecipeDataLoader dataLoader)
        {
            _nounExtractor = nounExtractor;
            _dataLoader = dataLoader;
            _wordsAPIService = wordsAPIService;
        }

        public static int LevenshteinDistance(string source1, string source2)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }

        public static int CalculateLevenshteinThreshold(string input)
        {
            if (input.Length <= 4)
                return 0; //i.e. egg would go to eggs etc.
            else if (input.Length >= 20)
                return 3; //Longer words may have more spelling inaccuracies, but we wouldn't want totally irrevivant results
            else if (input.Length >= 10)
                return 2; //thiis is the range 5<=input.Length<=10 This may be too high.
            else
                return 1;//thiis is the range 5<=input.Length<=10 This may be too high.
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

        private static int FindFirstInteger(string input)
        {
            int number = 0;

            for (int i = 0; i < input.Length; i++) // loop over the complete input
            {
                if (Char.IsDigit(input[i])) //check if the current char is digit
                    number = number * 10 + (input[i] - '0');
                else
                    return number; //Stop the loop after the first character
            }
            return -1;
        }

        private static double ConvertComplexFractionToDouble(string input)
        {
            input = input.Trim();
            string[] wholeNumber = input.Split(" ");
            return TextProcessor.extractMaximumNumber(wholeNumber[0]) + ConvertSimpleFractionToDouble(input);
        }

        private static double ConvertSimpleFractionToDouble(string input)
        {
            input = input.Trim();
            string[] fraction = input.Split("/");
            int numerator = TextProcessor.extractMaximumNumber(fraction[0]);
            int denominator = TextProcessor.extractMaximumNumber(fraction[1]);
            return (numerator / denominator);
        }

        private static double ExtractDouble(string input)
        {
            if (Regex.IsMatch(input, "[0-9]{1,2} [0-9]{1,2}/[0-9]"))// Matches fractions in the form 1 1/2 etc
            {
                //then return.
                //split(" ") then find the largest number in [0]
                //at index two split("/"), find the largest number in each section.
                string[] splitInputOnSpace = input.Split(" ");
                int number = TextProcessor.extractMaximumNumber(splitInputOnSpace[0]);
                return number + ConvertComplexFractionToDouble(splitInputOnSpace[1]);
            }
            else if (Regex.IsMatch(input, "[0-9]{1,2}/[0-9]")) // matches fractions like 1/2 1/4 etc
            {
                return ConvertComplexFractionToDouble(input);
            }
            else if (Regex.IsMatch(input, "[0-9]{1,3}\\.[0-9]{1,2}")) // Matches decimals like 1.3 etc
            {
                return double.Parse(input);
            }
            else if (Regex.IsMatch(input, "[0-9] ?(½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)"))
            {
                int integerComponent = TextProcessor.extractMaximumNumber(input);
                //Find which fraction was in the input. Find the value of the number. Add
                foreach (KeyValuePair<string, double> keyValuePair in doubleEquivalent)
                {
                    if (input.Contains(keyValuePair.Key)) // It has
                    {
                        return integerComponent + keyValuePair.Value;
                    }
                }
                return integerComponent;
            }
            else //I really have no idea what input they attempted, so I will just choose the largest number.
            {
                return TextProcessor.FindFirstInteger(input);
            }
        }

        private async Task<string> RemoveUneccessaryLinesFromDescription(string description)
        {
            string previousLine = "";
            string newDescription = "";
            description = description.Replace("Ingredients", "").Replace("Method", "");
            foreach (string loopLine in description.Split("\n"))
            {
                //List<string> nouns = await _nounExtractor.ExtractNouns(loopLine);

                string line = loopLine.Trim();
                if (previousLine.Length <= 2 && line.Length <= 2)
                {
                    previousLine = line;
                }
                else
                {
                    if (line.Length > 10)
                    {
                        newDescription += line + "\n";
                        previousLine = line;
                    }
                    else
                    {
                        List<string> nouns = await _nounExtractor.ExtractNouns(loopLine);
                        if (nouns.Count > 0)
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
            }
            return newDescription;
        }

        public static readonly Dictionary<string, double> doubleEquivalent = new()
        {
            { "½", 0.5 },
            { "⅓", 0.33 },
            { "⅔", 0.66 },
            { "¼", 0.25 },
            { "⅕", 0.2 },
            { "⅗", 0.6 },
            { "⅘", 0.8 },
            { "⅙", 0.16 },
            { "⅚", 0.83 },
            { "⅐", 0.14 },
            { "⅛", 0.125 },
            { "⅜", 0.325 },
            { "⅝", 0.625 },
            { "⅞", 0.875 },
            { "⅑", 0.11 },
            { "⅒", 0.10 },
        };

        public static string ConvertStringToSUPPORTEDUNITS(string unit)
        {
            unit = unit.Trim().ToLower();
            if (unit == "g")
                return "Grams";
            else if (unit == "l")
                return "Litre";
            else if (unit == "ml")
                return "Mililetres";
            foreach (string supportedUnit in Recipe.SUPPORTEDUNITS)
            {
                if (unit == supportedUnit.ToLower())
                    return supportedUnit;
            }

            foreach (KeyValuePair<string, string> keyValuePair in ReplaceShortHand)
            {
                unit = unit.Replace(keyValuePair.Key, keyValuePair.Value);
            }
            unit = unit.Trim().ToLower();
            foreach (string supportedUnit in Recipe.SUPPORTEDUNITS)
            {
                if (unit.ToLower() == supportedUnit.ToLower())
                    return supportedUnit;
            }

            return "x"; // Problem is that we have defined more units in the REGEX than we actually support.
        }

        public static Dictionary<string, string> ReplaceShortHand = new Dictionary<string, string>()
        {
            {"oz","Ounce" },
            {"rounded","" },
            {"heaped","" },
            {"level","" },
            {"gram","Grams" },
            {"kg","Kilograms"},
            {"fl.","Fluid Ounces" },
            {"pt.","Pint" },
            {"teaspoon","Tsp" },
            {"tablespoon","Tbsp" },
            {"lb." ,"Pound"},
            {"ml","Mililetres" },
            {"qt.","Quart" },
        };

        public const string SPOONS = "((rounded|heaped|level) )?((tea|desert|table)? ?(spoonful|spoon|tbsp|tsp))s?";
        public const string UNITS = "(((fluid ounce|ounce|oz|kilograms|cup|gram|pint|pound|quart|kg|g|ml|fl\\.|gal\\.|lb\\.|pt|qt\\.)s?)|(" + SPOONS + "))"; //sort by length- ie teaspoons before teaspoon
        public const string NUMBERSWITHFRACTIONS = "([0-9]|½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)";
        public const string TIMEWORDS = "(minutes|minute|mins|min|hours|hour|seconds|second|days|day)";
        public const string INGREDIENTSWITHCOUNTREGEX = BULLETPOINT + "[0-9]{0,3} ?" + NUMBERSWITHFRACTIONS + "(\\.|\\/| |-)?[0-9]{0,6} ?" + UNITS + " [^\\.\\;\n\t]{3,200}";
        public const string INGREDIENTSWITHOUTUNITREGEX = BULLETPOINT + "[1-9][0-9]? ((?!" + TIMEWORDS + ")[A-z]| |,|\\([^\r\t\\(\\)\n]{2,100}\\)|-){3,150}[a-z]";
        public const string UNITSFROMTEXT = "(?<=" + NUMBERSWITHFRACTIONS + ") ?" + UNITS + " ";
        public const string BULLETPOINT = "(•\t)?";
        public const string METHODREGEX = "[1-9][0-9]{0,3}\\.(\t| )[^\t\n\r]{10,500}";
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
                method.MethodText = Regex.Replace(match.Value, "[1-9][0-9]{0,3}\\.(\t| )", "");
                methods.Add(method);
            }
            return methods;
        }

        public async Task<List<UserDefinedIngredientInRecipe>> GetIngredientsWithUnits(string inputText, bool insertIngredientOnEmptyResult)
        {
            List<UserDefinedIngredientInRecipe> userDefinedIngredientInRecipes = new();
            //Go to https://regexr.com/69vn4 for a break down.
            MatchCollection ingredientsWithUnitsRegex = Regex.Matches(inputText, INGREDIENTSWITHCOUNTREGEX, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));

            var bag = new ConcurrentBag<object>();
            var tasks = ingredientsWithUnitsRegex.Select(async match =>
            {
                // some pre stuff
                string unit = "";
                string ingredientName = match.Value;

                MatchCollection units = Regex.Matches(match.Value, UNITSFROMTEXT, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (units.Count == 1)
                {
                    unit = TextProcessor.ConvertStringToSUPPORTEDUNITS(units[0].Value);
                }
                else
                {
                    unit = "x";
                }
                ingredientName = Regex.Replace(ingredientName, UNITSFROMTEXT, "");
                double quantity = TextProcessor.ExtractDouble(match.Value);
                ingredientName = ingredientName.Replace(quantity.ToString(), "").Trim();

                uint? ingredientID = await GetIngredientID(ingredientName, insertIngredientOnEmptyResult);
                if (ingredientID != null)
                {
                    UserDefinedIngredientInRecipe userDefinedIngredientInRecipe = new UserDefinedIngredientInRecipe();
                    userDefinedIngredientInRecipe.IngredientID = ingredientID;
                    userDefinedIngredientInRecipe.Quantity = quantity;
                    userDefinedIngredientInRecipe.Unit = unit;
                    //userDefinedIngredientInRecipes.Add(userDefinedIngredientInRecipe);
                    bag.Add(userDefinedIngredientInRecipe);
                }
                
                // some post stuff
            });
            await Task.WhenAll(tasks);
            List<UserDefinedIngredientInRecipe> bagResults  = new List<UserDefinedIngredientInRecipe>();
            foreach(UserDefinedIngredientInRecipe item in bag)
            {
                bagResults.Add(item);
            }
            return bagResults; //userDefinedIngredientInRecipes;
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
        public async Task<List<UserDefinedIngredientInRecipe>> GetIngredientsWithoutUnit(string textAreaWithoutTimes, bool insertIngredientOnEmptyResult)
        {
            List<UserDefinedIngredientInRecipe> ingredientInRecipes = new();
            // + UNITS + "|"
            MatchCollection ingredientsWithNumber = Regex.Matches(textAreaWithoutTimes, INGREDIENTSWITHOUTUNITREGEX, RegexOptions.Compiled, new TimeSpan(10000000));
            foreach (Match match in ingredientsWithNumber)
            {
                UserDefinedIngredientInRecipe userDefinedIngredientInRecipe = new();
                userDefinedIngredientInRecipe.Quantity = TextProcessor.extractMaximumNumber(match.Value);
                userDefinedIngredientInRecipe.Unit = "x";
                userDefinedIngredientInRecipe.IngredientID = await GetIngredientID(match.Value.Replace(userDefinedIngredientInRecipe.Quantity.ToString(), "").Trim(), insertIngredientOnEmptyResult);
                ingredientInRecipes.Add(userDefinedIngredientInRecipe);
            }
            return ingredientInRecipes;
        }

        public async Task<string> CreateDescription(string inputText)
        {
            //Remove all occurances of a time related numeric value.
            //This prevents the ingredietns without units from picking them up.
            inputText = Regex.Replace(inputText, SERVINGREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            inputText = Regex.Replace(inputText, INGREDIENTSWITHCOUNTREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            inputText = Regex.Replace(inputText, METHODREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            inputText = Regex.Replace(inputText, SERVINGREGEX, "", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            return await RemoveUneccessaryLinesFromDescription(inputText);
        }
        public string GetTitle(string input)
        {
            foreach(string line in input.Split("\n"))
            {
                if (line != null)
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
            description = inputText;
            //see https://regexr.com/69vg0

            newRecipe.Servings = await Task.Run(() => GetServingCount(inputText));

            newRecipe.Method = await Task.Run(() => GetMethods(inputText));

            newRecipe.Description = await Task.Run(() => CreateDescription(inputText));

            newRecipe.Difficulty = await Task.Run(() => GetDifficulty(newRecipe.Method));
            Task<List<UserDefinedIngredientInRecipe>> getIngredientsWithUnits =  Task.Run(() => GetIngredientsWithUnits(inputText, true));
            Task<List<UserDefinedIngredientInRecipe>> getIngredientsWithoutUnits = Task.Run(() => GetIngredientsWithoutUnit(newRecipe.Description, true));

            string possibleNouns = newRecipe.Description;
            foreach (Method s in newRecipe.Method)
            {
                possibleNouns += s.MethodText + "\n ";
            }

            //Dependent on description and method being complete!
            newRecipe.Equipment = await Task.Run(() => GetEquipmentID(possibleNouns));
            await Task.Delay(10);
            getIngredientsWithoutUnits.Wait();
            getIngredientsWithUnits.Wait();
            newRecipe.Ingredients.AddRange(getIngredientsWithoutUnits.Result);
            newRecipe.Ingredients.AddRange(getIngredientsWithUnits.Result);
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
                if (typeOf != null && (typeOf.typeOf.Contains("equipment") || typeOf.typeOf.Contains("kitchen appliance") || typeOf.typeOf.Contains("utensil") || typeOf.typeOf.Contains("electronic equipment") || typeOf.typeOf.Contains("mixer")))
                {
                    string noun = typeOf.word;
                    IEnumerable<Equipment> equipment = await _dataLoader.FindEquipmentLike(noun);
                    if (equipment.Any())
                    {
                        foreach (Equipment equipment1 in equipment)
                            if (TextProcessor.LevenshteinDistance(noun, equipment1.EquipmentName) <= TextProcessor.CalculateLevenshteinThreshold(noun) && !insertMade)
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
            IEnumerable<UserDefinedIngredient> ingredientsInDB = await _dataLoader.FindIngredients(ingredientName);

            if (ingredientsInDB.Any())
            {
                foreach (UserDefinedIngredient userDefinedIngredient in ingredientsInDB)
                {
                    if (TextProcessor.LevenshteinDistance(ingredientName, userDefinedIngredient.IngredientName) <= TextProcessor.CalculateLevenshteinThreshold(ingredientName)) // i.e. only find a ingredient that is very similar to the cureent ingedient. The threshold is designed to prevent erreoneous results at the low end e.g. eggs => egg etc, but having a small ammount of flexibility at the top end.
                    {
                        return userDefinedIngredient.IngredientID;
                    }
                }
            }
            return null;
        }

        private Dictionary<string, Task<uint?>> ingredientTaskDictionary = new();

        public async Task<uint?> GetIngredientID(string ingredientName, bool insertIngredientOnEmptyResult)
        {
            ingredientName = ingredientName.Replace("•	", "");
            uint? ingredientId;
            if (ingredientTaskDictionary.ContainsKey(ingredientName))
            {
                return await ingredientTaskDictionary[ingredientName];
            }
            else
            {
                Task<uint?> task = FindIngredientInDatabase(ingredientName);
                ingredientTaskDictionary.Add(ingredientName, task);
                await task;
                ingredientId = task.Result ;
                if (ingredientId == null && insertIngredientOnEmptyResult)
                {
                    //The ingredient has not been found. First, find all the nouns in the text. Then call call wordsAPI to find which type the ingredient belongs to.
                    UserDefinedIngredient.Type type = UserDefinedIngredient.Type.None;
                    List<string> nouns = ingredientName.Split(" ").ToList();// await _nounExtractor.ExtractNouns(ingredientName);
                    if (nouns != null)
                    {

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
                        foreach (TypeOf item in bag.ToArray())
                        {
                            type |= UserDefinedIngredient.GetTypeEnum(item);
                        }
                    }
                    UserDefinedIngredient userDefined = new();
                    userDefined.IngredientName = ingredientName;
                    userDefined.TypeOf = type;
                    return await _dataLoader.InsertIngredient(userDefined);
                }
                else
                {
                    return ingredientId;
                }
            }
            
           
        }
    }

    public interface ITextProcessor
    {
        public Task<Recipe> CreateRecipe(string inputText);
    }
}
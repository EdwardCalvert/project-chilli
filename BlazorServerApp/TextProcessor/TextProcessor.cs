using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BlazorServerApp;
using BlazorServerApp.Models;
using BlazorServerApp.WordsAPI;

namespace BlazorServerApp.TextProcessor
{
    public class TextProcessor
    {

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

        static int extractMaximumNumber(String str)
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

        static double ConvertTextFractionToDouble(string input)
        {
            string[] fraction = input.Split("\\");
            int numerator = TextProcessor.extractMaximumNumber(fraction[0]);
            int denominator = TextProcessor.extractMaximumNumber(fraction[1]);
            return numerator / denominator;
        }

        static double ExtractDouble(string input)
        {

            if (Regex.IsMatch(input, "[0-9]{1,2} [0-9]\\/[0-9]"))// Matches fractions in the form 1 1/2 etc
            {
                //then return. 
                //split(" ") then find the largest number in [0]
                //at index two split("/"), find the largest number in each section.
                string[] splitInputOnSpace = input.Split(" ");
                int number = TextProcessor.extractMaximumNumber(splitInputOnSpace[0]);
                return number + ConvertTextFractionToDouble(splitInputOnSpace[1]);
            }
            else if (Regex.IsMatch(input, "[0-9]\\/[0-9]")) // matches fractions like 1/2 1/4 etc 
            {
                return ConvertTextFractionToDouble(input);
            }
            else if (Regex.IsMatch(input, "[0-9]{1,3}\\.[0-9]{1,2}}")) // Matches decimals like 1.3 etc
            {
                return double.Parse(input);
            }
            else if (Regex.IsMatch(input, "[0-9] ?(½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)"))
            {
                int integerComponent = TextProcessor.extractMaximumNumber(input);
                //Find which fraction was in the input. Find the value of the number. Add 
                foreach(KeyValuePair<string, double> keyValuePair in doubleEquivalent)
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
                return TextProcessor.extractMaximumNumber(input);
            }
        }

        static string RemoveUneccessaryLinesFromDescription(string description)
        {
            string previousLine = "";
            string newDescription = "";
            description = description.Replace("Ingredients", "").Replace("Method","");
            foreach(string loopLine in description.Split("\n"))
             {
                string line = loopLine.Trim();
                if (previousLine.Length <= 2 && line.Length <= 2)
                {
                    previousLine = line;
                }
                else
                {
                    newDescription += line + "\n";
                    previousLine = line;
                }
            }
            return newDescription;
        }

        public static Dictionary<string, double> doubleEquivalent = new Dictionary<string, double>()
        {
            {"½",0.5 },{"⅓",0.33 },{"⅔",0.66},{"¼",0.25},{"⅕",0.2 },{"⅗",0.6 },{"⅘",0.8 },{"⅙",0.16 },{"⅚",0.83 },{"⅐",0.14},{"⅛",0.125 },{"⅜",0.325 },{"⅝",0.625 },{"⅞",0.875 },{"⅑",0.11},{"⅒",0.10},
        };
        

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

            
            foreach(KeyValuePair<string,string>  keyValuePair in ReplaceShortHand)
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

        const string SPOONS = "((rounded|heaped|level) )?((tea|desert|table)? ?(spoonful|spoon|tbsp|tsp))s?";
        const string UNITS = "(((fluid ounce|ounce|oz|kilograms|cup|gram|pint|pound|quart|kg|g|ml|fl\\.|gal\\.|lb\\.|pt|qt\\.)s?)|(" + SPOONS + "))"; //sort by length- ie teaspoons before teaspoon
        const string NUMBERSWITHFRACTIONS = "([0-9]|½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)";
        const string TIMEWORDS = "(minutes|minute|mins|min|hours|hour|seconds|second|days|day)";
        const string INGREDIENTSSEACH = "[0-9]{0,3} ?" + NUMBERSWITHFRACTIONS + "(\\.|\\/| |-)?[0-9]{0,6} ?" + UNITS + " [^\\.\\;\n\t]{3,200}";
        const string UNITSFROMTEXT = "(?<=" +NUMBERSWITHFRACTIONS + ") ?" + UNITS + " ";

        public async static Task<Recipe> CreateRecipe(IRecipeDataLoader dataLoader,string inputText,IWordsAPIService wordsAPIService)
        {
            if (inputText.Length < 50)
            {
                throw new Exception("That can't be a valid recipe!");
            }
            string description;
            Recipe newRecipe = new Recipe();
            newRecipe.RecipeName = inputText.Split("\n")[0];
            description = inputText;
            //see https://regexr.com/69vg0

            //Remove all occurances of a time related numeric value.
            //This prevents the ingredietns without units from picking them up.
            string textAreaWithoutTimes = Regex.Replace(inputText, "([1-9][0-9]?( (to|-|—) ))?[1-9][0-9]? " + TIMEWORDS,"", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            
            //https://regexr.com/69vg0
            MatchCollection servingRegularExpression = Regex.Matches(inputText, "((serves|makes) ?[1-9][0-9]? ?(to|-)? ?[1-9]?[0-9]?)|([1-9][0-9]? ?(to|-)? ?[1-9]?[0-9]? ?(servings|serving|portions|portion))", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach (Match match in servingRegularExpression)
            {
                textAreaWithoutTimes = description.Replace(match.Value, "");
            }
            if (servingRegularExpression.Count == 1)
            {
                newRecipe.Servings = extractMaximumNumber(servingRegularExpression[0].Value);
            }

            //Go to https://regexr.com/69vqp to understand how this works
            MatchCollection methodMatchCollection = Regex.Matches(inputText, "[1-9][0-9]{0,3}\\.(\t| )[^\t\n\r]{10,500}", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach (Match match in methodMatchCollection)
            {
                description = description.Replace(match.Value, "");
                textAreaWithoutTimes = description.Replace(match.Value, "");
                Method method = new Method();
                
                method.MethodText = Regex.Replace(match.Value, "[1-9][0-9]{0,3}\\.(\t| )", "");
                newRecipe.Method.Add(method);
            }

            //Go to https://regexr.com/69vn4 for a break down. 
            MatchCollection ingredientsWithUnitsRegex = Regex.Matches(inputText, INGREDIENTSSEACH, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach (Match match in ingredientsWithUnitsRegex)
            {
                string unit = "";
                string ingredientName = match.Value;
                
                MatchCollection units = Regex.Matches(match.Value, UNITSFROMTEXT, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (units.Count == 1)
                {

                    unit = TextProcessor.ConvertStringToSUPPORTEDUNITS( units[0].Value);
                    //ingredientName.Replace(unit, "");
                }
                else
                {
                    unit = "x";
                }
                ingredientName = Regex.Replace(ingredientName, UNITSFROMTEXT, "");
                double quantity = TextProcessor.ExtractDouble(match.Value);
                 ingredientName = ingredientName.Replace(quantity.ToString(),"").Trim();
                
                UserDefinedIngredientInRecipe userDefinedIngredientInRecipe = new UserDefinedIngredientInRecipe();
                userDefinedIngredientInRecipe.IngredientID = await GetIngredientID(dataLoader,ingredientName,wordsAPIService);
                userDefinedIngredientInRecipe.Quantity = quantity;
                userDefinedIngredientInRecipe.Unit = unit;
                newRecipe.Ingredients.Add(userDefinedIngredientInRecipe);

                description = description.Replace(match.Value, "");
                textAreaWithoutTimes = description.Replace(match.Value, "");
            }

            // + UNITS + "|" 
            MatchCollection ingredientsWithNumber = Regex.Matches(textAreaWithoutTimes, "[1-9][0-9]? ((?!"+ TIMEWORDS + ")[A-z]| |,|\\([^\r\t\\(\\)\n]{2,100}\\)|-){3,150}[a-z]", RegexOptions.Compiled, new TimeSpan(10000000));
            foreach (Match match in ingredientsWithNumber)
            {
                //description = description.Replace(match.Value, "");
                UserDefinedIngredientInRecipe userDefinedIngredientInRecipe = new UserDefinedIngredientInRecipe();
                userDefinedIngredientInRecipe.Quantity = TextProcessor.extractMaximumNumber(match.Value);
                userDefinedIngredientInRecipe.Unit = "x";
                userDefinedIngredientInRecipe.IngredientID = await TextProcessor.GetIngredientID(dataLoader, match.Value.Replace(userDefinedIngredientInRecipe.Quantity.ToString(),"").Trim(),wordsAPIService);
                newRecipe.Ingredients.Add(userDefinedIngredientInRecipe);
            }
            newRecipe.Description = TextProcessor.RemoveUneccessaryLinesFromDescription( description);

            if(newRecipe.Method.Count < 5)
            {
                newRecipe.Difficulty = "Easy";
            }
            else if(newRecipe.Method.Count > 10)
            {
                newRecipe.Difficulty = "Hard";
            }
            else
            {
                newRecipe.Difficulty = "Medium";
            }

            newRecipe.Equipment = await GetEquipmentID(dataLoader,inputText, wordsAPIService);
            return newRecipe;
        }

        public static bool DoesEquipmentAreadyHaveID(List<Equipment> equipment, string equipmentName)
        {
            foreach(Equipment equipment1 in equipment)
            {
                if(equipment1.EquipmentName == equipmentName)
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<List<Equipment>> GetEquipmentID(IRecipeDataLoader dataLoader, string descriptonToSearch, IWordsAPIService wordsAPIService)
        {
            List<Equipment> EquipmentIDs = new();
            List<string> nouns = await NounExtractor.ExtractNouns(descriptonToSearch);
            foreach (string noun in nouns)
            {
                bool insertMade = false;
                TypeOf typeOf = await wordsAPIService.CallCachedAPI(noun);
                if (typeOf != null && (typeOf.typeOf.Contains("equipment")|| typeOf.typeOf.Contains("kitchen appliance") || typeOf.typeOf.Contains("utensil")))
                {
                   IEnumerable<Equipment> equipment =  await dataLoader.FindEquipmentLike(noun);
                    if(equipment.Any())
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
                        equipment1.EquipmentID = await dataLoader.InsertEquipment(equipment1);
                        EquipmentIDs.Add(equipment1);
                        
                    }
                }
            }
            return EquipmentIDs; 
        }

        public static async Task<uint> GetIngredientID(IRecipeDataLoader dataLoader, string ingredientName,IWordsAPIService wordsAPIService)
        {
            //Find any existing ingredients
            IEnumerable<UserDefinedIngredient> ingredientsInDB = await dataLoader.FindIngredients(ingredientName);
            bool foundIngredient = false;
            if (ingredientsInDB.Any())
            {
                foreach (UserDefinedIngredient userDefinedIngredient in ingredientsInDB)
                {
                    if (!foundIngredient && TextProcessor.LevenshteinDistance(ingredientName, userDefinedIngredient.IngredientName) <= TextProcessor.CalculateLevenshteinThreshold(ingredientName)) // i.e. only find a ingredient that is very similar to the cureent ingedient. The threshold is designed to prevent erreoneous results at the low end e.g. eggs => egg etc, but having a small ammount of flexibility at the top end.
                    {
                        foundIngredient = true;
                        return (uint)userDefinedIngredient.IngredientID;
                    }
                }
            }
            if (!foundIngredient)
            {
                //The ingredient has not been found. First, find all the nouns in the text. Then call call wordsAPI to find which type the ingredient belongs to. 
                UserDefinedIngredient.Type type = UserDefinedIngredient.Type.None;
                List<string> nouns = await NounExtractor.ExtractNouns(ingredientName);
                if (nouns != null)
                {
                    foreach (string noun in nouns)
                    {
                        TypeOf typeOf = await wordsAPIService.CallCachedAPI(noun);
                        type |= UserDefinedIngredient.GetTypeEnum(typeOf);
                    }
                }
                UserDefinedIngredient userDefined = new ();
                userDefined.IngredientName = ingredientName;
                userDefined.TypeOf = type;
                return await dataLoader.InsertIngredient(userDefined);
            }
            return 0; //Hopefully will never be run!
        }
    }  
}

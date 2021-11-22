using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BlazorServerApp;
using BlazorServerApp.Models;

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

        static double ExtractDouble(string input)
        {
            // Replace ½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒ with decimal
            //

            if (Regex.IsMatch(input, "[0-9]{1,2} [0-9]\\/[0-9]"))// Matches fractions in the form 1 1/2 etc
            {
                //then return. 
            }
            else if (Regex.IsMatch(input, "[0-9]\\/[0-9]")) // matches fractions like 1/2 1/4 etc 
            {

            }
            else if (Regex.IsMatch(input, "[0-9]{1,3}\\.[0-9]{1,2}}")) // Matches decimals like 1.3 etc
            {

            }
            else if (Regex.IsMatch(input, "[0-9] ?(½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)"))
            {
                int integerComponent = TextProcessor.extractMaximumNumber(input);
                //Find which fraction was in the input. Find the value of the number. Add 
                foreach(KeyValuePair<string, double> keyValuePair in doubleEquivalent)
                {
                    if (input.Contains(keyValuePair.Key)) // It has 
                    {

                    }
                }
            }
            else //I really have no idea what input they attempted, so I will just choose the largest number.
            {
                return TextProcessor.extractMaximumNumber(input);
            }

            /*Flow
             if there is only one block of numbers, then it must be a number up to  7 digits
            if the text contains a - then we are looking at a range, so choose the maximum
            if there is the division symbol, then looking at a manual fraction => check the number of  digits (since it could be 1 1/2 etc)
             
             */


            //How on earth do you convert 1 1/2?
        }

        public static Dictionary<string, double> doubleEquivalent = new Dictionary<string, double>()
        {
            {"½",0.5 },{"⅓",0.33 },{"⅔",0.66},{"¼",0.25},{"⅕",0.2 },{"⅗",0.6 },{"⅘",0.8 },{"⅙",0.16 },{"⅚",0.83 },{"⅐",0.14},{"⅛",0.125 },{"⅜",0.325 },{"⅝",0.625 },{"⅞",0.875 },{"⅑",0.11},{"⅒",0.10},
        };

        public async static Task<Recipe> CreateRecipe(IRecipeDataLoader dataLoader,string inputText)
        {
        List<string> ingredientsWithCount = new List<string>();
        List<string> ingredients = new List<string>();
        List<string> times = new List<string>();
        List<string> servings = new();
        List<string> methods = new();

        string description;
            Recipe newRecipe = new Recipe();
            newRecipe.RecipeName = inputText.Split("\n")[0];
            description = inputText;
            //see https://regexr.com/69vg0
            const string SPOONS = "((rounded|heaped|level) )?((tea|desert|table)? ?(spoonful|spoon|tbsp|tsp))s?";
            const string UNITS = "(((fluid ounce|ounce|oz|kilograms|cup|cup|gram|pint|pound|quart|g|kg|ml|fl\\.|gal\\.|lb\\.|pt|qt\\.|bunch|handful)s?)|(" + SPOONS + "))"; //sort by length- ie teaspoons before teaspoon
            const string NUMBERSWITHFRACTIONS = "([0-9]|½|⅓|⅔|¼|¾|⅕|⅖|⅗|⅘|⅙|⅚|⅐|⅛|⅜|⅝|⅞|⅑|⅒)";
            const string TIMEWORDS = "(minutes|minute|mins|min|hours|hour|seconds|second|days|day)";
            const string ingredientsSearch = "[0-9]{0,3} ?" + NUMBERSWITHFRACTIONS + "(\\.|\\/| |-)?[0-9]{0,6} ?" + UNITS + " [^\\.\\;\n\t]{3,200}";


            MatchCollection timeRegex = Regex.Matches(inputText, "([1-9][0-9]?( (to|-|—) ))?[1-9][0-9]? " + TIMEWORDS, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            string textAreaWithoutTimes = "";
            foreach (Match match in timeRegex)
            {
                //string unit = "";
                //int totalTimeInMinutes = 0;

                ////if(match.Value.Contains(minutes|minute|mins|mi))
                //MatchCollection units = Regex.Matches(match.Value, TIMEWORDS, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                //if (units.Count == 1)
                //{
                //    unit = units[0].Value;
                //    switch (unit)
                //    {
                //        case "minutes":
                //        case "minute":
                //        case "mins":
                //        case "min":
                //            //add numerical value to total time;
                //            break;
                //        case "hours":
                //        case "hour":
                //            break;
                //        case "second":
                //        case "seconds":
                //            break;
                //        case "day":
                //        case "days":
                //            break;

                //    }
                //}
                textAreaWithoutTimes = description.Replace(match.Value, "");
                times.Add(match.Value);
            }

            //https://regexr.com/69vg0
            MatchCollection servingRegularExpression = Regex.Matches(inputText, "((serves|makes) ?[1-9][0-9]? ?(to|-)? ?[1-9]?[0-9]?)|([1-9][0-9]? ?(to|-)? ?[1-9]?[0-9]? ?(servings|serving|portions|portion))", RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach (Match match in servingRegularExpression)
            {
                //description = description.Replace(match, "");
                textAreaWithoutTimes = description.Replace(match.Value, "");
                servings.Add(match.Value);
            }
            if (servings.Count == 1)
            {
                newRecipe.Servings = extractMaximumNumber(servings[0]);
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
            MatchCollection ingredientsWithUnitsRegex = Regex.Matches(inputText, ingredientsSearch, RegexOptions.Compiled | RegexOptions.IgnoreCase, new TimeSpan(10000000));
            foreach (Match match in ingredientsWithUnitsRegex)
            {
                string unit = "";
                MatchCollection units = Regex.Matches(match.Value, UNITS, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (units.Count == 1)
                {
                    unit = units[0].Value.Trim();
                }
                else
                {
                    unit = "x";
                }

                double quantity = 8888;
                string ingredientName = "";
                
                ///
                IEnumerable<UserDefinedIngredient> ingredientsInDB =  await dataLoader.FindIngredients(ingredientName);
                bool insertMade = false;
                if (ingredientsInDB.Count() >= 0)
                {
                    foreach (UserDefinedIngredient userDefinedIngredient in ingredientsInDB)
                    {
                        if (TextProcessor.LevenshteinDistance(ingredientName, userDefinedIngredient.IngredientName) < 2 && !insertMade)
                        {
                            UserDefinedIngredientInRecipe userDefinedIngredientInRecipe = new UserDefinedIngredientInRecipe();
                            userDefinedIngredientInRecipe.IngredientID = userDefinedIngredient.IngredientID;
                            userDefinedIngredientInRecipe.Quantity = quantity;
                            newRecipe.Ingredients.Add(userDefinedIngredientInRecipe);
                            insertMade = true;
                        }
                    }
                }
                if(!insertMade)
                {
                    UserDefinedIngredient userDefined = new UserDefinedIngredient();
                    userDefined.IngredientName = ingredientName;
                    uint ingredientID = await dataLoader.InsertIngredient(userDefined);
                    UserDefinedIngredientInRecipe userDefinedIngredientInRecipe = new UserDefinedIngredientInRecipe();
                    userDefinedIngredientInRecipe.IngredientID = ingredientID;
                    userDefinedIngredientInRecipe.Quantity = quantity;
                    newRecipe.Ingredients.Add(userDefinedIngredientInRecipe);
                }
                
                description = description.Replace(match.Value, "");
                textAreaWithoutTimes = description.Replace(match.Value, "");
                ingredientsWithCount.Add(match.Value);
            }

            MatchCollection ingredientsWithNumber = Regex.Matches(textAreaWithoutTimes, "[1-9][0-9]? ((?!" + UNITS + "|" + TIMEWORDS + ")[A-z]| |,|\\([^\r\t\\(\\)\n]{2,100}\\)|-){3,150}[a-z]", RegexOptions.Compiled, new TimeSpan(10000000));
            foreach (Match match in ingredientsWithNumber)
            {
                description = description.Replace(match.Value, "");
                ingredients.Add(match.Value);
            }

            newRecipe.Description = description;
            return newRecipe;
            // description = description.Replace("Ingredients", "Ingredients- download recipe for a more comprehensive view");
        }
    }
}

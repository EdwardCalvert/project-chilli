using BlazorServerApp.TextProcessor;
using BlazorServerApp.WordsAPI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class Ingredient
    {
        IWordsAPIService? _wordsAPIService;
        INounExtractor? _nounExtractor;
        bool reevaluateTypes = false;
        public Ingredient(IWordsAPIService? words = null, INounExtractor? nounExtractor = null)
        {
            if (words != null && nounExtractor != null)
            {
                _debounceTimer = new Timer();
                _debounceTimer.Interval = 300;
                _debounceTimer.AutoReset = false;
                _debounceTimer.Elapsed += InvokeNameChange;
                reevaluateTypes = true;
                _wordsAPIService = words;
                _nounExtractor = nounExtractor;
               
            }
            else
            {
                reevaluateTypes = false;
            }
        }

        public Ingredient() { }
        private Timer _debounceTimer;
        public uint? IngredientID { get; set; }
        public uint RecipeID { get; set; }
        public string IngredientName { get { return _ingredientName; } set { _ingredientName = value;
                if (reevaluateTypes)
                {
                    if (value.Length == 0)
                    {
                        _debounceTimer.Stop();
                    }
                    else
                    {
                        _debounceTimer.Stop();
                        _debounceTimer.Start();
                    }
                }
            } }
        private string _ingredientName;
        public EventCallback<Ingredient> nameChanged { get; set; }

        private async void InvokeNameChange(Object source, ElapsedEventArgs e)
        {
            TypeOf = Type.None;
            foreach(string noun in  await _nounExtractor.ExtractNouns(IngredientName))
            {
                TypeOf response = await _wordsAPIService.CallCachedAPI(noun);
                TypeOf |= GetTypeEnum(response);
            }
            await nameChanged.InvokeAsync();

            ReRender.Invoke();

        }

        public Action ReRender { get; set; }
        public Type TypeOf { get; set; }

        [System.Flags]
        public enum Type : ushort
        {
            None = 0,
            Poultry = 1,
            Meat = 1 << 1,
            Fish = 1 << 2,
            Fruit = 1 << 3,
            Vegetables = 1 << 4,
            Nuts = 1 << 5,
            Milk = 1 << 6,
            Egg = 1 << 7,
            Dairy = 1 << 8,
        }

        public const Type AllSelected= Type.Poultry | Type.Meat | Type.Fish | Type.Fruit | Type.Vegetables | Type.Nuts | Type.Milk | Type.Egg | Type.Dairy;

        public static Type GetTypeEnum(TypeOf typeOf)
        {
            if (typeOf != null && typeOf.typeOf != null)
            {
                Type type = Type.None;
                foreach (string s in typeOf.typeOf)
                {
                    type |= GetTypeEnum(s);
                }
                return type;
            }
            else
            {
                return Type.None;
            }
        }
        public static Type GetTypeEnum(string s)
        {
            Type type = Type.None;
            if (s.Contains("meat"))
            {
                type |= Type.Meat;
            }
            if (s.Contains("poultry"))
            {
                type |= Type.Poultry;
            }
            if (s.Contains("milk"))
            {
                type |= Type.Milk;
            }
            if (s.Contains("fish"))
            {
                type |= Type.Fish;
            }
            if (s.Contains("fruit"))
            {
                type |= Type.Fruit;
            }
            if (s.Contains("vegetable"))
            {
                type |= Type.Vegetables;
            }
            if (s.Contains("egg"))
            {
                type |= Type.Egg;
            }
            if (s.Contains("dairy"))
            {
                type |= Type.Dairy;
            }
            return type;
        }

        public string SqlInsertStatement()
        {
            return "INSERT INTO Ingredient(RecipeID,IngredientName,TypeOf) VALUES(@RecipeID,@ingredientName,@TypeOf);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { ingredientName = IngredientName, TypeOf = TypeOf , RecipeID = RecipeID,IngredientID = IngredientID};
        }
    }
}
using BlazorServerApp.WordsAPI;
using System.Collections.Generic;

namespace BlazorServerApp.Models
{
    public class UserDefinedIngredient
    {
        public uint? IngredientID { get; set; }
        public string IngredientName { get; set; }

        public Type TypeOf { get; set; }

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
            Vegan = Vegetables | Fruit | Nuts,
            Vegetarian = Vegan | Dairy | Egg,
        }

        public const Type AllSelected = Type.Poultry | Type.Meat | Type.Fish | Type.Fruit | Type.Vegetables | Type.Nuts | Type.Milk | Type.Egg | Type.Dairy;
        public const Type Veganism = Type.Vegetables | Type.Fruit | Type.Nuts;
        public static Dictionary<string, string[]> ComplexTypes = new() { { "Vegan", new string[3] { "Vegetables", "Fruit", "Nuts" } }, { "Vegetarian", new string[7] { "Vegan", "Dairy", "Egg", "Vegetables", "Fruit", "Milk", "Nuts" } } };
        public const Type Vegetarianism = Veganism | Type.Dairy | Type.Egg;

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
            return "INSERT INTO UserDefinedIngredients(IngredientName,TypeOf) VALUES(@ingredientName,@TypeOf);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { ingredientName = IngredientName, TypeOf = TypeOf };
        }
    }
}
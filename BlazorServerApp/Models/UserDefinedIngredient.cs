using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.WordsAPI;

namespace BlazorServerApp.Models
{
    public class UserDefinedIngredient
    {
         public uint? IngredientID { get; set; }
        public string IngredientName { get; set; }

        public Type TypeOf { get; set; }

        public enum Type
        {
            None = 0,
            Poultry = 1,
            Meat = 1<<1,
            Fish = 1 <<2,
            Fruit = 1<<3,
            Vegetables  = 1<<4,
            Nuts = 1<<5,
            Milk = 1<<6,

        }

        public static Type GetTypeEnum(TypeOf typeOf)
        {
            
            if (typeOf != null) {
                Type type = Type.None;
                foreach (string s in typeOf.typeOf)
                {
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
                }
                return type;

             }
            else
            {
                return Type.None;
            }
        }


        public string SqlInsertStatement()
        {
            return "INSERT INTO UserDefinedIngredients(IngredientName,TypeOf) VALUES(@ingredientName,@TypeOf);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { ingredientName = IngredientName,TypeOf = TypeOf};
        }
    }
}

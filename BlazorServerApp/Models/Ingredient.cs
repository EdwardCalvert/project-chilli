using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace BlazorServerApp.Models
{
    public class Ingredient : IEnumerable
    {

        public uint IngredientID { get; set; }
        public string FoodCode { get; set; }
        public string IngredientName { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Kcal { get; set; }
        public double Starch { get; set; }
        public double TotalSugars { get; set; }
        public double Glucose { get; set; }
        public double Fructose { get; set; }
        public double Sucrose { get; set; }
        public double Maltose { get; set; }
        public double Lactose { get; set; }
        public double Alchohol { get; set; }
        public double NSP { get; set; }
        public double Fibre { get; set; }
        public double SaturatedFattyAcids { get; set; }
        public double PolyunsaturatedFattyAcids { get; set; }
        public double MonounsaturatedFattyAcids { get; set; }
        public double Cholesterol { get; set; }
        public string AlternateName { get; set; }

        private static List<string> DoubleProperties = new() { "Protein", "Fat", "Carbohydrates", "Kcal", "Starch", "TotalSugars", "Glucose", "Fructose", "Sucrose", "Maltose", "Lactose", "Alchohol", "NSP","Fibre", "SaturatedFattyAcids", "MonounsaturatedFattyAcids", "PolyunsaturatedFattyAcids", "Cholesterol" };

        public string SqlInsertStatement()
        {
            return "INSERT INTO Ingredients(FoodCode,IngredientName,Protein,Fat,Carbohydrates,Kcal,Starch,TotalSugars, Glucose, Fructose, Sucrose, Maltose, Lactose, Alchohol, NSP, Fibre, SaturatedFattyAcids, MonounsaturatedFattyAcids, PolyunsaturatedFattyAcids, Cholesterol,AlternateName) VALUES(@foodCode,@ingredientName,@protein,@fat,@carbohydrates,@kcal,@starch,@totalSugars, @glucose, @fructose, @sucrose, @maltose, @lactose, @alchohol, @nSP, @fibre,@saturatedFattyAcids, @monounsaturatedFattyAcids, @polyunsaturatedFattyAcids, @cholesterol,@alternateName)";
        }

        public dynamic SqlAnonymousType()
        {
            return new { foodCode = FoodCode, ingredientName = IngredientName, protein = Protein, @fat = Fat, @carbohydrates = Carbohydrates, @kcal = Kcal, @starch = Starch, @totalSugars = TotalSugars, @glucose = Glucose, @fructose = Fructose, @sucrose = Sucrose, @maltose = Maltose, @lactose = Lactose, @alchohol = Alchohol, @nSP = NSP, fibre = Fibre, @saturatedFattyAcids = SaturatedFattyAcids, @monounsaturatedFattyAcids = MonounsaturatedFattyAcids, @polyunsaturatedFattyAcids = PolyunsaturatedFattyAcids, cholesterol = Cholesterol,alternateName = AlternateName };
        }

        public IEnumerator GetEnumerator()
        { 
            foreach(string PropertyName in DoubleProperties)
            {
                yield return PropertyName;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

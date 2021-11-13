using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace BlazorServerApp.Models
{
    public class NutritionStructure : IEnumerable
    {
        public double Kcal { get; set; }
        public double Fat { get; set; }
        public double Saturates { get; set; }
        public double Sugar { get; set; }
        public double Fibre { get; set; }
        public double Carbohydrates { get; set; }
        public double Salt { get; set; }


        public NutritionStructure(double kcal, double fat, double saturates, double sugar, double fibre, double carbohydrates, double salt)
        {
            Kcal = kcal;
            Fat = fat;
            Saturates = saturates;
            Sugar = sugar;
            Fibre = fibre;
            Carbohydrates = carbohydrates;
            Salt = salt;
        }

        private List<string> properties = new() { "Fat", "Saturates", "Sugar", "Carbohydrates", "Salt" };

        public bool IsEmpty()
        {
            return Kcal == 0 && Fat == 0 && Saturates ==0 && Sugar == 0 && Fibre ==0 && Carbohydrates == 0 && Salt ==0;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (string PropertyName in properties)
            {
                yield return PropertyName;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class RecomendedIntake : NutritionStructure
    {
        public RecomendedIntake(double kcal, double fat, double saturates, double sugar, double fibre, double carbohydrates, double salt) : base(kcal, fat, saturates, sugar, fibre, carbohydrates, salt)
        {

        }


    }

    public class DisplayNutritionModel : NutritionStructure
    {
        private RecomendedIntake RecomendedIntake;

        public DisplayNutritionModel(double kcal, double fat, double saturates, double sugar, double fibre, double carbohydrates, double salt, RecomendedIntake recomendedIntake) : base(kcal, fat, saturates, sugar, fibre, carbohydrates, salt)
        {
            RecomendedIntake = recomendedIntake;
        }

        public double GetProperty(string propertyName)
        {
            return (double)this.GetType().GetProperty(propertyName).GetValue(this, null);
        }

        public double GetRecomendedIntake(string propertyName)
        {
            return (double)RecomendedIntake.GetType().GetProperty(propertyName).GetValue(RecomendedIntake, null);
        }

        public int GetPercentage(string propertyName)
        {
            return (int)((GetProperty(propertyName)/GetRecomendedIntake(propertyName) )*100);
        }

        public string Colour(int percent)
        {
            if(percent<0 )
            {
                return "";
            }
            else
            {
                if(percent>25)
                {
                    return "Red";
                }
                else if (percent < 6)
                {
                    return "Green";
                }
                else
                {
                    return "Yellow";
                }
            }
        }
    }
}

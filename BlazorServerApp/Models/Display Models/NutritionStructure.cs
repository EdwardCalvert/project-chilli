using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class NutritionStructure 
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

        public bool IsEmpty()
        {
            return Kcal == 0 && Fat == 0 && Saturates ==0 && Sugar == 0 && Fibre ==0 && Carbohydrates == 0 && Salt ==0;
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

        public int SaltPercentage()
        {
            return (int)((Salt / RecomendedIntake.Salt) * 100); 
        }
        public int FatPercentage()
        {
            return (int)((Fat / RecomendedIntake.Fat) * 100);
        }
        public int SaturatePercentage()
        {
            return (int)((Saturates / RecomendedIntake.Saturates) * 100);
        }
        public int FibrePercentage()
        {
            return (int)((Fibre / RecomendedIntake.Fibre)*100);
        }
        public int SugarPercentage()
        {
            return (int)((Sugar / RecomendedIntake.Sugar)*100);
        }
        public int CarbohydratePercentage()
        {
            return (int)((Carbohydrates / RecomendedIntake.Carbohydrates)*100);
        }
        public int EnergyPercentage()
        {
            return (int)((Kcal / RecomendedIntake.Kcal)*100);
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

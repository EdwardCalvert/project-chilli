using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class NutritionStructure 
    {
        public float Kcal { get; set; }
        public float Fat { get; set; }
        public float Saturates { get; set; }
        public float Sugar { get; set; }
        public float Fibre { get; set; }
        public float Carbohydrates { get; set; }
        public float Salt { get; set; }


        public NutritionStructure(float kcal, float fat, float saturates, float sugar, float fibre, float carbohydrates, float salt)
        {
            Kcal = kcal;
            Fat = fat;
            Saturates = saturates;
            Sugar = sugar;
            Fibre = fibre;
            Carbohydrates = carbohydrates;
            Salt = salt;
        }
    }

    public class RecomendedIntake : NutritionStructure
    {
        public RecomendedIntake(float kcal, float fat, float saturates, float sugar, float fibre, float carbohydrates, float salt) : base(kcal, fat, saturates, sugar, fibre, carbohydrates, salt)
        {

        }
    }

    public class DisplayNutritionModel : NutritionStructure
    {
        private RecomendedIntake RecomendedIntake;

        public DisplayNutritionModel(float kcal, float fat, float saturates, float sugar, float fibre, float carbohydrates, float salt, RecomendedIntake recomendedIntake) : base(kcal, fat, saturates, sugar, fibre, carbohydrates, salt)
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

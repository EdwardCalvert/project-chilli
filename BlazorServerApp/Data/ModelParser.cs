using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.Models;
using BlazorServerApp.Extensions;

namespace BlazorServerApp.Data
{
    public class ModelParser
    {
        public static DisplayReviewModel ParseReviewDataModelToDisplayReviewModel(ReviewDataModel reviewDataModel)
        {
            DisplayReviewModel displayReviewModel = new DisplayReviewModel();
            displayReviewModel.ReviewTitle = reviewDataModel.ReviewTitle;
            displayReviewModel.ReviewText = reviewDataModel.ReviewText;
            displayReviewModel.ReviewersName = reviewDataModel.ReviewersName;
            displayReviewModel.Star = Star.CreateStar(reviewDataModel.StarCount);
            displayReviewModel.RecipeID = (int) reviewDataModel.RecipeID;
            displayReviewModel.DateCreated = reviewDataModel.DateSubmitted;
            return displayReviewModel;
        }

        public static List<DisplayReviewModel> ParseReviewDataModelToDisplayReviewModel(List<ReviewDataModel> reviewDataModels)
        {
            List<DisplayReviewModel> displayReviews = new List<DisplayReviewModel>(reviewDataModels.Count);
            foreach(ReviewDataModel review in reviewDataModels)
            {
                displayReviews.Add(ModelParser.ParseReviewDataModelToDisplayReviewModel(review));
            }
            return displayReviews;
        }

        public static DisplayMethodModel ParseMethodDataModelToDisplayMethodModel(MethodDataModel methodDataModel)
        {
            DisplayMethodModel displayMethodModel = new DisplayMethodModel();
            displayMethodModel.Step = methodDataModel.MethodText;
            displayMethodModel.StepNumber = methodDataModel.StepNumber;
            return displayMethodModel;
        }
        public static List<DisplayMethodModel> ParseMethodDataModelToDisplayMethodModel(List<MethodDataModel> methodDataModel)
        {
            List<DisplayMethodModel> methods = new List<DisplayMethodModel>(methodDataModel.Count);
            foreach (MethodDataModel method in methodDataModel)
            {
                methods.Add(ModelParser.ParseMethodDataModelToDisplayMethodModel(method));
            }
            return methods;
        }

        public static ReviewDataModel ParseDisplayReviewModelToDisplayDataModel(DisplayReviewModel displayReviewModel)
        {
            if (displayReviewModel.RecipeID == default) {
                throw new Exception("The RecipeID MUST be provided");
            }
            ReviewDataModel reviewDataModel = new ReviewDataModel();
            reviewDataModel.ReviewersName = displayReviewModel.ReviewersName;
            reviewDataModel.ReviewText = displayReviewModel.ReviewText;
            reviewDataModel.StarCount = displayReviewModel.Star.GetNumberOfStars();
            reviewDataModel.ReviewTitle = displayReviewModel.ReviewTitle;
            reviewDataModel.RecipeID = (uint) displayReviewModel.RecipeID;
            reviewDataModel.DateSubmitted = displayReviewModel.DateCreated;
            return reviewDataModel;
        }

        public static DisplayRecipeModel ParseOnlyRecipeDataModelIntoDisplayRecipeModel(RecipeDataModel recipeDataModel)
        {
            DisplayRecipeModel displayRecipeModel = new DisplayRecipeModel();
            displayRecipeModel.CookingTime = (int)recipeDataModel.CookingTime;
            displayRecipeModel.Servings = (int)recipeDataModel.Servings;
            displayRecipeModel.MealType = (DisplayRecipeModel.mealType)recipeDataModel.MealType;
            displayRecipeModel.RecipeName = recipeDataModel.RecipeName;
            displayRecipeModel.CookingTime = (int)recipeDataModel.CookingTime;
            displayRecipeModel.PreperationTime = (int)recipeDataModel.PreperationTime;
            if (!string.IsNullOrEmpty(recipeDataModel.DocxFilePath))
            {
                displayRecipeModel.DocxFilePath = recipeDataModel.DocxFilePath;
            }

            displayRecipeModel.Description = recipeDataModel.Description;
            displayRecipeModel.RecipeID = recipeDataModel.RecipeID;
            displayRecipeModel.DisplayNutritionModel = new DisplayNutritionModel(recipeDataModel.Kcal, recipeDataModel.Fat, recipeDataModel.Saturates, recipeDataModel.Sugar, recipeDataModel.Fibre, recipeDataModel.Carbohydrates, recipeDataModel.Salt, DisplayRecipeModel.RecomendedIntake);

            return displayRecipeModel;
        }

        public static List<DisplayRecipeModel> ParseOnlyRecipeDataModelIntoDisplayRecipeModel(List<RecipeDataModel> recipeDataModel)
        {
            List<DisplayRecipeModel> recipeModels = new();
            foreach (RecipeDataModel model in recipeDataModel)
            {
                recipeModels.Add(ModelParser.ParseOnlyRecipeDataModelIntoDisplayRecipeModel(model));
            }
            return recipeModels;
        }

        public static RecipeDataModel ParseOnlyDisplayRecipeModelIntoRecipeDataModel(DisplayRecipeModel model)
        {
            RecipeDataModel dataModel = new RecipeDataModel();
            dataModel.CookingTime = (uint)model.CookingTime;
            dataModel.Servings = (uint)model.Servings;
            dataModel.MealType = (RecipeDataModel.MEALTYPE)model.MealType;
            dataModel.RecipeName = model.RecipeName;
            dataModel.CookingTime = (uint)model.CookingTime;
            dataModel.PreperationTime = (uint)model.CookingTime;
            dataModel.DocxFilePath = model.DocxFilePath;
            dataModel.PageVisits = 1;
            dataModel.LastRequested = DateTime.Now;
            dataModel.Description = model.Description;
            return dataModel;
        }



        public static List<RecipeDataModel> ParseOnlyDisplayRecipeModelIntoRecipeDataModel(List<DisplayRecipeModel> frontEndModel)
        {
            List<RecipeDataModel> recipeDataModels = new List<RecipeDataModel>(frontEndModel.Count);
            foreach (DisplayRecipeModel model in frontEndModel)
            {
                recipeDataModels.Add(ParseOnlyDisplayRecipeModelIntoRecipeDataModel(model));
            }
            return recipeDataModels;
        }
        public static MethodDataModel ParseDisplayMethodModelToMethodDataModel(DisplayMethodModel displayMethodModels, uint RecipeID)
        {
            MethodDataModel methodDataModel = new MethodDataModel();
            methodDataModel.StepNumber = displayMethodModels.StepNumber;
            methodDataModel.MethodText = displayMethodModels.Step;
            methodDataModel.RecipeID = RecipeID;
            return methodDataModel;
        }

        public static List<MethodDataModel> ParseDisplayMethodModelToMethodDataModel(List<DisplayMethodModel> displayMethodModels, uint RecipeIDForAllDataModels)
        {
            List<MethodDataModel> methodDataModels = new List<MethodDataModel>();
            foreach(DisplayMethodModel displayMethodModel in displayMethodModels)
            {
                methodDataModels.Add(ParseDisplayMethodModelToMethodDataModel(displayMethodModel, RecipeIDForAllDataModels));
            }
            return methodDataModels;
        }

        public static List<EquipmentDataModel>  ParseDisplayEquipmentModelToEquipmentDataModel(List<DisplayEquipmentModel >displayEquipmentModels)
        {
            List<EquipmentDataModel> dataModels = new List<EquipmentDataModel>(displayEquipmentModels.Count);
            foreach (DisplayEquipmentModel displayEquipmentModel in displayEquipmentModels)
            {
                EquipmentDataModel equipmentDataModel = new EquipmentDataModel();
                equipmentDataModel.EquipmentName = displayEquipmentModel.EquipmentName;
                equipmentDataModel.EquipmentID = displayEquipmentModel.EquipmentID;
                equipmentDataModel.TypeOf = displayEquipmentModel.TypeOf;
                dataModels.Add(equipmentDataModel);
            }
            return dataModels;
        }

 
    }
}

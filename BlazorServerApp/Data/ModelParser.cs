using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.Models;

namespace BlazorServerApp.Data
{
    public class ModelParser
    {
        public static DisplayReviewModel ParseReviewDataModelToDisplayReviewModel(ReviewDataModel reviewDataModel)
        {
            DisplayReviewModel displayReviewModel = new DisplayReviewModel();
            displayReviewModel.ReviewTitle = reviewDataModel.ReviewerTitle;
            displayReviewModel.ReviewText = reviewDataModel.ReviewText;
            displayReviewModel.ReviewersName = reviewDataModel.ReviewersName;
            displayReviewModel.StarCount = reviewDataModel.StarCount;

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
    }
}

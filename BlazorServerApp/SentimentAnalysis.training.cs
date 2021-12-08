﻿﻿// This file was auto-generated by ML.NET Model Builder. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML;

namespace BlazorServerApp
{
    public partial class SentimentAnalysis
    {
        public static ITransformer RetrainPipeline(MLContext context, IDataView trainData)
        {
            var pipeline = BuildPipeline(context);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(@"LoggedIn", @"LoggedIn")      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(@"SentimentText", @"SentimentText"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"LoggedIn",@"SentimentText"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(@"Sentiment", @"Sentiment"))      
                                    .Append(mlContext.Transforms.NormalizeMinMax(@"Features", @"Features"))      
                                    .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(l1Regularization:0.630093109048752F,l2Regularization:6.72162799983559F,labelColumnName:@"Sentiment",featureColumnName:@"Features"))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(@"PredictedLabel", @"PredictedLabel"));

            return pipeline;
        }
    }
}

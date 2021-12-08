using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class Review
    {
        [Required]
        [StringLength(50, ErrorMessage = "Review title is too long.")]
        [MinLength(2, ErrorMessage = "Review title is too short")]
        public string ReviewTitle { get; set; }
        [Required]
        [StringLength(DatabaseConstants.VarCharMax, ErrorMessage = "Review text is too long.")]
        [MinLength(2, ErrorMessage = "Name is too short")]
        public string ReviewText { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(2, ErrorMessage = "Name is too short")]
        public string ReviewersName { get; set; }

        public Star Star { get; set; }

        public uint RecipeID { get; set; }

        public uint ReviewID { get; set; }

        public int StarCount { get; set; }

        public DateTime DateSubmitted { get; set; }

        public string SQLInsertStatement()
        {
            return $"INSERT INTO Review(RecipeID,ReviewersName,ReviewTitle,ReviewText,StarCount,DateSubmitted)  VALUES(@recipeID,@reviewersName,@reviewTitle,@reviewText,@starCount,@dateSubmitted);";
        }

        public dynamic SQLAnonymousType()
        {
            return new { recipeID = RecipeID, reviewersName = ReviewersName, reviewTitle = ReviewTitle, reviewText = ReviewText, starCount = Star.GetNumberOfStars(), dateSubmitted = RecipeDataLoader.MySQLTimeFormat(DateSubmitted) };
        }

        public static string ReturnStars(List<Review> reviewModels)
        {
            List<Star> stars = new(reviewModels.Count);
            foreach (Review model in reviewModels)
            {
                stars.Add(model.Star);
            }
            return Star.ReturnAverageStarRating(stars);
        }
    }
}
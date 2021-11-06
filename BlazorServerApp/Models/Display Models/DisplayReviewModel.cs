using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class DisplayReviewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Review title is too long.")]
        [MinLength(2, ErrorMessage = "Review title is too short")]
        public string ReviewTitle { get; set; }
        public string ReviewText { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(2, ErrorMessage = "Name is too short")]
        public string ReviewersName { get; set; }

        [Required]
        public int StarCount { get; set; }

        public static readonly Dictionary<int,string> Stars = new Dictionary<int, string>() { { 5, "⭐⭐⭐⭐⭐" } , { 4, "⭐⭐⭐⭐" } , { 3, "⭐⭐⭐" } , { 2, "⭐⭐" } , { 1, "⭐" }, { 0, "" } };

        public static int ReturnAverageRating(List<DisplayReviewModel> reviewModels)
        {
            if (reviewModels != null && reviewModels.Count > 0)
            {
                float sum = 0;
                foreach(DisplayReviewModel model in reviewModels)
                {
                    sum += model.StarCount;
                }
                return (int)(sum / reviewModels.Count);
            }
            return 0;
        }

        public static string ReturnStars(List<DisplayReviewModel> reviewModels)
        {
            return Stars[ReturnAverageRating(reviewModels)];
        }
    }

}

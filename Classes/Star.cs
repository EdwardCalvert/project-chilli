﻿using System;
using System.Collections.Generic;

namespace BlazorServerApp.Models
{
    public class Star
    {
        private int _starCount;
        private const int _maximumStars = 5;
        private const int _miniimumStars = 0;
        public static readonly Dictionary<int, string> Stars = new Dictionary<int, string>() { { 5, "⭐⭐⭐⭐⭐" }, { 4, "⭐⭐⭐⭐" }, { 3, "⭐⭐⭐" }, { 2, "⭐⭐" }, { 1, "⭐" }, { 0, "" } };

        private Star(int StarCount)
        {
            _starCount = StarCount;
        }

        public int GetNumberOfStars()
        {
            return _starCount;
        }

        public string GetStarsForUI()
        {
            return Stars[_starCount];
        }

        public void SetStarCount(int NewCount)
        {
            Validate(NewCount);
            _starCount = NewCount;
        }

        public static Star CreateStar(int starCount)
        {
            Validate(starCount);
            return new Star(starCount);
        }

        private static int ReturnAverageRating(List<Star> stars)
        {
            if (stars != null && stars.Count > 0)
            {
                double sum = 0;
                foreach (Star star in stars)
                {
                    sum += star.GetNumberOfStars();
                }
                return (int)(sum / stars.Count);
            }
            return 0;
        }

        public static string ReturnAverageStarRating(List<Star> reviewModels)
        {
            return Star.GetStarsForUI(ReturnAverageRating(reviewModels));
        }

        public static string GetStarsForUI(int starCount)
        {
            return Stars[starCount];
        }

        private static bool Validate(int starCount)
        {
            if (starCount < _miniimumStars || starCount > _maximumStars)
            {
                throw new ArgumentOutOfRangeException("You can't make that many stars");
            }
            else
            {
                return true;
            }
        }
    }
}
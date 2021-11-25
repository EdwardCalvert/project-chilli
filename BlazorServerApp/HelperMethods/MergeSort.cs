using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerApp.Models;

namespace BlazorServerApp.HelperMethods
{
    public class MergeSort
    {

        public static List<T> GenericMergeSort<T>(List<T> list) where T : IComparable
        {
            int length = list.Count;
            if(length <= 1) 
            {
                return list;
            }

            int median = list.Count / 2;

            List<T> left = new List<T>();
            left.AddRange(list.GetRange(0,median));

            List<T> right = new List<T>();
            right.AddRange(list.GetRange(median, length - median)); //problem

            GenericMergeSort<T>(left);
            GenericMergeSort<T>(right);

            return Merge<T>(list,left,right);
        }

        private static List<T> Merge<T>(List<T> sortedList, List<T> left, List<T> right) where T : IComparable
        {
            int leftIndex = 0;
            int rightIndex = 0;

            int leftLength = left.Count;
            int rightLength = right.Count;
            int totalItems = leftLength + rightLength;

            for(int index =0; index <totalItems; index++)
            {
                if(leftIndex >= leftLength)
                {
                    sortedList[index] = right[rightIndex];
                    rightIndex++;
                }
                else if(rightIndex >= right.Count)
                {
                    sortedList[index] = left[leftIndex];
                    leftIndex++;
                }
                else if (left[leftIndex].CompareTo(right[rightIndex]) < 0)
                {
                    sortedList[index] = left[leftIndex];
                    leftIndex++;
                }
                else // If the two values are the same, they will be added with the right first. 
                {
                    sortedList[index] = right[rightIndex];
                    rightIndex++;
                }
            }
            return sortedList;
        }


        public static List<Recipe> RecipeMergeSort(List<Recipe> recipes,SearchEnginge.SortBy sortBy, SearchEnginge.Order order)
        {
            if (sortBy != SearchEnginge.SortBy.Default) // Impossible
            {
                int length = recipes.Count;
                if (length <= 1) // What happens if you put 0 items in?
                {
                    return recipes;
                }

                int median = recipes.Count / 2;

                List<Recipe> left = new List<Recipe>();
                left.AddRange(recipes.GetRange(0, median));

                List<Recipe> right = new List<Recipe>();
                right.AddRange(recipes.GetRange(median, length - median)); //problem

                RecipeMergeSort(left, sortBy, order);
                RecipeMergeSort(right, sortBy, order);

                return Merge(recipes, left, right, sortBy, order);
            }
            return recipes;
        }


        private static List<Recipe> Merge(List<Recipe> sortedList, List<Recipe> left, List<Recipe> right,SearchEnginge.SortBy sortBy,SearchEnginge.Order order)
        {

                int leftIndex = 0;
                int rightIndex = 0;

                int leftLength = left.Count;
                int rightLength = right.Count;
                int totalItems = leftLength + rightLength;

                for (int index = 0; index < totalItems; index++)
                {
                    if (leftIndex >= leftLength)
                    {
                        sortedList[index] = right[rightIndex];
                        rightIndex++;
                    }
                    else if (rightIndex >= right.Count)
                    {
                        sortedList[index] = left[leftIndex];
                        leftIndex++;
                    }
                    else if (order == SearchEnginge.Order.Descending)
                    {

                        if (MergeSort.Compare(left[leftIndex], right[rightIndex], sortBy.ToString()) > 0)//Here we need to use the sort by value to get the field value, then compare. 
                        {
                            sortedList[index] = left[leftIndex];
                            leftIndex++;
                        }
                        else
                        {
                            sortedList[index] = right[rightIndex];
                            rightIndex++;
                        }
                    }
                    else
                    {
                        if (MergeSort.Compare(left[leftIndex], right[rightIndex], sortBy.ToString()) < 0)//Here we need to use the sort by value to get the field value, then compare. 
                        {
                            sortedList[index] = left[leftIndex];
                            leftIndex++;
                        }
                        else
                        {
                            sortedList[index] = right[rightIndex];
                            rightIndex++;
                        }
                    }
                }
                return sortedList;
            }


            //This method will only compare fields that are not lists
            public static int Compare(Recipe x, Recipe y,string field)
        {
            object value1;
            object value2;

            if (field == "Reviews")
            {
                value1 = Review.ReturnStars(x.Reviews).Length;
                value2 = Review.ReturnStars(y.Reviews).Length;
            }
            else if (field == "Difficulty")
            {
                if (x.Difficulty != null)
                {
                    value1 = Recipe.DifficultyEnum[x.Difficulty];
                }
                else
                {
                    value1 = 0;
                }
                if (y.Difficulty != null)
                {
                    value2 = Recipe.DifficultyEnum[y.Difficulty];
                }
                else
                {
                    value2 = 0;
                }
            }
            else
            {
                value1 = x.GetType().GetProperty(field).GetValue(x, null);
                value2 = y.GetType().GetProperty(field).GetValue(y, null);
            }
            if(value1 == null && value2 == null)
            {
                return 0;
            }

            else if(value1 == null)
            {
                return 1;
            }
            else if(value2 == null)
                {
                return -1;
                }
            return (int) value1.GetType().GetMethods().Where(m => m.Name == "CompareTo").FirstOrDefault().Invoke(value1,new object[1] { value2 }); 
        }
    }
}

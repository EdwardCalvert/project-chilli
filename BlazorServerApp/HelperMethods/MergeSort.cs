using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.HelperMethods
{
    public class MergeSort
    {

        public static List<T> GenericMergeSort<T>(List<T> list) where T : IComparable
        {
            int length = list.Count;
            if(length <= 1) // What happens if you put 0 items in?
            {
                return list;
            }

            int median = list.Count / 2;

            List<T> left = new List<T>();
            left.AddRange(list.GetRange(0,median));

            List<T> right = new List<T>();
            right.AddRange(list.GetRange(median, length - median)); //problem

            //Array.Copy(items, left, left.Length);
            //Array.Copy(new int [2], median, new int[2], 0, 11);

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.HelperMethods
{
    public class BinarySearch
    {
        public static int GenericBinarySearch<T>(List<T> array, T toFind, Comparer<T> comparer)
        {
            int leftPointer = 0;
            int rightPointer = array.Count - 1;
            int result = -1;
            while (leftPointer <= rightPointer && result == -1)
            {
                int midPoint = (leftPointer + rightPointer) / 2;
                if (comparer.Compare(array[midPoint], toFind) > 0)
                {
                    rightPointer = midPoint - 1;
                }
                else if (comparer.Compare(array[midPoint], toFind) < 0)
                {
                    leftPointer = midPoint + 1;
                }
                else if (comparer.Compare(array[midPoint], toFind) == 0)
                {
                    return midPoint;
                }
            }
            return default;
        }
    }
}

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
            if(length < 1) // What happens if you put 0 items in?
            {
                return list;
            }

            int median = list.Count / 2;

            List<T> left = new List<T>();
            left.AddRange(list.GetRange(0,median));

            return new List<T> ();
        }
    }
}

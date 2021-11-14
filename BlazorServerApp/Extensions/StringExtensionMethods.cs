using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Extensions
{
    public static class StringExtensionMethods
    {
        public static string SentenceCase(this string text)
        {
            return Split(RemoveSpace(text));
        }

        public static string RemoveSpace(string fullString)
        {
            return fullString.Trim();

        }

        public static string Split(string fullString)
        {
            var strArr = fullString.Split(new char[] { '.' });
            for (int iCount = 0; iCount < strArr.Count(); iCount++)
            {
                if (strArr[iCount].Length > 0)
                {
                    strArr[iCount] = strArr[iCount].Insert(0, strArr[iCount][0].ToString().ToUpper());
                    strArr[iCount] = strArr[iCount].Remove(1, 1);
                }
            }
            return string.Join(".", strArr);
        }

        public static string ReverseCSVValues(this string text)
        {
            string[] words = text.Split(",");
            Array.Reverse(words);
            return string.Join(" ", words).ToLower().Replace("  ", " ");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Extensions
{
    public static class @string
    {
        public static string MakeSQLSafe(this string @string)
        {
            return @string.Replace("'", "#39;");
        }

        public static string UnSQLize(this string @string)
        {
            return @string.Replace("#39;", "'");
        }
    }
}

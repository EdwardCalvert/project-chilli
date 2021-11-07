using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class SQLText
    {
        private string _text;

        private SQLText(string text)
        {
            _text = text;
        }

        public string GetPlainText()
        {
            return _text;
        }

        public string GetEscapedSQLText()
        {
            return MakeSQLSafe(_text);
        }

        public static SQLText CreateFromPlainText(string text)
        {
            return new SQLText(UnSQLize(text));
        }

        public static SQLText CreateFromSQLText(string text)
        {
            return new SQLText(UnSQLize(text));
        }


        private static string MakeSQLSafe( string @string)
        {
            return @string.Replace("'", "#39;");
        }

        private static string UnSQLize( string @string)
        {
            return @string.Replace("#39;", "'");
        }
    }
}

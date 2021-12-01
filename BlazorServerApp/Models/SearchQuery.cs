namespace BlazorServerApp.Models
{
    public class SearchQuery
    {
        public string SearchTerm { get; set; }
        public string ResultAsJSON { get; set; }

        public string SqlInsertStatement()
        {
            return "INSERT INTO SearchQuery VALUES(@SearchTerm, @ResultAsJSON);";
        }

        public dynamic SqlAnonymousType()
        {
            return new { SearchTerm = SearchTerm, ResultAsJSON = ResultAsJSON };
        }
    }
}
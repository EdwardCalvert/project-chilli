using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{

    public class DictionaryModel
    {
        public Word[] words { get; set; }
    }

    public class Word
    {
        public string word { get; set; }
        //public string origin { get; set; }
        public Meaning meanings { get; set; }
        public List<string> definitions { get
            {
                List<string> definitions = new();
                    foreach(var noun in meanings.noun)
                    {

                            definitions.Add(noun.definition);
                        
                    }
                return definitions;
            } }
    }

    public class Meaning
    {
        public Noun[] noun { get; set; }
    }

    public class Noun
    {
        public string definition { get; set; }
    }

    

}

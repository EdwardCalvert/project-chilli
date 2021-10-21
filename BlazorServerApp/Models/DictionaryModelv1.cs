using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models.v1
{

    public class APIResultV1
    {
        public string word { get; set; }
        public Phonetic[] phonetics { get; set; }
        public Meaning meaning { get; set; }
        public List<string> definitions
        {
            get
            {
                List<string> definitions = new();
                foreach (var noun in meaning.noun)
                {

                    definitions.Add(noun.definition);

                }
                return definitions;
            }
        }

    }

    public class Meaning
    {
        public Exclamation[] exclamation { get; set; }
        public Noun[] noun { get; set; }
        public IntransitiveVerb[] intransitiveverb { get; set; }
    }

    public class Exclamation
    {
        public string definition { get; set; }
        public string example { get; set; }
    }

    public class Noun
    {
        public string definition { get; set; }
        public string example { get; set; }
        public string[] synonyms { get; set; }
    }

    public class IntransitiveVerb
    {
        public string definition { get; set; }
        public string example { get; set; }
    }

    public class Phonetic
    {
        public string text { get; set; }
        public string audio { get; set; }
    }


}

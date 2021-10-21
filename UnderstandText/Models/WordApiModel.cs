using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderstandText.Models
{
    class WordApiModel
    {
    }
    public class Rootobject
    {
        public Result[] results { get; set; }
        public Syllables syllables { get; set; }
        public Pronunciation pronunciation { get; set; }
    }

    public class Syllables
    {
        public int count { get; set; }
        public string[] list { get; set; }
    }

    public class Pronunciation
    {
        public string all { get; set; }
    }

    public class Result
    {
        public string definition { get; set; }
        public string partOfSpeech { get; set; }
        public string[] synonyms { get; set; }
        public string[] typeOf { get; set; }
        public string[] derivation { get; set; }
    }

}

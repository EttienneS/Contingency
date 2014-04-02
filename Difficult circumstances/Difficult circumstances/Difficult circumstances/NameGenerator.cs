using System;
using System.Text.RegularExpressions;

namespace Difficult_circumstances
{
    internal class NameGenerator
    {
        private static readonly Regex InvalidNameRegex = new Regex(@"(?:([aiuy])\1)|(?:(\w\w)\2)|([^aeiouy]{3,})|([aeiouy]{3,})|(\wy\w)|(^nd)|(^nt)|(^rt)|(^rs)|(^ht)|([aiou]$)|(tw$)");

        private static readonly string[] Digraphs =
        {
            "en", "re", "er", "nt", "th", "on", "in", "te", "an", "or", "st",
            "ed", "ne", "ve", "es", "nd", "to", "se", "at", "ti", "ar", "ee",
            "rt", "as", "co", "io", "ty", "fo", "fi", "ra", "et", "le", "ou",
            "ma", "tw", "ea", "is", "si", "de", "hi", "al", "ce", "da", "ec",
            "rs", "ur", "ni", "ri", "el", "la", "ro", "ta"
        };

        private static Random random = new Random();

        public static string GenerateName()
        {
            string potentialName;

            do
            {
                potentialName = "";

                for (int digraphCount = 0; digraphCount < random.Next(2, 5); digraphCount++)
                {
                    potentialName += Digraphs[random.Next(0, Digraphs.GetUpperBound(0))];
                }
            } while (InvalidNameRegex.IsMatch(potentialName));

            return potentialName.Substring(0, 1).ToUpper() + potentialName.Substring(1);
        }
    }
}
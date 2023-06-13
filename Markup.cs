using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace party_crab {
    public class Markup
    {
        static Dictionary<string, Tuple<string, string>> defaultMarkups = new Dictionary<string, Tuple<string, string>>
        {
            { "**", new Tuple<string, string>("<b>", "</b>") },
            { "_", new Tuple<string, string>("<i>", "</i>") },
            { "*", new Tuple<string, string>("<i>", "</i>") },
            { "||", new Tuple<string, string>("<mark>", "</mark>") }
        };

        public static string MarkupReplace(string text, string value, string start, string end)
        {
            string returnText = text;
            int valueCount = returnText.Split(new string[] { value }, StringSplitOptions.None).Length - 1;
            int escapeCount = returnText.Split(new string[] { "\\" + value }, StringSplitOptions.None).Length - 1;
            valueCount -= escapeCount;

            if (valueCount % 2 != 0)
            {
                valueCount -= 1;
            }

            int valuePrevious = 0;
            for (int i = 0; i < valueCount / 2; i++)
            {
                int valueStart = returnText.IndexOf(value, valuePrevious);
                returnText = returnText.Substring(0, valueStart) + start + returnText.Substring(valueStart + value.Length);

                int valueEnd = returnText.IndexOf(value, valueStart + value.Length);
                returnText = returnText.Substring(0, valueEnd) + end + returnText.Substring(valueEnd + value.Length);

                valuePrevious = valueEnd + value.Length;
            }

            return returnText;
        }

        public static string MarkupColor(string text)
        {
            int valueCount = Math.Min(text.Split('[').Length - 1, text.Split(']').Length - 1);

            if (valueCount % 2 != 0)
            {
                valueCount -= 1;
            }

            string pattern = @"\[(.*?)\]";
            int valueAmount = Regex.Matches(text, pattern).Count;
            string returnText = Regex.Replace(text, pattern, "<color=$1>");
            returnText += string.Concat(Enumerable.Repeat("</color>", valueAmount));

            return returnText;
        }

        public static string MarkupChange(string text, Dictionary<string, Tuple<string, string>> markups = null)
        {
            string returnText = text;

            if (markups == null)
            {
                markups = defaultMarkups;
            }

            foreach (var markup in markups)
            {
                returnText = MarkupReplace(returnText, markup.Key, markup.Value.Item1, markup.Value.Item2);
            }

            returnText = MarkupColor(returnText);

            return returnText;
        }
    }
}
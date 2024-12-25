#nullable disable

using EZcore.Models;

namespace EZcore.Extensions
{
    public static class StringExtensions
    {
        public static string FirstLetterToUpperOthersToLower(this string value)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                result = value.Substring(0, 1).ToUpper();
                if (value.Length > 1)
                    result += value.Substring(1).ToLower();
            }
            return result;
        }

        public static string RemoveHtmlTags(this string value, string brTagSeperator = ", ")
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Replace("&nbsp;", " ").Replace("<br>", brTagSeperator).Replace("<br />", brTagSeperator).Replace("<br/>", brTagSeperator)
                    .Replace("&amp;", "&").Trim();
                char[] array = new char[value.Length];
                int arrayIndex = 0;
                bool inside = false;
                for (int i = 0; i < value.Length; i++)
                {
                    char let = value[i];
                    if (let == '<')
                    {
                        inside = true;
                        continue;
                    }
                    if (let == '>')
                    {
                        inside = false;
                        continue;
                    }
                    if (!inside)
                    {
                        array[arrayIndex] = let;
                        arrayIndex++;
                    }
                }
                result = new string(array, 0, arrayIndex);
            }
            return result;
        }

        public static string ChangeTurkishCharactersToEnglish(this string valueTR)
        {
            string valueEN = string.Empty;
            if (!string.IsNullOrWhiteSpace(valueTR))
            {
                Dictionary<string, string> characterDictionary = new Dictionary<string, string>()
                {
                    { "Ö", "O" },
                    { "Ç", "C" },
                    { "Ş", "S" },
                    { "Ğ", "G" },
                    { "Ü", "U" },
                    { "ö", "o" },
                    { "ç", "c" },
                    { "ş", "s" },
                    { "ğ", "g" },
                    { "ü", "u" },
                    { "İ", "I" },
                    { "ı", "i" }
                };
                foreach (var character in valueTR)
                {
                    if (characterDictionary.ContainsKey(character.ToString()))
                        valueEN += characterDictionary[character.ToString()];
                    else
                        valueEN += character.ToString();
                }
            }
            return valueEN;
        }

        public static int GetCount(this string value, char character)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Count(v => v == character);
            return 0;
        }

        public static string GetDisplayName(this string value, Lang lang = Lang.TR)
        {
            string result = string.Empty;
            string[] valueParts;
            if (!string.IsNullOrWhiteSpace(value))
            {
                result = value;
                if (value.GetCount('{') == 1 && value.GetCount('}') == 1 && value.GetCount(';') == 1)
                {
                    value = value.Substring(1, value.Length - 2);
                    valueParts = value.Split(';');
                    if (lang == Lang.TR)
                        result = valueParts.First();
                    else
                        result = valueParts.Last();
                }
            }
            return result;
        }

        public static string GetErrorMessage(this string value, Lang lang = Lang.TR)
        {
            string result = string.Empty;
            string displayName;
            string[] valueParts;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Contains("not valid", StringComparison.OrdinalIgnoreCase) || value.Contains("invalid", StringComparison.OrdinalIgnoreCase))
                {
                    result = lang == Lang.TR ? "Geçersiz değer!" : "Invalid value!";
                }
                else if ((value.GetCount('{') == 0 && value.GetCount('}') == 0) || (value.GetCount('{') == 2 && value.GetCount('}') == 2))
                {
                    if (value.GetCount('{') == 2 && value.GetCount('}') == 2)
                    {
                        displayName = value.Substring(value.IndexOf('{'), value.IndexOf('}') + 1);
                        value = value.Replace(displayName, GetDisplayName(displayName, lang));
                    }
                    if (value.GetCount(';') == 1)
                    {
                        valueParts = value.Split(';');
                        if (lang == Lang.TR)
                            result = valueParts.First();
                        else
                            result = valueParts.Last();
                    }
                    else
                    {
                        result = value;
                    }
                }
            }
            return result;
        }

        public static string ReplaceNewLineWithLineBreak(this string value, string newLine = "\n")
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
                return result;
            result = value.Replace(newLine, "<br>");
            return result;
        }

        public static string SeperateUpperCaseCharacters(this string value, char seperator = ' ')
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
                return result;
            result += value[0];
            for (int i = 1; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]) && !char.IsUpper(value[i - 1]) && char.IsLetter(value[i - 1]))
                    result += seperator;
                result += value[i];
            }
            return result;
        }

        public static string Find(this string value, string expression, bool matchCase, string foundPrefix = "~", string foundSuffix = "~")
        {
            string result = null;
            if (string.IsNullOrWhiteSpace(value))
                return result;
            if (string.IsNullOrWhiteSpace(expression))
                return value;
            StringComparison comparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            if (!value.Contains(expression, comparison))
                return result;
            result = value;
            bool found = false;
            int i = 0;
            while (i <= result.Length - expression.Length)
            {
                if (result.Substring(i, expression.Length).Equals(expression, comparison))
                {
                    result = result.Insert(i, foundPrefix).Insert(i + expression.Length + foundSuffix.Length, foundSuffix);
                    i += foundPrefix.Length + expression.Length + foundSuffix.Length;
                    found = true;
                }
                else
                {
                    i++;
                }
            }
            result = found ? result : null;
            return result;
        }

        public static string Find(this string value, string expression, bool matchCase, bool matchWord, string foundPrefix = "~", string foundSuffix = "~", string lineSeperator = "\n")
        {
            string result = null;
            if (string.IsNullOrWhiteSpace(value))
                return result;
            if (string.IsNullOrWhiteSpace(expression))
                return value;
            StringComparison comparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            if (!value.Contains(expression, comparison))
                return result;
            bool found = false;
            int p;
            char[] punctuations;
            string[] lines, words, subWords;
            string line, word;
            if (matchWord)
            {
                lines = value.Split(lineSeperator);
                for (int l = 0; l < lines.Length; l++)
                {
                    line = lines[l].Trim('\r');
                    if (line.Contains(expression, comparison))
                    {
                        words = line.Split(' ');
                        for (int w = 0; w < words.Length; w++)
                        {
                            word = words[w];
                            if (word.Equals(expression, comparison))
                            {
                                words[w] = foundPrefix + word + foundSuffix;
                                found = true;
                            }
                            else
                            {
                                punctuations = word.Where(char.IsPunctuation).ToArray();
                                if (punctuations.Any() && word.Contains(expression, comparison))
                                {
                                    if (word.Trim(punctuations).Equals(expression.Trim(punctuations), comparison))
                                    {
                                        words[w] = Find(word, expression, matchCase, foundPrefix, foundSuffix) ?? word;
                                        found = true;
                                    }
                                    else
                                    {
                                        subWords = word.Split(punctuations);
                                        word = string.Empty;
                                        p = 0;
                                        foreach (string subWord in subWords)
                                        {
                                            if (subWord.Equals(expression, comparison))
                                            {
                                                word += Find(subWord, expression, matchCase, foundPrefix, foundSuffix) ?? subWord;
                                                found = true;
                                            }
                                            else
                                            {
                                                word += subWord;
                                            }
                                            if (p < punctuations.Length)
                                                word += punctuations[p++];
                                        }
                                        words[w] = word;
                                    }
                                }
                            }
                        }
                        lines[l] = string.Join(' ', words);
                    }
                }
                result = found ? string.Join(lineSeperator, lines) : null;
            }
            else
            {
                result = Find(value, expression, matchCase, foundPrefix, foundSuffix);
            }
            return result;
        }
    }
}

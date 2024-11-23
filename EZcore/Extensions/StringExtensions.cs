#nullable disable

using EZcore.Models;

namespace EZcore.Extensions
{
    public static class StringExtensions
    {
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

        public static T Trim<T>(this T instance) where T : class, new()
        {
            var properties = instance.GetType().GetProperties().Where(property => property.PropertyType == typeof(string)).ToList();
            object value;
            if (properties is not null)
            {
                foreach (var property in properties)
                {
                    value = property.GetValue(instance);
                    if (value is not null)
                        property.SetValue(instance, ((string)value).Trim());
                }
            }
            return instance;
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
    }
}

#nullable disable

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
    }
}

#nullable disable

using EZcore.Models;
using System.Reflection;

namespace EZcore.Services
{
    public class ClassService
    {
        public List<PropertyModel> GetProperties<T>() where T : class, new()
        {
            List<PropertyModel> properties = null;
            List<Attribute> customAttributes;
            string displayName;
            bool ignore;
            var propertyInfoArray = typeof(T).GetProperties();
            if (propertyInfoArray is not null && propertyInfoArray.Any())
            {
                properties = new List<PropertyModel>();
                foreach (var propertyInfo in propertyInfoArray)
                {
                    ignore = false;
                    displayName = string.Empty;
                    customAttributes = propertyInfo.GetCustomAttributes().ToList();
                    if (customAttributes is not null && customAttributes.Any())
                    {
                        ignore = customAttributes.Any(customAttribute => customAttribute.GetType() == typeof(Attributes.IgnoreAttribute));
                        foreach (var customAttribute in customAttributes)
                        {
                            if (!ignore && customAttribute.GetType() == typeof(Attributes.DisplayNameAttribute))
                            {
                                displayName = ((Attributes.DisplayNameAttribute)customAttribute).DisplayName;
                                break;
                            }
                            if (!ignore && customAttribute.GetType() == typeof(System.ComponentModel.DisplayNameAttribute))
                            {
                                displayName = ((System.ComponentModel.DisplayNameAttribute)customAttribute).DisplayName;
                                break;
                            }
                        }
                    }
                    if (!ignore)
                        properties.Add(new PropertyModel(propertyInfo.Name, displayName));
                }
            }
            return properties;
        }

        public PropertyInfo GetPropertyInfo<T>(string propertyName, T instance = null) where T : class, new()
        {
            return instance is null ? typeof(T).GetProperty(propertyName) : instance.GetType().GetProperty(propertyName);
        }
    }
}

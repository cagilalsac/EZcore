#nullable disable

using EZcore.Models;
using System.Reflection;

namespace EZcore.Extensions
{
    public static class ObjectExtensions
    {
        public static PropertyInfo GetPropertyInfo<T>(string propertyName, T instance = null) where T : class, new()
        {
            return instance is null ? typeof(T).GetProperty(propertyName) : instance.GetType().GetProperty(propertyName);
        }

        public static List<PropertyInfo> GetPropertyInfo<T>(T instance = null) where T : class, new()
        {
            return instance is null ? typeof(T).GetProperties().ToList() : instance.GetType().GetProperties().ToList();
        }

        public static List<PropertyModel> GetProperties<T>() where T : class, new()
        {
            List<PropertyModel> list = null;
            List<Attribute> customAttributes;
            string displayName;
            bool ignore;
            var properties = GetPropertyInfo<T>();
            if (properties is not null && properties.Any())
            {
                list = new List<PropertyModel>();
                foreach (var property in properties)
                {
                    ignore = false;
                    displayName = string.Empty;
                    customAttributes = property.GetCustomAttributes().ToList();
                    if (customAttributes is not null)
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
                        list.Add(new PropertyModel(property.Name, displayName));
                }
            }
            return list;
        }

        public static TTarget Map<TTarget>(this object source, TTarget target = null) where TTarget : class, new()
        {
            if (source is null)
                return null;
            if (target is null)
                target = new TTarget();
            var targetProperties = GetPropertyInfo<TTarget>();
            if (targetProperties is not null)
            {
                foreach (var targetProperty in targetProperties)
                {
                    var sourceProperty = GetPropertyInfo(targetProperty.Name, source);
                    if (sourceProperty is not null && sourceProperty.CanRead && targetProperty.CanWrite)
                    {
                        var sourcePropertyType = sourceProperty.PropertyType;
                        var targetPropertyType = targetProperty.PropertyType;
                        if (sourcePropertyType.IsGenericType && sourcePropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            sourcePropertyType = sourcePropertyType.GetGenericArguments()[0];
                        if (targetPropertyType.IsGenericType && targetPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            targetPropertyType = targetPropertyType.GetGenericArguments()[0];
                        if (sourcePropertyType == targetPropertyType)
                        {
                            var targetPropertyValue = sourceProperty.GetValue(source);
                            targetProperty.SetValue(target, targetPropertyValue);
                        }
                    }
                }
            }
            return target;
        }

        public static T Trim<T>(this T instance) where T : class, new()
        {
            if (instance is null)
                return null;
            var properties = GetPropertyInfo(instance).Where(property => property.PropertyType == typeof(string)).ToList();
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
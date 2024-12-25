#nullable disable

using EZcore.DAL;
using EZcore.Models;
using System.Data;
using System.Reflection;

namespace EZcore.Extensions
{
    public static class ListExtensions
    {
        public static DataTable ConvertToDataTable<T>(this List<T> list, Lang lang = Lang.TR) where T : class, new()
        {
            DataTable dataTable = null;
            DataRow row;
            PropertyInfo propertyInfo;
            object propertyValue;
            List<string> propertyNames;
            List<string> displayNames;
            var properties = ObjectExtensions.GetProperties<T>().Where(property => property.Name != nameof(Record)).ToList();
            if (properties is not null && properties.Any())
            {
                propertyNames = properties.Select(p => p.Name).ToList();
                displayNames = properties.Select(p => p.DisplayName).ToList();
                dataTable = new DataTable();
                for (int i = 0; i < properties.Count; i++)
                {
                    propertyInfo = ObjectExtensions.GetPropertyInfo<T>(properties[i].Name);
                    displayNames[i] = displayNames[i].GetDisplayName(lang);
                    dataTable.Columns.Add(displayNames[i], Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }
                if (list is not null && list.Any())
                {
                    foreach (var item in list)
                    { 
                        row = dataTable.NewRow();
                        for (int i = 0; i < properties.Count; i++)
                        {
                            propertyValue = ObjectExtensions.GetPropertyInfo(propertyNames[i], item).GetValue(item);
                            row[displayNames[i]] = propertyValue ?? DBNull.Value;
                        }
                        dataTable.Rows.Add(row);
                    }
                }
            }
            return dataTable;
        }
    }
}

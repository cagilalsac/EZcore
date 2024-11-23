#nullable disable

namespace EZcore.Models
{
    public class PropertyModel
    {
        public string Name { get; }
        public string DisplayName { get; }

        public PropertyModel(string name, string displayName = "")
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}

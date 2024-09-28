namespace EZcore.Extensions
{
    public static class EnumExtensions
    {
        public static Dictionary<int, string> ToDictionary(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(v => (int)(object)v, v => v.ToString());
        }
    }
}

namespace EZcore.Extensions
{
    public static class BoolExtensions
    {
        public static string ToHtml(this bool value, string trueHtml = "<i class='bx bx-check fs-3'></i>", string falseHtml = "<i class='bx bx-x fs-3'></i>")
        {
            return value ? trueHtml : falseHtml; 
        }
    }
}

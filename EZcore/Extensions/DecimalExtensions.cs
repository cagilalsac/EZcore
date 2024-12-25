using EZcore.Models;

namespace EZcore.Extensions
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// Supports to a maximum value of 10 trillion. Automatically rounds decimals to 2 digits. Only supports Turkish (Türkçe) for now.
        /// </summary>
        public static string ConvertMoneyToString(this decimal value, Lang lang = Lang.TR, bool adjoint = false)
        {
            if (lang != Lang.TR)
                return "";
            if (value > 10000000000000.0m)
                return "";
            string currency = "TL";
            if (value == 0)
                return "SIFIR " + currency;
            string decimalPoint = ",";
            string result = "";
            string sAmount = value.ToString("F2").Replace('.', ',');
            string wholePart = sAmount.Substring(0, sAmount.IndexOf(','));
            string decimalPart = sAmount.Substring(sAmount.IndexOf(',') + 1, 2);
            string[]? ones;
            string[]? tens;
            string[]? thousands;
            if (adjoint)
            {
                ones = ["", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ"];
                tens = ["", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN"];
                thousands = ["KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", ""];
            }
            else
            {
                ones = ["", "BİR ", "İKİ ", "ÜÇ ", "DÖRT ", "BEŞ ", "ALTI ", "YEDİ ", "SEKİZ ", "DOKUZ "];
                tens = ["", "ON ", "YİRMİ ", "OTUZ ", "KIRK ", "ELLİ ", "ALTMIŞ ", "YETMİŞ ", "SEKSEN ", "DOKSAN "];
                thousands = ["KATRİLYON ", "TRİLYON ", "MİLYAR ", "MİLYON ", "BİN ", ""];
            }
            int groupCount = 6;
            wholePart = wholePart.PadLeft(groupCount * 3, '0');
            string groupValue;
            for (int i = 0; i < groupCount * 3; i += 3)
            {
                groupValue = "";
                if (wholePart.Substring(i, 1) != "0")
                {
                    if (adjoint)
                        groupValue += ones[Convert.ToInt32(wholePart.Substring(i, 1))] + "YÜZ";
                    else
                        groupValue += ones[Convert.ToInt32(wholePart.Substring(i, 1))] + "YÜZ ";
                }
                if (groupValue.Trim() == "BİRYÜZ" || groupValue.Trim() == "BİR YÜZ")
                    groupValue = adjoint == true ? "YÜZ" : "YÜZ ";
                groupValue += tens[Convert.ToInt32(wholePart.Substring(i + 1, 1))];
                groupValue += ones[Convert.ToInt32(wholePart.Substring(i + 2, 1))];
                if (groupValue != "")
                    groupValue += thousands[i / 3];
                if (groupValue.Trim() == "BİRBİN" || groupValue.Trim() == "BİR BİN")
                    groupValue = adjoint == true ? "BİN" : "BİN ";
                result += groupValue;
            }
            if (Convert.ToInt64(wholePart) != 0)
            {
                if (currency.Trim().ToUpper().Equals("TL"))
                {
                    if (adjoint)
                        result += " " + currency;
                    else
                        result += currency;
                }
                result = result.Trim();
            }
            else
            {
                result = "";
            }
            if (Convert.ToInt64(decimalPart) != 0)
            {
                if (currency.Trim().ToUpper().Equals("TL"))
                {
                    result += " ";
                }
                else
                {
                    if (decimalPoint.Trim().Equals(","))
                    {
                        if (adjoint)
                            result += "VİRGÜL";
                        else
                            result += " VİRGÜL ";
                    }
                    else
                    {
                        if (adjoint)
                            result += "NOKTA";
                        else
                            result += " NOKTA ";
                    }
                }
                if (decimalPart.Substring(0, 1) != "0")
                    result += tens[Convert.ToInt32(decimalPart.Substring(0, 1))];
                if (decimalPart.Substring(1, 1) != "0")
                    result += ones[Convert.ToInt32(decimalPart.Substring(1, 1))];
                result = result.Trim();
                if (currency.Trim().ToUpper().Equals("TL"))
                    result += " kr.";
                else
                    result += " " + currency;
            }
            else
            {
                if (!currency.Trim().ToUpper().Equals("TL"))
                    result += " " + currency;
            }
            return result;
        }
    }
}

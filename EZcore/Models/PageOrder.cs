#nullable disable

using EZcore.Extensions;

namespace EZcore.Models
{
    public class PageOrder
    {
        public int PageNumber { get; set; }
        public string RecordsPerPageCount { get; set; }
        public int TotalRecordsCount { get; set; }
        public List<string> RecordsPerPageCounts { get; private set; }
        
        public List<int> PageNumbers
        {
            get
            {
                var pageNumbers = new List<int>();
                int recordsPerPageCount;
                if (TotalRecordsCount > 0 && int.TryParse(RecordsPerPageCount, out recordsPerPageCount))
                {
                    var numberOfPages = Convert.ToInt32(Math.Ceiling(TotalRecordsCount / Convert.ToDecimal(recordsPerPageCount)));
                    for (int page = 1; page <= numberOfPages; page++)
                    {
                        pageNumbers.Add(page);
                    }
                }
                else
                {
                    pageNumbers.Add(1);
                }
                return pageNumbers;
            }
        }

        public string OrderExpression { get; set; }
        public Dictionary<string, string> OrderExpressions { get; private set; }

        public bool PageOrderSession { get; set; }

        public Lang Lang { get; set; }

        public PageOrder()
        {
            PageNumber = 1;
            RecordsPerPageCount = "10";
            RecordsPerPageCounts = ["5", "10", "25", "50", "100", Thread.CurrentThread.CurrentCulture.Name == "tr-TR" ? "Tümü" : "All"];
            OrderExpression = string.Empty;
            OrderExpressions = new Dictionary<string, string>();
        }

        /// <summary>
        /// Must be added by related entity property names, seperated by a space character for multiple words if any, and the relevant Turkish expressions. 
        /// Turkish characters will be replaced with corresponding English characters. 
        /// </summary>
        /// <param name="entityPropertyName"></param>
        /// <param name="expressionTR"></param>
        public void AddOrderExpression(string entityPropertyName, string expressionTR = "")
        {
            var descKey = "DESC";
            var descValue = Thread.CurrentThread.CurrentCulture.Name == "tr-TR" ? "Azalan" : "Descending";
            var key = entityPropertyName.ChangeTurkishCharactersToEnglish().Replace(" ", "");
            var value = Thread.CurrentThread.CurrentCulture.Name == "tr-TR" && !string.IsNullOrWhiteSpace(expressionTR) ? expressionTR : entityPropertyName;
            if (!OrderExpressions.ContainsKey(key))
            {
                OrderExpressions.Add(key, value);
                OrderExpressions.Add($"{key}{descKey}", $"{value} {descValue}");
            }
        }
    }
}

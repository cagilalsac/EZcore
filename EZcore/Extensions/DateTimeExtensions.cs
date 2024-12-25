namespace EZcore.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AddTime(this DateTime dateTime, int hours = 0, int minutes = 0, int seconds = 0)
        {
            dateTime = dateTime.AddHours(hours);
            dateTime = dateTime.AddMinutes(minutes);
            dateTime = dateTime.AddSeconds(seconds);
            return dateTime;
        }

        public static Dictionary<DateTime, DateTime?> Intersect(this DateTime startDateTime, Dictionary<DateTime, DateTime?> dateTimes, DateTime? endDateTime = null)
        {
            var dateTimesResult = new Dictionary<DateTime, DateTime?>();
            if (endDateTime is not null)
            {
                if (dateTimes.Any(dt => (startDateTime < dt.Key && endDateTime > (dt.Value ?? DateTime.MaxValue))
                    || (startDateTime >= dt.Key && startDateTime <= (dt.Value ?? DateTime.MaxValue))
                    || (endDateTime >= dt.Key && endDateTime <= (dt.Value ?? DateTime.MaxValue))))
                {
                    dateTimesResult.Add(startDateTime, endDateTime);
                }
            }
            else
            {
                if (dateTimes.Any(dt => startDateTime <= (dt.Value ?? DateTime.MaxValue)))
                {
                    dateTimesResult.Add(startDateTime, endDateTime);
                }
            }
            return dateTimesResult;
        }
    }
}

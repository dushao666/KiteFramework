namespace Infrastructure.Extension;
/// <summary>
/// 时间扩展
/// </summary>
public static class TimeExtension
{
    public static DateTime NewUTCTimeZone()
    {
        return DateTime.UtcNow;
    }

    public static DateTime ConvertToUTCTime(long ticks)
    {
        return new DateTime(new DateTime(1970, 1, 1, 0, 0, 0).Ticks + ticks * 10000);
    }

    public static DateTime ConvertToLocalTime(long ticks)
    {
        return new DateTime(new DateTime(1970, 1, 1, 0, 0, 0).Ticks + ticks * 10000).AddHours(8.0);
    }

    public static DateTime NewTimeZone()
    {
        return DateTime.UtcNow.AddHours(8.0);
    }

    public static DateTime ToUtcFormat(this DateTime dateTiem)
    {
        return dateTiem.ToUniversalTime();
    }

    public static DateOnly? DateOnlyFormat(DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    public static TimeOnly? TimeOnlyFormat(DateTime dateTime)
    {
        return TimeOnly.FromDateTime(dateTime);
    }

    public static DateOnly? DateOnlyFormat(DateTime? dateTime)
    {
        return DateOnly.FromDateTime(dateTime.Value);
    }

    public static TimeOnly? TimeOnlyFormat(DateTime? dateTime)
    {
        return TimeOnly.FromDateTime(dateTime.Value);
    }

    public static DateTime? DateTimeFormat(DateOnly? date, TimeOnly? time)
    {
        return date?.ToDateTime(time.GetValueOrDefault());
    }

    public static DateOnly? CurrentDate()
    {
        return DateOnlyFormat(NewTimeZone());
    }

    public static TimeOnly? CurrentTime()
    {
        return TimeOnlyFormat(NewTimeZone());
    }

    public static DateTime ConvertTimestamp(double timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        if (timestamp > 9999999999.0)
        {
            throw new ArgumentException("Error timestamp,too large");
        }

        return dateTime.AddSeconds(timestamp).ToLocalTime();
    }

    public static long GetCurrentTimestamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000000;
    }

    public static long GetCurrentMilliTimestamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000;
    }
}
namespace ONEPASS_FITNESS.Services
{
    public class GymTimeZoneProvider
    {
        private const string DefaultTimeZoneId = "Pacific/Auckland";

        public GymTimeZoneProvider(IConfiguration configuration)
        {
            var id = configuration["GymTimeZone"] ?? DefaultTimeZoneId;

            try
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
            {
                TimeZone = TimeZoneInfo.Utc;
            }
        }

        public TimeZoneInfo TimeZone { get; }

        public DateTime ToUtc(DateTime localTime)
        {
            var unspecified = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);

            if (TimeZone.IsInvalidTime(unspecified))
            {
                // Clocks jumped forward over this local time, shift past the gap.
                unspecified = unspecified.AddHours(1);
            }

            return TimeZoneInfo.ConvertTimeToUtc(unspecified, TimeZone);
        }

        public DateTime ToLocal(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcTime, DateTimeKind.Utc), TimeZone);
        }
    }
}

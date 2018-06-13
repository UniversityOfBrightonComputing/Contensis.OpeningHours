using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityOfBrighton.Contensis.OpeningHours
{
    /// <summary>
    /// Defines a period during which something is open on given days for possibly different times
    /// </summary>
    public class OpenTimePeriod
    {
        public string Name;
        // Taxonomy keys for the different types this period applies to
        public string[] PeriodFor;
        // 1 is lowest
        public int Priority;
        public DateRange When;
        public List<DayOpenTime> DayOpenTimes;
        public List<DateTime?> Exceptions; // nullable because CMS sets initial first value to null

        /// <summary>
        /// Range of dates applicable for
        /// </summary>
        public struct DateRange
        {
            public DateTime? From;
            public DateTime? To;
        }

        /// <summary>
        /// One time for which is open
        /// </summary>
        public struct OpenTime
        {
            public string Start;
            public string End;
        }

        /// <summary>
        /// Checks if the specified date is within this OpenTimePeriod
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool WithinPeriod(DateTime date)
        {
            return (When.From <= date.Date && When.To >= date.Date && !MatchesException(date));
        }

        /// <summary>
        /// If there is at least one non-null member of Exceptions which matches the date then should exclude
        /// </summary>
        /// <param name="date">The DateTime to check</param>
        /// <returns>True if the date should be excluded, False if not</returns>
        public bool MatchesException(DateTime date)
        {
            return (Exceptions == null) ? false : Exceptions.Any(d => d != null && d.Value.Date == date.Date);
        }

        /// <summary>
        /// For a given DateTime works out if the time is Open or Closed throws ArgumentOutOfRangeException if
        /// the date is not within this range because the result would be wrong.
        /// </summary>
        /// <param name="date">DateTime must be within the period or throws ArgumentOutOfRangeException</param>
        /// <returns>True if open, False if not</returns>
        public bool IsOpen(DateTime date)
        {
            if (!WithinPeriod(date))
            {
                throw new ArgumentOutOfRangeException("date",
                    "Date is not within period, make sure to check with WithinPeriod() before calling");
            }

            // Any DayOpenTimes which match on day of the week
            var day = date.DayOfWeek;
            var matchingDayOpenTimes = DayOpenTimes.Where(d => d.Days.Contains(day));

            // Check matchingDayOpenTimes to see if is open for the time of day
            var time = date.TimeOfDay;
            foreach (var dayOpenTime in matchingDayOpenTimes)
            {
                if (dayOpenTime.IsOpenAt(time))
                {
                    return true;
                }
            }

            // If not found then now is not open
            return false;
        }

        /// <summary>
        /// For a given DayOfWeek return a list of OpenTimes
        /// </summary>
        /// <param name="day">DayOfWeek to check</param>
        /// <returns>List of OpenTimes times for day</returns>
        public List<OpenTime> GetOpenTimesForDayOfWeek(DayOfWeek day)
        {
            return DayOpenTimes
                .Where(d => d.Days.Contains(day))
                .Select(d => new OpenTime { Start = d.Start, End = d.End })
                .ToList();
        }
    }
}
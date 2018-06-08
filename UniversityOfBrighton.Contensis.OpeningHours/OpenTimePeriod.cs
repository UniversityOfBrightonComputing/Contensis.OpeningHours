using System;
using System.Collections.Generic;
using System.Linq;
using Zengenti.Contensis.Delivery;

namespace UniversityOfBrighton.Contensis.OpeningHours
{
    /// <summary>
    /// For exporting openTimePeriod Items from the CMS
    /// </summary>
    public class OpenTimePeriod
    {
        public string Name;
        public string[] PeriodFor;
        public int Priority;
        public DateRange When;
        public List<DayOpenTime> DayOpenTimes;
        public List<DateTime?> Exceptions; // nullable because CMS sets initial first value to null

        public class DayOpenTime
        {
            public List<DayOfWeek> Days;
            public string Start;
            public string End;
        }

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

        // If there is at least one non-null member of Exceptions which matches the date then exclude
        public bool MatchesException(DateTime date)
        {
            return (Exceptions == null) ? false : Exceptions.Any(d => d != null && d.Value.Date == date.Date);
        }


        /// <summary>
        /// For a given DateTime works out if the time is Open or Closed
        /// </summary>
        /// <param name="date">DateTime must be within the period or throws ArgumentOutOfRangeException</param>
        /// <returns>True if open</returns>
        public bool IsOpen(DateTime date)
        {
            if (!WithinPeriod(date))
            {
                throw new ArgumentOutOfRangeException("Date is not within period");
            }

            // Any DayOpenTimes which match on day of the week
            var day = date.DayOfWeek;
            var matchingDayOpenTimes = DayOpenTimes.Where(d => d.Days.Contains(day));

            // Search matching times to see if time is within start and end and 
            var time = date.TimeOfDay;
            foreach (var dayOpenTime in matchingDayOpenTimes)
            {
                if (IsOpen24Hours(dayOpenTime))
                {
                    return true;
                }
                else if(HasOpenedBy(dayOpenTime, time) && IsStillOpenAt(dayOpenTime, time))
                {
                    return true;
                }
            }

            // If not found then now is not open
            return false;

        }

        public bool IsOpen24Hours(DayOpenTime dayOpenTime)
        {
            return (dayOpenTime.Start == "0:00" && dayOpenTime.End == "0:00");
        }

        public bool HasOpenedBy(DayOpenTime dayOpenTime, TimeSpan time)
        {
            var start = TimeSpan.Parse(dayOpenTime.Start);
            return (time >= start);
        }

        public bool IsStillOpenAt(DayOpenTime dayOpenTime, TimeSpan time)
        {
            if(dayOpenTime.End == "0:00")
            {
                return true;
            }
            else
            {
                var end = TimeSpan.Parse(dayOpenTime.End);
                return (time < end);
            }
            
        }

        public List<OpenTime> GetOpenTimesForDayOfWeek(DayOfWeek day)
        {
            return DayOpenTimes
                .Where(d => d.Days.Contains(day))
                .Select(d => new OpenTime { Start = d.Start, End = d.End })
                .ToList();
        }
    }
}
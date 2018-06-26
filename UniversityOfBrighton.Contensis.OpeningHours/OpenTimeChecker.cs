using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityOfBrighton.Contensis.OpeningHours
{
    /// <summary>
    /// Provides methods to check a set of OpenTimePeriods against DateTime and Type
    /// </summary>
    public class OpenTimeChecker
    {
        private List<OpenTimePeriod> periods;
        public List<OpenTimePeriod> Periods => periods;

        /// <summary>
        /// Initialise with a list of OpenTimePeriods which the checker will use
        /// </summary>
        /// <param name="periods">The list of OpenTimePeriods the checker will use</param>
        public OpenTimeChecker(List<OpenTimePeriod> periods) => this.periods = periods;

        /// <summary>
        /// According to the intialised OpenTimePeriods is this DateTime open?
        /// </summary>
        /// <param name="date">A DateTime to check against intialised OpenTimePeriods</param>
        /// <returns>True if is open False if not</returns>
        public bool IsOpen(DateTime date)
        {
            var matchPeriod = GetMostApplicableTimePeriod(date);
            if (matchPeriod != null)
            {
                return matchPeriod.IsOpen(date);
            }
            else
            {
                // If there was no matching period then must be closed
                return false;
            }
        }

        /// <summary>
        /// According to the intialised OpenTimePeriods is this DateTime open? Records the name of the
        /// OpenTimePeriod used
        /// </summary>
        /// <param name="date">A DateTime to check against intialised OpenTimePeriods</param>
        /// <param name="name">The name of the matching OpenTimePeriod used to determine if open</param>
        /// <returns>True if is open False if not</returns>
        public bool IsOpen(DateTime date, out string name)
        {
            var matchPeriod = GetMostApplicableTimePeriod(date);
            if (matchPeriod != null)
            {
                name = matchPeriod.Name;
                return matchPeriod.IsOpen(date);
            }
            else
            {
                name = $"TimePeriod not found for {date}";
                // If there was no matching period then must be closed
                return false;
            }
        }

        /// <summary>
        /// According to the intialised OpenTimePeriods is this DateTime open? Records the name of the
        /// OpenTimePeriod used
        /// </summary>
        /// <param name="date">A DateTime to check against intialised OpenTimePeriods</param>
        /// <param name="name">The name of the matching OpenTimePeriod used to determine if open</param>
        /// <returns>True if is open False if not</returns>
        public bool IsOpen(DateTime date, out DateTime? nextTime, int maxAheadDays = 7)
        {
            var matchPeriod = GetMostApplicableTimePeriod(date);
            bool isOpen = (matchPeriod != null && matchPeriod.IsOpen(date));
            if (isOpen)
            {
                nextTime = GetNextClosingTime(date, matchPeriod, maxAheadDays);
            }
            else
            {
                nextTime = GetNextOpenTime(date, matchPeriod, maxAheadDays);
            }
            return isOpen;
        }

        private DateTime? GetNextOpenTime(DateTime date, OpenTimePeriod matchPeriod, int maxAheadDays = 7)
        {
            // try to find the next open time in today's matchPeriod
            var dayOpenTimesForToday = matchPeriod.GetDayOpenTimesForDayOfWeek(date.DayOfWeek);
            // get DayOpenTimes for today where start hasn't happened yet
            var opensToCome = dayOpenTimesForToday.Where(d => d.StartTime > date.TimeOfDay);
            if (opensToCome.Count() > 0)
            {
                var earliestStartTime = opensToCome.Min(d => d.StartTime.Value);
                return date.Date.Add(earliestStartTime);
            }

            // TODO add in test for beyond 7 days
            // get the first open time tomorrow (then loop increasing days)
            var loopNumber = 0;
            while (loopNumber < maxAheadDays)
            {
                date = date.AddDays(1);
                matchPeriod = GetMostApplicableTimePeriod(date);
                if(matchPeriod != null)
                {
                    dayOpenTimesForToday = matchPeriod.GetDayOpenTimesForDayOfWeek(date.DayOfWeek);
                    if (dayOpenTimesForToday.Any(d => d.StartTime != null))
                    {
                        var earliestStartTime = dayOpenTimesForToday.Min(d => d.StartTime.Value);
                        return date.Date.Add(earliestStartTime);
                    }

                }
                loopNumber++;
            }
            // by default we return null
            return null;
        }

        private DateTime? GetNextClosingTime(DateTime date, OpenTimePeriod matchPeriod, int maxAheadDays = 7)
        {
            // try to next closing time today
            var dayOpenTimesForToday = matchPeriod.GetDayOpenTimesForDayOfWeek(date.DayOfWeek);
            // get DayOpenTimes for today where end hasn't happened yet
            var closesToCome = dayOpenTimesForToday.Where(d => d.EndTime > date.TimeOfDay);
            if (closesToCome.Count() > 0)
            {
                var earliestEndTime = closesToCome.Min(d => d.EndTime.Value);
                return date.Date.Add(earliestEndTime);
            }

            // TODO add in test for beyond 7 days
            // get the first end time tomorrow (then loop increasing days)
            var loopNumber = 0;
            while (loopNumber < maxAheadDays)
            {
                date = date.AddDays(1);
                matchPeriod = GetMostApplicableTimePeriod(date);
                if (matchPeriod == null)
                {
                    return date.Date;
                }
                else
                {
                    dayOpenTimesForToday = matchPeriod.GetDayOpenTimesForDayOfWeek(date.DayOfWeek);
                    if (dayOpenTimesForToday.Any(d => d.EndTime != null))
                    {
                        var earliestEndTime = dayOpenTimesForToday.Min(d => d.StartTime.Value);
                        return date.Date.Add(earliestEndTime);
                    }
                }
                loopNumber++;
            }
            // by default we return null
            return null;
        }

        /// <summary>
        /// Find the highest priority OpenTimePeriod from intialised list
        /// which is within the time range and isn't for an excluded date
        /// NB If 2 or more periods both match the DateTime and have the same
        /// priority then there is no guarantee which OpenTimePeriod will be returned
        /// </summary>
        /// <param name="date">A DateTime to check against intialised OpenTimePeriods</param>
        /// <returns>OpenTimePeriod which inlcudes the date with the highest priority</returns>
        public OpenTimePeriod GetMostApplicableTimePeriod(DateTime date)
        {
            var matchingPeriods = Periods.Where(p => p.WithinPeriod(date));
            if (matchingPeriods.Any())
            {
                int maxPriority = matchingPeriods.Max(p => p.Priority);
                return matchingPeriods.Where(p => p.Priority == maxPriority).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// For a given DateTime return a list (in case there is more than one) of OpenTimes
        /// useful for outputting on page or for Google MyBusiness
        /// </summary>
        /// <param name="date">A DateTime to check against intialised OpenTimePeriods</param>
        /// <returns>OpenTimes for date, empty list if none matching</returns>
        public List<OpenTimePeriod.OpenTime> GetOpenTimesForDay(DateTime date)
        {
            var period = GetMostApplicableTimePeriod(date);
            if (period != null)
            {
                return period.GetOpenTimesForDayOfWeek(date.DayOfWeek);
            }
            else
            {
                return new List<OpenTimePeriod.OpenTime>();
            }
        }


    }
}
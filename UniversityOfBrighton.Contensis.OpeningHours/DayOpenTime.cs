﻿using System;
using System.Collections.Generic;

namespace UniversityOfBrighton.Contensis.OpeningHours
{
    /// <summary>
    /// Days of the week start and end times apply for
    /// </summary>
    public class DayOpenTime
    {
        public List<DayOfWeek> Days;
        public string Start;
        public string End;

        public TimeSpan StartTime => TimeSpan.Parse(Start);
        public TimeSpan EndTime => TimeSpan.Parse(End);

        private static TimeSpan midnight = new TimeSpan(0, 0, 0);

        /// <summary>
        /// For a given time is it open
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns></returns>
        public bool IsOpenAt(TimeSpan time)
        {
            if (IsOpen24Hours())
            {
                return true;
            }
            else
            {
                return HasOpenedBy(time) && IsStillOpenAt(time);
            }
        }

        /// <summary>
        /// If the start and end time are set to "0:00" then is open 24 hours for these days
        /// </summary>
        /// <returns>True if is open 24 hours (e.g. counts as open for all times on matching days</returns>
        public bool IsOpen24Hours()
        {
            return (StartTime == midnight && EndTime == midnight);
        }

        /// <summary>
        /// Has the Start already occured by this time
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns></returns>
        public bool HasOpenedBy(TimeSpan time)
        {
            return (time >= StartTime);
        }

        /// <summary>
        /// Has the time that this closes already occurred?
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns></returns>
        public bool IsStillOpenAt(TimeSpan time)
        {
            var end = EndTime;
            if (end == midnight)
            {
                return true;
            }
            else
            {
                return (time < end);
            }
        }
    }

}
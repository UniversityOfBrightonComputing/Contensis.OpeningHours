using System;
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
            return (Start == "0:00" && End == "0:00");
        }

        /// <summary>
        /// Has the Start already occured by this time
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns></returns>
        public bool HasOpenedBy(TimeSpan time)
        {
            var start = TimeSpan.Parse(Start);
            return (time >= start);
        }

        /// <summary>
        /// Has the time that this closes already occurred?
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns></returns>
        public bool IsStillOpenAt(TimeSpan time)
        {
            if (End == "0:00")
            {
                return true;
            }
            else
            {
                var end = TimeSpan.Parse(End);
                return (time < end);
            }
        }
    }

}
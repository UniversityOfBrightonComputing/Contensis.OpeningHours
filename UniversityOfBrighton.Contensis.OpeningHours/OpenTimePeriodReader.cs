using System;
using System.Collections.Generic;
using System.Linq;

using Zengenti.Contensis.Delivery;
using Zengenti.Data;

namespace UniversityOfBrighton.Contensis.OpeningHours
{
    /// <summary>
    /// Reader of the OpenTimePeriod 
    /// </summary>
    public class OpenTimePeriodReader
    {
        private ContensisClient Client;
        private List<OpenTimePeriod> allPeriods;
        private string PeriodContentName;

        public List<OpenTimePeriod> AllPeriods
        {
            get
            {
                if(allPeriods == null)
                {
                    throw new InvalidOperationException("List of periods have not yet been fetched from Server using FetchAllOpenTimePeriods()");
                }
                else
                {
                    return allPeriods;
                }
            }

            set
            {
                allPeriods = value;
            }
        }

        public OpenTimePeriodReader(ContensisClient client, string periodContentName = "openTimePeriod")
        {
            Client = client;
            PeriodContentName = periodContentName;
        }

        /// <summary>
        /// Use to see if a date is open
        /// </summary>
        /// <param name="date">The DateTime to check</param>
        /// <returns>True if is open false if not</returns>
        public bool IsOpen(DateTime date, string type)
        {
            var matchPeriod = GetMostApplicableTimePeriod(date, type);
            if(matchPeriod != null)
            {
                return matchPeriod.IsOpen(date);
            }
            else
            {
                return false;
            }
        }

        public OpenTimePeriod GetMostApplicableTimePeriod(DateTime date, string type)
        {
            var matchingPeriods = AllPeriods.Where(p => p.PeriodFor.Contains(type) && p.WithinPeriod(date));
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

        public string GetMostApplicableTimePeriodName(DateTime date, string type)
        {
            var period = GetMostApplicableTimePeriod(date, type);
            if(period != null)
            {
                return period.Name;
            }
            else
            {
                return $"TimePeriod not found for {date}";
            }
            
        }

        public void FetchAllOpenTimePeriods()
        {
            bool morePages = true;
            int pageSize = 25;
            int pageIndex = 0;

            var list = new List<OpenTimePeriod>();

            while (morePages)
            {
                var results = Client.Entries.List<OpenTimePeriod>(PeriodContentName, new PageOptions(pageIndex, pageSize));
                foreach (var item in results.Items)
                {
                    list.Add(item);
                }
                pageIndex += pageSize;
                morePages = list.Count < results.TotalCount;
            }
            AllPeriods = list;
        }

        public List<OpenTimePeriod.OpenTime> GetOpenTimesForDay(DateTime date, string type)
        {
            var period = GetMostApplicableTimePeriod(date, type);
            if(period != null)
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
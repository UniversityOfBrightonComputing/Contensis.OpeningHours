using System.Collections.Generic;
using System.Linq;
using Zengenti.Contensis.Delivery;
using Zengenti.Data;
using System;

namespace UniversityOfBrighton.Contensis.OpeningHours
{
    /// <summary>
    /// Helper class for fetching lists of OpenTimePeriods from CMS
    /// </summary>
    public static class OpenTimePeriodReader
    {
        /// <summary>
        /// Uses Delivery API to get list of OpenTimePeriod from the CMS
        /// Does not use search because the search is cached whereas a list is up-to-date as soon as published
        /// </summary>
        /// <param name="client">Contensis Delivery API client</param>
        /// <param name="typeTaxonomyKey">Taxonomy key of the type to fetch, if null all OpenTimePeriods are returned</param>
        /// <param name="periodContentName">Name of the content type in the CMS default is openTimePeriod</param>
        /// <returns>List of OpenTimePeriods from CMS</returns>
        public static List<OpenTimePeriod> FetchOpenTimePeriods(ContensisClient client, string typeTaxonomyKey = null, string periodContentName = "openTimePeriod")
        {
            var allPeriods = FetchAllOpenTimePeriods(client, periodContentName);
            if(typeTaxonomyKey == null)
            {
                return allPeriods;
            }
            else
            {
                return FilterListByType(allPeriods, typeTaxonomyKey);
            }
        }

        /// <summary>
        /// Use client to fetch all the OpenTimePeriod content type entries from CMS
        /// </summary>
        /// <param name="client">Contensis Delivery API client</param>
        /// <param name="contentName">Name of content type in the CMS</param>
        /// <returns>List of all OpenTimePeriods</returns>
        public static List<OpenTimePeriod> FetchAllOpenTimePeriods(ContensisClient client, string contentName)
        {
            bool morePages = true;
            int pageSize = 25;
            int pageIndex = 0;

            var list = new List<OpenTimePeriod>();

            // NB client.Entries will throw a RestRequestException if contentName isn't found,
            // Would try catch but then will just show closed which is not ideal
            while (morePages)
            {
                var results = client.Entries.List<OpenTimePeriod>(contentName, new PageOptions(pageIndex, pageSize));
                foreach (var item in results.Items)
                {
                    list.Add(item);
                }
                pageIndex += pageSize;
                morePages = list.Count < results.TotalCount;
            }

            return list;
        }

        /// <summary>
        /// Filter list so only mathcing types are returned
        /// </summary>
        /// <param name="list">the list to filter</param>
        /// <param name="typeTaxonomyKey">taxonomy key of the type to filter by</param>
        /// <returns></returns>
        public static List<OpenTimePeriod> FilterListByType(List<OpenTimePeriod> list, string typeTaxonomyKey)
        {
            if(list != null)
            {
                var filteredPeriods = list.Where(p => p.PeriodFor.Contains(typeTaxonomyKey));
                if (filteredPeriods != null)
                {
                    return filteredPeriods.ToList();
                }
            }
            return null;
        }
    }
}
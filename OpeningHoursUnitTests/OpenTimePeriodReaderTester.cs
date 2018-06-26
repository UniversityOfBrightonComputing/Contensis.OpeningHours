using System.Collections.Generic;
using System.Linq;
using Xunit;
using UniversityOfBrighton.Contensis.OpeningHours;

namespace OpeningHoursUnitTests
{
    public class OpenTimePeriodReaderTester
    {
        public static List<OpenTimePeriod> listOfAll = new List<OpenTimePeriod>
        {
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 1",
                PeriodFor = new string[] { "test1", "test2" }
            },
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 2",
                PeriodFor = new string[] { "test2", "test3" }
            },
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 3",
                PeriodFor = new string[] { "test3" }
            }
        };


        [Fact]
        public void FilterListByType()
        {
            Assert.Empty(OpenTimePeriodReader.FilterListByType(listOfAll, null));

            var filterOnTest1 = OpenTimePeriodReader.FilterListByType(listOfAll, "test1");
            Assert.Single(filterOnTest1);
            Assert.True(filterOnTest1[0].Name == "Test Open Time Period 1");

            var filterOnTest2 = OpenTimePeriodReader.FilterListByType(listOfAll, "test2");
            Assert.Equal(2, filterOnTest2.Count);
            var names2 = filterOnTest2.Select(x => x.Name);
            Assert.Contains("Test Open Time Period 1", names2);
            Assert.Contains("Test Open Time Period 2", names2);

            var filterOnTest3 = OpenTimePeriodReader.FilterListByType(listOfAll, "test3");
            Assert.Equal(2, filterOnTest3.Count);
            var names3 = filterOnTest3.Select(x => x.Name);
            Assert.Contains("Test Open Time Period 2", names3);
            Assert.Contains("Test Open Time Period 3", names3);
        }
    }
}

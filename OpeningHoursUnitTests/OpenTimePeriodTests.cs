using System;
using System.Collections.Generic;
using Xunit;
using UniversityOfBrighton.Contensis.OpeningHours;

namespace OpeningHoursUnitTests
{
    public class OpenTimePeriodTests
    {
        /// <summary>
        /// Open Mondays and half day Tuesdays July-Dec excluding Christmas
        /// </summary>
        OpenTimePeriod testPeriod1 = new OpenTimePeriod
        {
            Name = "Test Open Time Period",
            PeriodFor = new string[] { "test1", "test2" },
            Priority = 1,                             
            When = new OpenTimePeriod.DateRange
            {
                From = DateTime.Parse("2018-07-01"),
                To = DateTime.Parse("2018-12-31")
            },
            DayOpenTimes = new List<DayOpenTime>
            {
                new DayOpenTime
                {
                    Days = new List<DayOfWeek> { DayOfWeek.Monday },
                    Start = "9:00",
                    End = "17:00"
                },
                new DayOpenTime
                {
                    Days = new List<DayOfWeek> { DayOfWeek.Tuesday },
                    Start = "8:00",
                    End = "12:30"
                },
            },
            Exceptions = new List<DateTime?>
            {
                null,
                DateTime.Parse("2018-12-25")
            }
        };

        [Fact]
        public void WithinPeriod()
        {
            Assert.True(testPeriod1.WithinPeriod(DateTime.Parse("2018-07-01")));
            Assert.True(testPeriod1.WithinPeriod(DateTime.Parse("2018-12-31")));
            Assert.False(testPeriod1.WithinPeriod(DateTime.Parse("2018-12-25")));
            Assert.False(testPeriod1.WithinPeriod(DateTime.Parse("2018-06-30")));
        }

        [Fact]
        public void MatchesException()
        {
            Assert.True(testPeriod1.MatchesException(DateTime.Parse("2018-12-25")));
            Assert.False(testPeriod1.MatchesException(DateTime.Parse("2018-12-26")));
        }

        [Fact]
        public void IsOpen_ThrowsException()
        {
            var date = DateTime.Parse("2018-12-25");
            Action testDate = () => testPeriod1.IsOpen(date);
            Assert.Throws<ArgumentOutOfRangeException>(testDate);

            date = DateTime.Parse("2019-01-01 00:00:00");
            testDate = () => testPeriod1.IsOpen(date);
            Assert.Throws<ArgumentOutOfRangeException>(testDate);

            date = DateTime.Parse("2017-12-31 23:59:59");
            testDate = () => testPeriod1.IsOpen(date);
            Assert.Throws<ArgumentOutOfRangeException>(testDate);
        }

        [Fact]
        public void IsOpen()
        {

            var testDate = DateTime.Parse("2018-07-01 00:00:00");
            var endOfYear = DateTime.Parse("2018-12-31 23:59:59");
            var christmas = DateTime.Parse("2018-12-25");
            var am9 = TimeSpan.Parse("09:00");
            var pm5 = TimeSpan.Parse("17:00");
            var am8 = TimeSpan.Parse("08:00");
            var pm1230 = TimeSpan.Parse("12:30");

            while (testDate <= endOfYear)
            {
                if(testDate.Date != christmas)
                {
                    if
                    (
                        (
                            testDate.DayOfWeek == DayOfWeek.Monday &&
                            testDate.TimeOfDay >= am9 &&
                            testDate.TimeOfDay < pm5
                        ) ||
                        (
                            testDate.DayOfWeek == DayOfWeek.Tuesday &&
                            testDate.TimeOfDay >= am8 &&
                            testDate.TimeOfDay < pm1230
                        )
                    )
                    {
                        Assert.True(testPeriod1.IsOpen(testDate));
                    }
                    else
                    {
                        Assert.False(testPeriod1.IsOpen(testDate));
                    }
                }                    
                testDate = testDate.AddHours(1);
            }
        }

        [Fact]
        public void GetOpenTimesForDayOfWeek()
        {
            var am9 = TimeSpan.Parse("09:00");
            var pm5 = TimeSpan.Parse("17:00");
            var am8 = TimeSpan.Parse("08:00");
            var pm1230 = TimeSpan.Parse("12:30");

            var mondayOpenTimes = testPeriod1.GetOpenTimesForDayOfWeek(DayOfWeek.Monday);
            Assert.Single(mondayOpenTimes);
            Assert.Equal(am9, TimeSpan.Parse(mondayOpenTimes[0].Start));
            Assert.Equal(pm5, TimeSpan.Parse(mondayOpenTimes[0].End));

            var tuesdayOpenTimes = testPeriod1.GetOpenTimesForDayOfWeek(DayOfWeek.Tuesday);
            Assert.Single(tuesdayOpenTimes);
            Assert.Equal(am8, TimeSpan.Parse(tuesdayOpenTimes[0].Start));
            Assert.Equal(pm1230, TimeSpan.Parse(tuesdayOpenTimes[0].End));

            var wednesdayOpenTimes = testPeriod1.GetOpenTimesForDayOfWeek(DayOfWeek.Wednesday);
            Assert.Empty(wednesdayOpenTimes);
        }

    }
}

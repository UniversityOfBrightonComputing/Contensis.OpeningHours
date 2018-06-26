using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using UniversityOfBrighton.Contensis.OpeningHours;

namespace OpeningHoursUnitTests
{
    public class OpenTimeCheckerTests
    {
        public static List<OpenTimePeriod> allPeriods = new List<OpenTimePeriod>
        {
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 1",
                PeriodFor = new string[] { "test1" },
                Priority = 1,
                When = new OpenTimePeriod.DateRange
                {
                    From = DateTime.Parse("2018-01-01"),
                    To = DateTime.Parse("2018-06-30")
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
                        Start = "9:00",
                        End = "17:00"
                    },
                },
                Exceptions = new List<DateTime?>
                {
                    null,
                    DateTime.Parse("2018-05-07")
                }
            },
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 2",
                PeriodFor = new string[] { "test1", "test3" },
                Priority = 2,
                When = new OpenTimePeriod.DateRange
                {
                    From = DateTime.Parse("2018-06-01"),
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
            },
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 3",
                PeriodFor = new string[] { "test3" },
                Priority = 1,
                When = new OpenTimePeriod.DateRange
                {
                    From = DateTime.Parse("2018-01-01"),
                    To = DateTime.Parse("2018-06-30")
                },
                DayOpenTimes = new List<DayOpenTime>
                {
                    new DayOpenTime
                    {
                        Days = new List<DayOfWeek> { DayOfWeek.Wednesday },
                        Start = "11:00",
                        End = "13:00"
                    },
                    new DayOpenTime
                    {
                        Days = new List<DayOfWeek> { DayOfWeek.Friday },
                        Start = "0:00",
                        End = "00:00"
                    },
                },
                Exceptions = new List<DateTime?>
                {
                    null
                }
            },
            new OpenTimePeriod
            {
                Name = "Test Open Time Period 4",
                PeriodFor = new string[] { "test3" },
                Priority = 2,
                When = new OpenTimePeriod.DateRange
                {
                    From = DateTime.Parse("2018-06-01"),
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
                    DateTime.Parse("2018-12-26")
                }
            }
        };

        

        [Fact]
        public void Constructor()
        {
            var filteredPeriods = OpenTimePeriodReader.FilterListByType(allPeriods, null);
            var checker = new OpenTimeChecker(filteredPeriods);
            Assert.Same(checker.Periods, filteredPeriods);
        }

        [Fact]
        public void IsOpen()
        {
            var filteredPeriods = OpenTimePeriodReader.FilterListByType(allPeriods, "test1");
            var checker = new OpenTimeChecker(filteredPeriods);

            var mayDay = DateTime.Parse("2018-05-07");
            var otherMayMondays = new List<DateTime>
            {
                DateTime.Parse("2018-05-14 09:00:00"),
                DateTime.Parse("2018-05-21 16:59:59"),
                DateTime.Parse("2018-05-28 16:59:59"),
            };

            Assert.False(checker.IsOpen(mayDay));
            otherMayMondays.ForEach(x => Assert.True(checker.IsOpen(x)));

            // 5 June is a Tuesday so should be test2 overriding and closed by 12:30
            var june5am = DateTime.Parse("2018-06-05 08:00:00");
            var june5pm = DateTime.Parse("2018-06-05 12:30:01");
            Assert.True(checker.IsOpen(june5am));
            Assert.False(checker.IsOpen(june5pm));

        }

        [Fact]
        public void IsOpen_Out()
        {
            var filteredPeriods = OpenTimePeriodReader.FilterListByType(allPeriods, "test3");
            var checker = new OpenTimeChecker(filteredPeriods);

            // Christmas is a Tuesday
            var christmasLunch = DateTime.Parse("2018-12-25 12:00:00");

            Assert.True(checker.IsOpen(christmasLunch, out var name));
            Assert.Equal("Test Open Time Period 4", name);

            // Boxing day is a Wednesday , so closed according to Test 2
            // Even though Test 3 says is open (has lower Priority)
            var boxingDayLunch = DateTime.Parse("2018-12-26 12:00:00");

            Assert.False(checker.IsOpen(boxingDayLunch, out name));
            Assert.Equal("Test Open Time Period 2", name);

        }

        [Fact]
        public void GetMostApplicableTimePeriod()
        {
            var filteredPeriods = OpenTimePeriodReader.FilterListByType(allPeriods, "test1");
            var checker = new OpenTimeChecker(filteredPeriods);
            var june5 = DateTime.Parse("2018-06-05"); // Tuesday should be Test 2

            Assert.Equal("Test Open Time Period 2", checker.GetMostApplicableTimePeriod(june5).Name);
        }

        [Fact]
        public void GetOpenTimesForDay()
        {
            var filteredPeriods = OpenTimePeriodReader.FilterListByType(allPeriods, "test1");
            var checker = new OpenTimeChecker(filteredPeriods);
            var june5 = DateTime.Parse("2018-06-05"); // Tuesday should be Test 2

            var times = checker.GetOpenTimesForDay(june5);
            Assert.Single(times);
            Assert.Equal("8:00", times[0].Start);
            Assert.Equal("12:30", times[0].End);
        }
    }
}

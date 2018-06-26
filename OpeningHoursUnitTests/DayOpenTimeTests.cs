using System;
using System.Collections.Generic;
using Xunit;
using UniversityOfBrighton.Contensis.OpeningHours;

namespace OpeningHoursUnitTests
{
    public class DayOpenTimeTests
    {

        [Fact]
        public void IsOpenAt()
        {

            var dayOpenTime = new DayOpenTime
            {
                Days = new List<DayOfWeek> { DayOfWeek.Monday },
                Start = "9:00",
                End = "17:00"
            };

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            TimeSpan time;

            while (hours < 24)
            {
                time = new TimeSpan(hours, minutes, seconds);
                if (hours >= 9 && hours < 17)
                {
                    Assert.True(dayOpenTime.IsOpenAt(time));
                }
                else
                {
                    Assert.False(dayOpenTime.IsOpenAt(time));
                }
                minutes++;
                if(minutes >= 60)
                {
                    minutes = 0;
                    hours++;
                }
            }
        }

        [Fact]
        public void IsOpen24Hours()
        {
            var not24Hours = new DayOpenTime
            {
                Days = new List<DayOfWeek> { DayOfWeek.Monday },
                Start = "0:00",
                End = "17:00"
            };

            Assert.False(not24Hours.IsOpen24Hours());

            not24Hours = new DayOpenTime
            {
                Days = new List<DayOfWeek> { DayOfWeek.Monday },
                Start = "17:00",
                End = "00:00"
            };

            Assert.False(not24Hours.IsOpen24Hours());

            var is24Hours = new DayOpenTime
            {
                Days = new List<DayOfWeek> { DayOfWeek.Monday },
                Start = "00:00",
                End = "00:00"
            };

            Assert.True(is24Hours.IsOpen24Hours());
        }

        [Fact]
        public void HasOpenedBy()
        {

            var dayOpenTime = new DayOpenTime
            {
                Days = new List<DayOfWeek> { DayOfWeek.Monday },
                Start = "9:00",
                End = "17:00"
            };

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            TimeSpan time;

            while (hours < 24)
            {
                time = new TimeSpan(hours, minutes, seconds);
                if (hours >= 9)
                {
                    Assert.True(dayOpenTime.HasOpenedBy(time));
                }
                else
                {
                    Assert.False(dayOpenTime.HasOpenedBy(time));
                }
                minutes++;
                if (minutes >= 60)
                {
                    minutes = 0;
                    hours++;
                }
            }
        }

        [Fact]
        public void IsStillOpenAt()
        {

            var dayOpenTime = new DayOpenTime
            {
                Days = new List<DayOfWeek> { DayOfWeek.Monday },
                Start = "9:00",
                End = "17:00"
            };

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            TimeSpan time;

            while (hours < 24)
            {
                time = new TimeSpan(hours, minutes, seconds);
                if (hours < 17)
                {
                    Assert.True(dayOpenTime.IsStillOpenAt(time));
                }
                else
                {
                    Assert.False(dayOpenTime.IsStillOpenAt(time));
                }
                minutes++;
                if (minutes >= 60)
                {
                    minutes = 0;
                    hours++;
                }
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ISOWeekDate
{
	[TestClass()]
	public class WeekDateTests
	{
		// In a 400-year cycle 71 years have 53 weeks, the rest have 52.
		// Source: https://en.wikipedia.org/wiki/ISO_week_date#Weeks_per_year
		private const int cycleLength = 400;
		private static readonly List<int> longYears = new List<int>()
			{
				4,      9,      15,     20,     26,
				32,     37,     43,     48,     54,
				60,     65,     71,     76,     82,
				88,     93,     99,
						105,    111,    116,    122,
				128,    133,    139,    144,    150,
				156,    161,    167,    172,    178,
				184,    189,    195,
						201,    207,    212,    218,
				224,    229,    235,    240,    246,
				252,    257,    263,    268,    274,
				280,    285,    291,    296,
								303,    308,    314,
				320,    325,    331,    336,    342,
				348,    353,    359,    364,    370,
				376,    381,    387,    392,    398
			};

		private static readonly Dictionary<WeekDate, DateTime> convertedDates = new Dictionary<WeekDate, DateTime>()
			{
				// Transition from 2004 to 2005
				{ new WeekDate(2004, 53, 6), new DateTime(2005, 1, 1) },
				{ new WeekDate(2004, 53, 7), new DateTime(2005, 1, 2) },
				
				// Transition from 2005 to 2006
				{ new WeekDate(2005, 52, 6), new DateTime(2005, 12, 31) },
				{ new WeekDate(2005, 52, 7), new DateTime(2006, 1, 1) },
				{ new WeekDate(2006, 1, 1), new DateTime(2006, 1, 2) },
				
				// Transition from 2006 to 2007
				{ new WeekDate(2006, 52, 7), new DateTime(2006, 12, 31) },
				{ new WeekDate(2007, 1, 1), new DateTime(2007, 1, 1) },
				
				// Transition from 2007 to 2008
				{ new WeekDate(2007, 52, 7), new DateTime(2007, 12, 30) },
				{ new WeekDate(2008, 1, 1), new DateTime(2007, 12, 31) },
				{ new WeekDate(2008, 1, 2), new DateTime(2008, 1, 1) },
				
				// Transition from 2008 to 2009
				{ new WeekDate(2008, 52, 7), new DateTime(2008, 12, 28) },
				{ new WeekDate(2009, 1, 1), new DateTime(2008, 12, 29) },
				{ new WeekDate(2009, 1, 2), new DateTime(2008, 12, 30) },
				{ new WeekDate(2009, 1, 3), new DateTime(2008, 12, 31) },
				{ new WeekDate(2009, 1, 4), new DateTime(2009, 1, 1) },
				
				// Transition from 2009 to 2010
				{ new WeekDate(2009, 52, 4), new DateTime(2009, 12, 31) },
				{ new WeekDate(2009, 53, 5), new DateTime(2010, 1, 1) },
				{ new WeekDate(2009, 53, 6), new DateTime(2010, 1, 2) },
				{ new WeekDate(2009, 53, 7), new DateTime(2010, 1, 3) },

				// Random dates
				{ new WeekDate(1980, 40, 1), new DateTime(1980, 9, 29) },
				{ new WeekDate(2008, 39, 5), new DateTime(2008, 9, 26) },
				{ new WeekDate(2008, 39, 6), new DateTime(2008, 9, 27) },
				{ new WeekDate(2032, 40, 5), new DateTime(2032, 10, 1) }
			};

		[TestMethod()]
		public void GetWeekCountInYearTest()
		{
			for (int year = 1; year <= cycleLength; year++)
			{
				Assert.AreEqual(
					longYears.Contains(year) ? 53 : 52,
					WeekDate.GetWeekCountInYear(year),
					$"{year}");
			}
		}

		[TestMethod()]
		public void GetWeekdayNumberTest()
		{
			var daysOfWeek = new Dictionary<int, DayOfWeek>()
			{
				{ 1, DayOfWeek.Monday },
				{ 2, DayOfWeek.Tuesday },
				{ 3, DayOfWeek.Wednesday },
				{ 4, DayOfWeek.Thursday },
				{ 5, DayOfWeek.Friday },
				{ 6, DayOfWeek.Saturday },
				{ 7, DayOfWeek.Sunday }
			};

			foreach(var dayOfWeek in daysOfWeek)
			{
				Assert.AreEqual(
					dayOfWeek.Key,
					WeekDate.GetWeekdayNumber(dayOfWeek.Value));
			}
		}

		[TestMethod()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetWeekdayNumberArgumentOutOfRangeTest()
		{
			var weekdayNumber = WeekDate.GetWeekdayNumber((DayOfWeek)8);
		}

		[TestMethod()]
		public void GetJanuaryFourthWeekdayTest()
		{
			var date = new WeekDate(2004, 53, 6);
			var expectedWeekday = 7;

			Assert.AreEqual(
				expectedWeekday,
				WeekDate.GetJanuaryFourthWeekday(date));

			Assert.Inconclusive();
		}

		[TestMethod()]
		public void GetOrdinalTest()
		{
			var date = new WeekDate(2008, 39, 6);
			var expectedOrdinal = 271;

			Assert.AreEqual(
				expectedOrdinal,
				date.GetOrdinal());

			Assert.Inconclusive();
		}

		[TestMethod()]
		public void GetDateTime()
		{
			foreach(var date in convertedDates)
			{
				Assert.AreEqual(
					date.Value,
					date.Key.GetDateTime(),
					$"WeekDate: {date.Key.ToString("YYYY-Www-D")}");
			}
		}
	}
}

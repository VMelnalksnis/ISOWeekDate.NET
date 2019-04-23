using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISOWeekDate
{
	[TestClass]
	public class WeekDateTests
	{
		// In a 400-year cycle 71 years have 53 weeks, the rest have 52.
		// Source: https://en.wikipedia.org/wiki/ISO_week_date#Weeks_per_year

		/// <summary>
		/// The number of years in an ISO calendar cycle.
		/// </summary>
		internal const int CycleLength = 400;
#pragma warning disable SA1137 // Elements should have the same indentation
		/// <summary>
		/// All the years from 1 through <see cref="CycleLength"/> that are 53 weeks long.
		/// </summary>
		internal static readonly List<int> LongYears = new List<int>()
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
				376,    381,    387,    392,    398,
			};
#pragma warning restore SA1137 // Elements should have the same indentation

		/// <summary>
		/// Must be in ascending order.
		/// </summary>
		internal static readonly Dictionary<WeekDate, DateTime> DateTimesByWeekDates = new Dictionary<WeekDate, DateTime>()
			{
				{ new WeekDate(1980, 40, 1), new DateTime(1980, 9, 29) },

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
				{ new WeekDate(2008, 39, 5), new DateTime(2008, 9, 26) },
				{ new WeekDate(2008, 39, 6), new DateTime(2008, 9, 27) },

				// Transition from 2008 to 2009
				{ new WeekDate(2008, 52, 7), new DateTime(2008, 12, 28) },
				{ new WeekDate(2009, 1, 1), new DateTime(2008, 12, 29) },
				{ new WeekDate(2009, 1, 2), new DateTime(2008, 12, 30) },
				{ new WeekDate(2009, 1, 3), new DateTime(2008, 12, 31) },
				{ new WeekDate(2009, 1, 4), new DateTime(2009, 1, 1) },

				// Transition from 2009 to 2010
				{ new WeekDate(2009, 53, 4), new DateTime(2009, 12, 31) },
				{ new WeekDate(2009, 53, 5), new DateTime(2010, 1, 1) },
				{ new WeekDate(2009, 53, 6), new DateTime(2010, 1, 2) },
				{ new WeekDate(2009, 53, 7), new DateTime(2010, 1, 3) },
				{ new WeekDate(2032, 40, 5), new DateTime(2032, 10, 1) },
			};

		internal static readonly Dictionary<int, DayOfWeek> DaysOfWeek = new Dictionary<int, DayOfWeek>()
			{
				{ 1, DayOfWeek.Monday },
				{ 2, DayOfWeek.Tuesday },
				{ 3, DayOfWeek.Wednesday },
				{ 4, DayOfWeek.Thursday },
				{ 5, DayOfWeek.Friday },
				{ 6, DayOfWeek.Saturday },
				{ 7, DayOfWeek.Sunday },
			};

		internal static readonly Dictionary<KeyValuePair<string, string>, WeekDate> WeekDatesByFormats = new Dictionary<KeyValuePair<string, string>, WeekDate>()
		{
			{ new KeyValuePair<string, string>("D", "1980-W40-1"), new WeekDate(1980, 40, 1) },
			{ new KeyValuePair<string, string>("d", "1980W401"), new WeekDate(1980, 40, 1) },
			{ new KeyValuePair<string, string>("Y", "1980-W40"), new WeekDate(1980, 40) },
			{ new KeyValuePair<string, string>("y", "1980W40"), new WeekDate(1980, 40) },
			{ new KeyValuePair<string, string>("D", "2032-W10-1"), new WeekDate(2032, 10, 1) },
			{ new KeyValuePair<string, string>("d", "2032W101"), new WeekDate(2032, 10, 1) },
			{ new KeyValuePair<string, string>("Y", "2032-W10"), new WeekDate(2032, 10) },
			{ new KeyValuePair<string, string>("y", "2032W10"), new WeekDate(2032, 10) },
		};

		[TestMethod]
		[Priority(1)]
		public void WeekDateFromDateTimeTest()
		{
			foreach (var convertedDate in DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Key,
					new WeekDate(convertedDate.Value),
					$"{convertedDate.Value}");
			}
		}

		[TestMethod]
		[Priority(1)]
		public void GetWeekCountInYearTest()
		{
			for (int year = 1; year <= CycleLength; year++)
			{
				Assert.AreEqual(
					LongYears.Contains(year) ? 53 : 52,
					WeekDate.GetWeeksInYear(year),
					$"{year}");
			}
		}

		[TestMethod]
		[Priority(1)]
		public void GetWeekdayNumberTest()
		{
			foreach (var dayOfWeek in DaysOfWeek)
			{
				Assert.AreEqual(
					dayOfWeek.Key,
					WeekDate.GetWeekdayNumber(dayOfWeek.Value));
			}
		}

		[TestMethod]
		[Priority(1)]
		public void GetWeekNumberTest()
		{
			foreach (var convertedDate in DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Key.Week,
					WeekDate.GetWeekOfYear(convertedDate.Value));
			}
		}

		[TestMethod]
		[Priority(1)]
		public void GetYearTest()
		{
			foreach (var convertedDate in DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Key.Year,
					WeekDate.GetYear(convertedDate.Value));
			}
		}

		[TestMethod]
		[Priority(1)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetWeekdayNumberArgumentOutOfRangeTest()
		{
			WeekDate.GetWeekdayNumber((DayOfWeek)8);
		}

		[TestMethod]
		[Priority(2)]
		public void ParseExactFormatSpecifiersTest()
		{
			foreach (KeyValuePair<KeyValuePair<string, string>, WeekDate> weekDateByFormat in WeekDatesByFormats)
			{
				Assert.AreEqual(
					weekDateByFormat.Value,
					WeekDate.ParseExact(weekDateByFormat.Key.Value, weekDateByFormat.Key.Key),
					$"Format: {weekDateByFormat.Key.Key}; String: {weekDateByFormat.Key.Value}");

				Assert.AreEqual(
					weekDateByFormat.Value,
					WeekDate.ParseExact(weekDateByFormat.Key.Value, WeekDate.Formats[weekDateByFormat.Key.Key]),
					$"Format: {WeekDate.Formats[weekDateByFormat.Key.Key]}; String: {weekDateByFormat.Key.Value}");
			}
		}

		// [TestMethod]
		public void GetOrdinalTest()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		[TestCategory("IComparable")]
		[TestCategory("Operator")]
		public void LessThanTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex < secondIndex,
						weekdates[firstIndex] < weekdates[secondIndex],
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IComparable")]
		[TestCategory("Operator")]
		public void GreaterThanTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex > secondIndex,
						weekdates[firstIndex] > weekdates[secondIndex],
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IComparable")]
		[TestCategory("Operator")]
		public void LessThanOrEqualToTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex <= secondIndex,
						weekdates[firstIndex] <= weekdates[secondIndex],
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IComparable")]
		[TestCategory("Operator")]
		public void GreaterThanOrEqualToTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex >= secondIndex,
						weekdates[firstIndex] >= weekdates[secondIndex],
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IComparable")]
		public void CompareToWeekDateTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex.CompareTo(secondIndex),
						weekdates[firstIndex].CompareTo(weekdates[secondIndex]),
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IComparable")]
		public void CompareToWeekDateNullTest()
		{
			Assert.AreEqual(1, DateTimesByWeekDates.Keys.First().CompareTo(null));
		}

		[TestMethod]
		[TestCategory("IComparable")]
		public void CompareToObjectTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex.CompareTo(secondIndex),
						weekdates[firstIndex].CompareTo((object)weekdates[secondIndex]),
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IComparable")]
		public void CompareToObjectNullTest()
		{
			Assert.AreEqual(1, DateTimesByWeekDates.Keys.First().CompareTo((object)null));
		}

		[TestMethod]
		[TestCategory("IComparable")]
		[ExpectedException(typeof(ArgumentException))]
		public void CompareToObjectArgumentExceptionTest()
		{
			DateTimesByWeekDates.Keys.First().CompareTo(new object());
		}

		[TestMethod]
		[TestCategory("IConvertible")]
		public void ToDateTimeTest()
		{
			foreach (var convertedDate in DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Value,
					convertedDate.Key.ToDateTime(CultureInfo.InvariantCulture),
					$"WeekDate: {convertedDate.Key.ToString("YYYY-Www-D")}");
			}
		}

		[TestMethod]
		[TestCategory("IEquatable")]
		[TestCategory("Operator")]
		public void EqualToTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex == secondIndex,
						weekdates[firstIndex] == weekdates[secondIndex],
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IEquatable")]
		[TestCategory("Operator")]
		public void NotEqualToTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex != secondIndex,
						weekdates[firstIndex] != weekdates[secondIndex],
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}

		[TestMethod]
		[TestCategory("IEquatable")]
		public void EqualsTest()
		{
			var weekdates = DateTimesByWeekDates.Keys.ToArray();

			for (int firstIndex = 0; firstIndex < weekdates.Length; firstIndex++)
			{
				for (int secondIndex = 0; secondIndex < weekdates.Length; secondIndex++)
				{
					Assert.AreEqual(
						firstIndex.Equals(secondIndex),
						weekdates[firstIndex].Equals(weekdates[secondIndex]),
						$"{weekdates[firstIndex].ToString()} CompareTo {weekdates[secondIndex].ToString()}");
				}
			}
		}
	}
}

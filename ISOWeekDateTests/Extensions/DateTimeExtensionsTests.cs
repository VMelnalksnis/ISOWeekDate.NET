using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISOWeekDate;

namespace ISOWeekDate.Extensions
{
	[TestClass]
	public class DateTimeExtensionsTests
	{
		[TestMethod]
		[Priority(1)]
		[TestCategory("Extensions")]
		public void GetISOWeekDateDayTest()
		{
			foreach (var convertedDate in WeekDateTests.DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Key.Weekday,
					convertedDate.Value.GetWeekDateDay(),
					convertedDate.Value.ToShortDateString());
			}
		}

		[TestMethod]
		[Priority(1)]
		[TestCategory("Extensions")]
		public void GetISOWeekDateWeekTest()
		{
			foreach (var convertedDate in WeekDateTests.DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Key.Week,
					convertedDate.Value.GetWeekDateWeek(),
					convertedDate.Value.ToShortDateString());
			}
		}

		[TestMethod]
		[Priority(1)]
		[TestCategory("Extensions")]
		public void GetISOWeekDateYearTest()
		{
			foreach (var convertedDate in WeekDateTests.DateTimesByWeekDates)
			{
				Assert.AreEqual(
					convertedDate.Key.Year,
					convertedDate.Value.GetWeekDateYear(),
					convertedDate.Value.ToShortDateString());
			}
		}
	}
}

using System;

namespace ISOWeekDate.Extensions
{
	public static class DateTimeExtensions
	{
		public static int GetWeekDateDay(this DateTime dateTime)
		{
			return WeekDate.GetWeekdayNumber(dateTime.DayOfWeek);
		}

		public static int GetWeekDateWeek(this DateTime dateTime)
		{
			return WeekDate.GetWeekOfYear(dateTime);
		}

		public static int GetWeekDateYear(this DateTime dateTime)
		{
			return WeekDate.GetYear(dateTime);
		}
	}
}

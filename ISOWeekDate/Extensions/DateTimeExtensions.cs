using System;

namespace ISOWeekDate.Extensions
{
	public static class DateTimeExtensions
	{
		public static int GetWeekdayNumber(this DateTime dateTime)
		{
			return WeekDate.GetWeekdayNumber(dateTime.DayOfWeek);
		}

		public static int GetWeekNumber(this DateTime dateTime)
		{
			return WeekDate.GetWeekOfYear(dateTime);
		}

		public static int GetISOWeekDateYear(this DateTime dateTime)
		{
			return WeekDate.GetYear(dateTime);
		}
	}
}

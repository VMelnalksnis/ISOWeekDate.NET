using ISOWeekDate.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ISOWeekDate
{
	/// <summary>
	/// Represents an ISO 8601 week date.
	/// </summary>
	/// <seealso cref="https://en.wikipedia.org/wiki/Week_date"/>
	public class WeekDate : IFormattable, IEquatable<WeekDate>
	{
		#region Fields
		private const int minimumYear = 1;
		private const int maximumYear = 9999;
		private const int minimumWeek = 1;
		private const int minimumDay = 1;
		private const int maximumDay = 7;

		private const string _completeBasicFormatSpecifier = "1";
		private const string _completeBasicFormat = "YYYYWwwD";

		private const string _completeExtendedFormatSpecifier = "2";
		private const string _completeExtendedFormat = "YYYY-Www-D";

		private const string _reducedBasicFormatSpecifier = "3";
		private const string _reducedBasicFormat = "YYYYWww";

		private const string _reducedExtendedFormatSpecifier = "4";
		private const string _reducedExtendedFormat = "YYYY-Www";

		private static readonly Dictionary<string, string> _formats = new Dictionary<string, string>()
		{
			{ _completeBasicFormatSpecifier, _completeBasicFormat },
			{ _completeExtendedFormatSpecifier, _completeExtendedFormat },
			{ _reducedBasicFormatSpecifier, _reducedBasicFormat },
			{ _reducedExtendedFormatSpecifier, _reducedExtendedFormat }
		};
		#endregion

		#region Properties
		/// <summary>
		/// Gets the ordinal day number in the week represented by this instance.
		/// </summary>
		/// <value>
		/// The day component, expressed as a value between 1 and 7, 
		/// where 1 represents Monday and 7 represents Sunday.
		/// </value>
		public int Weekday { get; }
		/// <summary>
		/// Gets the ordinal week number in the year represented by this instace.
		/// </summary>
		/// <value>
		/// The week component, expressed as a value between 1 and 53.
		/// </value>
		public int Week { get; }
		/// <summary>
		/// Gets the year represented by this instance.
		/// </summary>
		/// <value>
		/// The year component, expressed as a value between 1 and 9999.
		/// </value>
		public int Year { get; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="WeekDate"/> class to a specified <see cref="DateTime"/>.
		/// </summary>
		/// <param name="date"></param>
		public WeekDate(DateTime date)
		{
			Weekday = date.GetWeekdayNumber();
			Week = date.GetWeekNumber();
			Year = date.GetISOWeekDateYear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// 
		/// <param name="year"></param>
		/// <param name="week"></param>
		/// <param name="weekday"></param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="year"/> is less than 1 or greater than 9999.
		/// -or-
		/// <paramref name="week"/> is less than 1 or greater than week count in <paramref name="year"/>.
		/// -or-
		/// <paramref name="weekday"/> is less than 1 or greater than 7.
		/// </exception>
		public WeekDate(int year, int week, int weekday)
		{
			if (year < minimumYear || year > maximumYear)
			{
				throw new ArgumentOutOfRangeException(nameof(year), year, "Provided year does not represent a valid year.");
			}

			if (week < minimumWeek || week > GetWeekCountInYear(year))
			{
				throw new ArgumentOutOfRangeException(nameof(week), week, "Provided week does not represent a valid ordinal week number.");
			}

			if (weekday < minimumDay || weekday > maximumDay)
			{
				throw new ArgumentOutOfRangeException(nameof(weekday), weekday, "Provided weekday does not represent a valid ordinal day number.");
			}

			Year = year;
			Week = week;
			Weekday = weekday;
		}
		#endregion

		#region Methods
		private static int P(int year) => (year + (year / 4) - (year / 100) + (year / 400)) % 7;

		private static int GetWeekCountInYear(int year) => 52 + ((P(year) == 4 || P(year - 1) == 3) ? 1 : 0);

		/// <summary>
		/// Gets the ordinal day number in the week of <paramref name="dayOfWeek"/>.
		/// </summary>
		/// 
		/// <param name="dayOfWeek"></param>
		/// 
		/// <returns>
		/// A digit from 1 through 7, beginning with Monday and ending with Sunday.
		/// </returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="dayOfWeek"/> is not one of the <see cref="DayOfWeek"/> values.
		/// </exception>
		public static int GetWeekdayNumber(DayOfWeek dayOfWeek)
		{
			switch (dayOfWeek)
			{
				case DayOfWeek.Monday:
					return 1;
				case DayOfWeek.Tuesday:
					return 2;
				case DayOfWeek.Wednesday:
					return 3;
				case DayOfWeek.Thursday:
					return 4;
				case DayOfWeek.Friday:
					return 5;
				case DayOfWeek.Saturday:
					return 6;
				case DayOfWeek.Sunday:
					return 7;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(dayOfWeek),
						dayOfWeek,
						"Unknown day of week.");
			}
		}

		/// <summary>
		/// Gets the week number of <paramref name="dateTime"/>.
		/// </summary>
		/// 
		/// <param name="dateTime"></param>
		/// 
		/// <returns>
		/// 
		/// </returns>
		public static int GetWeekNumber(DateTime dateTime)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dateTime);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				dateTime = dateTime.AddDays(3);
			}

			// Return the week of our adjusted day
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		public static int GetYear(DateTime dateTime)
		{
			return dateTime.Year - ((dateTime.Month == 1 && dateTime.GetWeekNumber() == 53) ? 1 : 0);
		}
		#endregion

		#region IFormattable
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return ToString(format);
		}

		public string ToString(string format)
		{
			if (!_formats.ContainsKey(format) && !_formats.ContainsValue(format))
			{
				throw new FormatException($"\"{format}\" is not a valid WeekDate format.");
			}

			if (_formats.ContainsKey(format))
			{
				format = _formats[format];
			}

			var builder = new StringBuilder(format, format.Length);
			builder.Replace("YYYY", Year.ToString("F4"));
			builder.Replace("ww", Week.ToString("F2"));
			builder.Replace("D", Weekday.ToString("F1"));

			return builder.ToString();
		}
		#endregion

		#region IEquatable<WeekDate>
		public override bool Equals(object obj)
		{
			return Equals(obj as WeekDate);
		}

		public bool Equals(WeekDate other)
		{
			return other != null &&
				   Weekday == other.Weekday &&
				   Week == other.Week &&
				   Year == other.Year;
		}

		public override int GetHashCode()
		{
			var hashCode = 1583253038;
			hashCode = hashCode * -1521134295 + Weekday.GetHashCode();
			hashCode = hashCode * -1521134295 + Week.GetHashCode();
			hashCode = hashCode * -1521134295 + Year.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(WeekDate date1, WeekDate date2)
		{
			return EqualityComparer<WeekDate>.Default.Equals(date1, date2);
		}

		public static bool operator !=(WeekDate date1, WeekDate date2)
		{
			return !(date1 == date2);
		}
		#endregion
	}
}

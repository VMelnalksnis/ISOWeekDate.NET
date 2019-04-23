using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("ISOWeekDateTests")]

namespace ISOWeekDate
{
	/// <summary>
	/// Represents an ISO 8601 week date.
	/// </summary>
	/// <seealso cref="https://en.wikipedia.org/wiki/Week_date"/>
	public partial class WeekDate
	{
		private const int MinimumYear = 1;
		private const int MaximumYear = 9999;
		private const int MinimumWeek = 1;
		private const int MinimumDay = 1;
		private const int MaximumDay = 7;
		private const int WeeksInShortYear = 52;
		private const int WeeksInLongYear = 53;

		private const string CompleteBasicFormatSpecifier = "d";
		private const string CompleteBasicFormat = "YYYYWwwD";

		private const string CompleteExtendedFormatSpecifier = "D";
		private const string CompleteExtendedFormat = "YYYY-Www-D";

		private const string ReducedBasicFormatSpecifier = "y";
		private const string ReducedBasicFormat = "YYYYWww";

		private const string ReducedExtendedFormatSpecifier = "Y";
		private const string ReducedExtendedFormat = "YYYY-Www";

		internal static readonly Dictionary<string, string> Formats = new Dictionary<string, string>()
		{
			{ CompleteBasicFormatSpecifier, CompleteBasicFormat },
			{ CompleteExtendedFormatSpecifier, CompleteExtendedFormat },
			{ ReducedBasicFormatSpecifier, ReducedBasicFormat },
			{ ReducedExtendedFormatSpecifier, ReducedExtendedFormat },
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="WeekDate"/> class to the specified date.
		/// </summary>
		///
		/// <param name="date">The date.</param>
		public WeekDate(DateTime date)
		{
			this.Weekday = GetWeekdayNumber(date.DayOfWeek);
			this.Week = GetWeekOfYear(date);
			this.Year = GetYear(date);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WeekDate"/> class to the specified year, week and day.
		/// </summary>
		///
		/// <param name="year">The year (1 through 9999)>.</param>
		/// <param name="week">The week (1 through the number of weeks in <paramref name="year"/>).</param>
		/// <param name="weekday">The day (1 through 7).</param>
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
			if (year < MinimumYear || year > MaximumYear)
			{
				throw new ArgumentOutOfRangeException(nameof(year), year, "Provided year does not represent a valid year.");
			}

			if (week < MinimumWeek || week > GetWeeksInYear(year))
			{
				throw new ArgumentOutOfRangeException(nameof(week), week, "Provided week does not represent a valid ordinal week number.");
			}

			if (weekday < MinimumDay || weekday > MaximumDay)
			{
				throw new ArgumentOutOfRangeException(nameof(weekday), weekday, "Provided weekday does not represent a valid ordinal day number.");
			}

			this.Year = year;
			this.Week = week;
			this.Weekday = weekday;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WeekDate"/> class to the specified year and week, and a day of 0.
		/// </summary>
		///
		/// <param name="year">The year (1 through 9999)>.</param>
		/// <param name="week">The week (1 through the number of weeks in <paramref name="year"/>).</param>
		///
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="year"/> is less than 1 or greater than 9999.
		/// -or-
		/// <paramref name="week"/> is less than 1 or greater than week count in <paramref name="year"/>.
		/// </exception>
		///
		/// <remarks>
		/// This is intended to only be used when parsing reduced format week date strings, which don't contain the day of the week.
		/// </remarks>
		internal WeekDate(int year, int week)
		{
			if (year < MinimumYear || year > MaximumYear)
			{
				throw new ArgumentOutOfRangeException(nameof(year), year, "Provided year does not represent a valid year.");
			}

			if (week < MinimumWeek || week > GetWeeksInYear(year))
			{
				throw new ArgumentOutOfRangeException(nameof(week), week, "Provided week does not represent a valid ordinal week number.");
			}

			this.Year = year;
			this.Week = week;
		}

		/// <summary>
		/// Gets the ordinal day number in the week represented by this instance.
		/// </summary>
		/// <value>
		/// The day component, expressed as a value between 1 and 7, where 1 represents Monday and 7 represents Sunday.
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

		/// <summary>
		/// Returns the number of weeks in the specified year.
		/// </summary>
		///
		/// <param name="year">And integer that represents the year.</param>
		///
		/// <returns>The number of weeks in the specified year.</returns>
		///
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="year"/> is less than 1
		/// -or-
		/// <paramref name="year"/> is greater than 9999.
		/// </exception>
		///
		/// <seealso cref="http://www.staff.science.uu.nl/~gent0113/calendar/isocalendar.htm"/>
		public static int GetWeeksInYear(int year)
		{
			if (year < MinimumYear || year > MaximumYear)
			{
				throw new ArgumentOutOfRangeException(nameof(year), year, "Year is outside the range defined by ISO 8601.");
			}

			int F(int x) => (x + (x / 4) - (x / 100) + (x / 400)) % 7;

			return F(year) == 4 || F(year - 1) == 3 ? WeeksInLongYear : WeeksInShortYear;
		}

		/// <summary>
		/// Gets the <see cref="WeekDate"/> ordinal day number of the specified <see cref="DayOfWeek"/>.
		/// </summary>
		///
		/// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to read.</param>
		///
		/// <returns>
		/// A integer from 1 through 7, beginning with Monday and ending with Sunday.
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
					throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, "Unknown day of week.");
			}
		}

		/// <summary>
		/// Gets the <see cref="WeekDate"/> week of the specified <see cref="DateTime"/>.
		/// </summary>
		///
		/// <param name="dateTime">The <see cref="DateTime"/> to read.</param>
		///
		/// <returns>
		/// An integer that represents the week date week in <paramref name="dateTime"/>.
		/// </returns>
		public static int GetWeekOfYear(DateTime dateTime)
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

		/// <summary>
		/// Gets the <see cref="WeekDate"/> year of the specified <see cref="DateTime"/>.
		/// </summary>
		///
		/// <param name="dateTime">The <see cref="DateTime"/> to read.</param>
		///
		/// <returns>
		/// An integer that represents the week date year in <paramref name="dateTime"/>.
		/// </returns>
		public static int GetYear(DateTime dateTime)
		{
			// The year of a week is equal to whatever the date is of the thursday of that week
			return dateTime.AddDays(4 - GetWeekdayNumber(dateTime.DayOfWeek)).Year;
		}

		/// <summary>
		/// Converts the specified string representation of a ISO 8601 week date to its <see cref="WeekDate"/> equivalent using the specified format. The format of the string representation must match the specified format exactly.
		/// </summary>
		///
		/// <param name="s">A string that contains a date and time to convert.</param>
		/// <param name="format">A format specifier that defines the required format of <paramref name="s"/>.</param>
		///
		/// <returns>
		/// An object that is equivalent to the date and time contained in <paramref name="s"/>, as specified by <paramref name="format"/>.
		/// </returns>
		///
		/// <exception cref="ArgumentNullException">
		/// <paramref name="s"/> or <paramref name="format"/> is <see langword="null"/>.
		/// </exception>
		///
		/// <exception cref="FormatException">
		/// <paramref name="s"/> or <paramref name="format"/> is an empty string.
		/// -or-
		/// <paramref name="format"/> is not a valid ISO 8601 week date format.
		/// -or-
		/// <paramref name="s"/> does not contain an ISO 8601 week date that corresponds to the pattern specified in <paramref name="format"/>.
		/// </exception>
		public static WeekDate ParseExact(string s, string format)
		{
			if (s == null)
			{
				throw new ArgumentNullException(nameof(s));
			}
			else if (s == string.Empty)
			{
				throw new FormatException("Source string is empty.");
			}

			if (format == null)
			{
				throw new ArgumentNullException(nameof(format));
			}
			else if (format == string.Empty)
			{
				throw new FormatException("Format string is empty.");
			}
			else if (!Formats.Keys.Contains(format) && !Formats.Values.Contains(format))
			{
				throw new FormatException($"Format {format} is not a valid week date format.");
			}

			// If format is a specifier, get the format it represents
			if (Formats.Keys.Contains(format))
			{
				format = Formats[format];
			}

			if (s.Length != format.Length)
			{
				throw new FormatException($"String {s} does not contain an ISO 8601 week date that corresponds to the patern specified in format {format}.");
			}

			try
			{
				var yearString = s.Split('W')[0].TrimEnd('-');

				if (!int.TryParse(yearString, out var year))
				{
					throw new FormatException($"String {s} does not contain an ISO 8601 week date that corresponds to the patern specified in format {format}.");
				}

				var weekString = s.Split('W')[1].Substring(0, 2);

				if (!int.TryParse(weekString, out var week))
				{
					throw new FormatException($"String {s} does not contain an ISO 8601 week date that corresponds to the patern specified in format {format}.");
				}

				if (format == CompleteBasicFormat || format == CompleteExtendedFormat)
				{
					var dayString = s.Split('W')[1].Substring(2).TrimStart('-');

					if (!int.TryParse(dayString, out var day))
					{
						throw new FormatException($"String {s} does not contain an ISO 8601 week date that corresponds to the patern specified in format {format}.");
					}

					return new WeekDate(year, week, day);
				}
				else
				{
					return new WeekDate(year, week);
				}
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw new FormatException($"String {s} does not contain an ISO 8601 week date that corresponds to the patern specified in format {format}.", e);
			}
		}

		// These versions don't support IConvertible; this is the only method that has an actual implementation
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2

		public DateTime ToDateTime(IFormatProvider provider)
		{
			var ordinalDay = (this.Week * 7) + this.Weekday - (GetWeekdayNumber(new DateTime(this.Year, 1, 4).DayOfWeek) + 3);

			return new DateTime(this.Year, 1, 1).AddDays(this.GetDayOfYear() - 1);
		}

#endif

		/// <summary>
		/// Returns the ordinal day number in the year represented by this instace.
		/// </summary>
		///
		/// <returns>
		/// A positive integer that represents the ordinal day of the year of the current instance.
		/// </returns>
		public int GetDayOfYear()
		{
			return (this.Week * 7) + this.Weekday - (GetWeekdayNumber(new DateTime(this.Year, 1, 4).DayOfWeek) + 3);
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as WeekDate);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			var hashCode = 1583253038;
			hashCode = (hashCode * -1521134295) + this.Weekday.GetHashCode();
			hashCode = (hashCode * -1521134295) + this.Week.GetHashCode();
			hashCode = (hashCode * -1521134295) + this.Year.GetHashCode();
			return hashCode;
		}
	}

	public partial class WeekDate : IComparable, IComparable<WeekDate>
	{
		public static bool operator <(WeekDate date1, WeekDate date2)
		{
			return date1.CompareTo(date2) < 0;
		}

		public static bool operator >(WeekDate date1, WeekDate date2)
		{
			return date1.CompareTo(date2) > 0;
		}

		public static bool operator <=(WeekDate date1, WeekDate date2)
		{
			return date1.CompareTo(date2) <= 0;
		}

		public static bool operator >=(WeekDate date1, WeekDate date2)
		{
			return date1.CompareTo(date2) >= 0;
		}

		/// <inheritdoc/>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			switch (obj)
			{
				case WeekDate otherWeekDate:
					return this.CompareTo(otherWeekDate);
				default:
					throw new ArgumentException($"Object is not a {typeof(WeekDate)}", nameof(obj));
			}
		}

		/// <inheritdoc/>
		public int CompareTo(WeekDate other)
		{
			if (other == null)
			{
				return 1;
			}

			if (this.Year.CompareTo(other.Year) != 0)
			{
				return this.Year.CompareTo(other.Year);
			}
			else if (this.Week.CompareTo(other.Week) != 0)
			{
				return this.Week.CompareTo(other.Week);
			}
			else
			{
				return this.Weekday.CompareTo(other.Weekday);
			}
		}
	}

// IConvertible is support from .NETStandard 1.3 and forward
#if !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2

	public partial class WeekDate : IConvertible
	{
		/// <inheritdoc/>
		public TypeCode GetTypeCode()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public bool ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public byte ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public DateTime ToDateTime(IFormatProvider provider)
		{
			return new DateTime(this.Year, 1, 1).AddDays(this.GetDayOfYear() - 1);
		}

		/// <inheritdoc/>
		public decimal ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public double ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public short ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public int ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public long ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public sbyte ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public float ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public string ToString(IFormatProvider provider)
		{
			return this.ToString();
		}

		/// <inheritdoc/>
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public ushort ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public uint ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public ulong ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}
	}

#endif

	public partial class WeekDate : IEquatable<WeekDate>
	{
		public static bool operator ==(WeekDate date1, WeekDate date2)
		{
			return EqualityComparer<WeekDate>.Default.Equals(date1, date2);
		}

		public static bool operator !=(WeekDate date1, WeekDate date2)
		{
			return !(date1 == date2);
		}

		/// <inheritdoc/>
		public bool Equals(WeekDate other)
		{
			return this.CompareTo(other) == 0;
		}
	}

	public partial class WeekDate : IFormattable
	{
		/// <inheritdoc/>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToString(format);
		}

		public string ToString(string format)
		{
			if (!Formats.ContainsKey(format) && !Formats.ContainsValue(format))
			{
				throw new FormatException($"\"{format}\" is not a valid WeekDate format.");
			}

			if (Formats.ContainsKey(format))
			{
				format = Formats[format];
			}

			var builder = new StringBuilder(format, format.Length);
			builder.Replace("YYYY", this.Year.ToString("D4"));
			builder.Replace("ww", this.Week.ToString("D2"));
			builder.Replace("D", this.Weekday.ToString("D1"));

			return builder.ToString();
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return this.ToString(Formats["D"]);
		}
	}
}

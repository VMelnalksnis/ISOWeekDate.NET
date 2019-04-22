using System;
using System.Globalization;
using ISOWeekDate.Extensions;

namespace ISOWeekDate
{
	public class ISOWeekDateFormat : IFormatProvider, ICustomFormatter
	{
		public static string ShortCompactPatternSpecifier
		{
			get { return "v"; }
		}

		public static string ShortExtendedPatternSpecifier
		{
			get { return "V"; }
		}

		public static string FullCompactPatternSpecifier
		{
			get { return "w"; }
		}

		public static string FullExtendedPatternSpecifier
		{
			get { return "W"; }
		}

		/// <inheritdoc/>
		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
			{
				return this;
			}
			else
			{
				return null;
			}
		}

		/// <inheritdoc/>
		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			// Provide default formatting if arg is not an DateTime
			if (arg.GetType() != typeof(DateTime))
			{
				try
				{
					return this.HandleOtherFormats(format, arg);
				}
				catch (FormatException e)
				{
					throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
				}
			}

			var date = (DateTime)arg;

			if (format == ShortCompactPatternSpecifier)
			{
				return string.Format("{0,4}W{1,2}", date.GetISOWeekDateYear(), date.GetWeekNumber());
			}
			else if (format == ShortExtendedPatternSpecifier)
			{
				return string.Format("{0,4}-W{1,2}", date.GetISOWeekDateYear(), date.GetWeekNumber());
			}
			else if (format == FullCompactPatternSpecifier)
			{
				return string.Format("{0,4}W{1,2}{2,1}", date.GetISOWeekDateYear(), date.GetWeekNumber(), date.GetWeekdayNumber());
			}
			else if (format == FullExtendedPatternSpecifier)
			{
				return string.Format("{0,4}-W{1,2}-{2,1}", date.GetISOWeekDateYear(), date.GetWeekNumber(), date.GetWeekdayNumber());
			}
			else
			{
				// Provide default formatting for unsupported format strings.
				try
				{
					return this.HandleOtherFormats(format, arg);
				}
				catch (FormatException e)
				{
					throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
				}
			}
		}

		private string HandleOtherFormats(string format, object arg)
		{
			if (arg is IFormattable)
			{
				return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
			}
			else if (arg != null)
			{
				return arg.ToString();
			}
			else
			{
				return string.Empty;
			}
		}
	}
}

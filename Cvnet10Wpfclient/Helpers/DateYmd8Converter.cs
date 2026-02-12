using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Runtime;

namespace Cvnet10Wpfclient.Helpers;
public sealed class DateYmd8Converter : IValueConverter {
	private const string Format = "yyyyMMdd";

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is string s && !string.IsNullOrWhiteSpace(s) &&
			DateTime.TryParseExact(s, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) {
			return date;
		}
		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is DateTime date) {
			return date.ToString(Format, CultureInfo.InvariantCulture);
		}
		return string.Empty;
	}
}

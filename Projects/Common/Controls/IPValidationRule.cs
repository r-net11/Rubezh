using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Controls
{
	public class IPValidationRule : ValidationRule
	{
		private const string IPRegExp = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
		private Regex _regex;
		public IPValidationRule()
		{
			_regex = new Regex(IPRegExp, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		}
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value != null && !_regex.IsMatch((string)value))
				return new ValidationResult(false, "Неверный формат IP адреса");
			return new ValidationResult(true, null);
		}
	}
}

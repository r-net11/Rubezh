using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

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
				return new ValidationResult(false, Resources.Language.IpValidationRule.Validate_Error);
			return new ValidationResult(true, null);
		}
	}
}
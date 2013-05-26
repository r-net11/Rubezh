using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Controls.Converters
{
	[ContentProperty("Cases")]
	public class SwitchConverter : IValueConverter
	{
		public SwitchConverter()
			: this(new SwitchCaseCollection())
		{
		}
		internal SwitchConverter(SwitchCaseCollection cases)
		{
			Contract.Requires(cases != null);

			Cases = cases;
			StringComparison = StringComparison.OrdinalIgnoreCase;
		}

		public SwitchCaseCollection Cases { get; private set; }
		public StringComparison StringComparison { get; set; }
		public object Else { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Cases.FirstOrDefault(x => x.When == null) ?? Else;
			foreach (var c in Cases.Where(x => x.When != null))
			{
				if (value is string && c.When is string && String.Equals((string)value, (string)c.When, StringComparison))
					return c.Then;
				object when = c.When;
				if (TryConvert(culture, value, ref when) && value.Equals(when))
					return c.Then;
			}
			return Else;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private static bool TryConvert(CultureInfo culture, object value1, ref object value2)
		{
			Type type1 = value1.GetType();
			Type type2 = value2.GetType();

			if (type1 == type2)
				return true;
			if (type1.IsEnum)
			{
				value2 = Enum.Parse(type1, value2.ToString(), true);
				return true;
			}

			var convertible1 = value1 as IConvertible;
			var convertible2 = value2 as IConvertible;
			if (convertible1 != null && convertible2 != null)
			{
				value2 = System.Convert.ChangeType(value2, type1, culture);
				return true;
			}
			return false;
		}
	}
	public sealed class SwitchCaseCollection : Collection<SwitchCase>
	{
		internal SwitchCaseCollection()
		{

		}
		public void Add(object when, object then)
		{
			Add(new SwitchCase
			{
				When = when,
				Then = then
			});
		}
	}
	[ContentProperty("Then")]
	public sealed class SwitchCase : DependencyObject
	{
		public static readonly DependencyProperty WhenProperty = DependencyProperty.Register("When", typeof(object), typeof(SwitchCase), new PropertyMetadata(default(object)));
		public static readonly DependencyProperty ThenProperty = DependencyProperty.Register("Then", typeof(object), typeof(SwitchCase), new PropertyMetadata(default(object)));

		public object When
		{
			get { return (object)GetValue(WhenProperty); }
			set { SetValue(WhenProperty, value); }
		}

		public object Then
		{
			get { return (object)GetValue(ThenProperty); }
			set { SetValue(ThenProperty, value); }
		}
	}
}

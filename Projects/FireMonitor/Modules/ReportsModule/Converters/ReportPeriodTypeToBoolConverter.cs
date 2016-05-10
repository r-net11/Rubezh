using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using StrazhAPI.SKD.ReportFilters;

namespace ReportsModule.Converters
{
	public class ReportPeriodTypeToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (ReportPeriodType)value == ReportPeriodType.Arbitrary;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

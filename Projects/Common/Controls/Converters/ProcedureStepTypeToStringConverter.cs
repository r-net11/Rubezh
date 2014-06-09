using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Automation;

namespace Controls.Converters
{
	public class ProcedureStepTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((ProcedureStepType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (ProcedureStepType)value;
		}
	}
}
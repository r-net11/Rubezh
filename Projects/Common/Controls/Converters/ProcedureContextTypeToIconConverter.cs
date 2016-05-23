using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using RubezhAPI.GK;
using RubezhAPI.Automation;

namespace Controls.Converters
{
	public class ProcedureContextTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ContextType)value)
			{
				case ContextType.Client:
					return "/Controls;component/Images/Procedure_client.png";
				case ContextType.Server:
					return "/Controls;component/Images/Procedure_server.png";
			}
			return "/Controls;component/Images/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
}
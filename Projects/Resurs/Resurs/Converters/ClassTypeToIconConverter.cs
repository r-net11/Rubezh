using ResursAPI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Resurs.Converters
{
	class ClassTypeToIconConverter : IMultiValueConverter ,IValueConverter
	{
		public object Convert(object [] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Any(x => x == DependencyProperty.UnsetValue))
				return null;
			if ((JournalType)values[1] != JournalType.System)
			{
				Uri uri;
				BitmapImage bitMap;
				switch ((ClassType)values[0])
				{
					case ClassType.IsUser:
						uri = new Uri("pack://application:,,,/Controls;component/Images/Employee.png");
						bitMap = new BitmapImage(uri);
						return bitMap;
					case ClassType.IsConsumer:
						uri = new Uri("pack://application:,,,/Controls;component/Images/AccessTemplate.png");
						bitMap = new BitmapImage(uri);
						return bitMap;
					case ClassType.IsDevice:
						uri = new Uri("pack://application:,,,/Controls;component/SKDIcons/System.png");
						bitMap = new BitmapImage(uri);
						return bitMap;
					default:
						return null;
				}
			}
			else
			{
				return null;
			}
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ClassType)value)
			{
				case ClassType.IsUser:
					return "/Controls;component/Images/Employee.png";
				case ClassType.IsConsumer:
					return "/Controls;component/Images/AccessTemplate.png";
				case ClassType.IsDevice:
					return "/Controls;component/SKDIcons/System.png";
					case ClassType.IsNone:
					return "/Controls;component/Images/PC.png";
				default:
					return null;
			}
		}
		public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
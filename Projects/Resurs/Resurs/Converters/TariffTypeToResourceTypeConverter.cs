using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Resurs.Converters
{
	class TariffTypeToResourceTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((TariffType)value)
			{
				case TariffType.ColdWater:
					return "м³";
					break;
				case TariffType.HotWater:
					return "м³";
					break;
				case TariffType.Gas:
					return "м³";
					break;
				case TariffType.Electricity:
					return "Квт*ч";
					break;
				case TariffType.Heat:
					return "GJ";
					break;
				default:
					throw new Exception();
					break;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

	}
}

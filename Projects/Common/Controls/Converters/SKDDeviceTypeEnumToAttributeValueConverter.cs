using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using StrazhAPI.SKD.Device;
using LocalizationConveters;

namespace Controls.Converters
{
	public class SKDDeviceTypeEnumToAttributeValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var deviceTypeLabel =
				//(SKDDeviceTypeLabelAttribute)value.GetType().GetMember(value.ToString())
                (LocalizedDeviceTypeLabel)value.GetType().GetMember(value.ToString())
					.First()
                //.GetCustomAttributes(typeof(SKDDeviceTypeLabelAttribute), false)
                    .GetCustomAttributes(typeof(LocalizedDeviceTypeLabel), false)
					.FirstOrDefault();

			if (deviceTypeLabel == null)
				return null;

			string res = null;
			switch (parameter.ToString().ToUpper())
			{
				case "TYPE":
					res = deviceTypeLabel.Type;
					break;
				case "LABEL":
					res = deviceTypeLabel.Label;
					break;
			}
			return res;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

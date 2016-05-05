using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using StrazhDeviceSDK.API;

namespace ControllerSDK.Converters
{
	class DeviceTypeEnumToAttributeValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var deviceTypeLabel =
				(DeviceTypeLabelAttribute)value.GetType().GetMember(value.ToString())
					.First()
					.GetCustomAttributes(typeof (DeviceTypeLabelAttribute), false)
					.FirstOrDefault();
			
			if (deviceTypeLabel == null)
				return null;
			
			string res = null;
			switch (parameter.ToString().ToLower())
			{
				case "type":
					res = deviceTypeLabel.Type;
					break;
				case "label":
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

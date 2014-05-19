using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XStateTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is XStateBit))
				return null;

			var stateType = (XStateBit)value;

			switch(stateType)
			{
				case XStateBit.Norm:
				case XStateBit.Save:

				case XStateBit.SetRegime_Automatic:
				case XStateBit.SetRegime_Manual:
				case XStateBit.SetRegime_Off:
					return null;
			}

			return "/Controls;component/StateClassIcons/" + stateType.ToString() + ".png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
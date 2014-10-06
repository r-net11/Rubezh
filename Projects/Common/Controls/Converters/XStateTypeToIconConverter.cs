using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XStateTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is GKStateBit))
				return null;

			var stateType = (GKStateBit)value;

			switch(stateType)
			{
				case GKStateBit.Norm:
				case GKStateBit.Save:

				case GKStateBit.SetRegime_Automatic:
				case GKStateBit.SetRegime_Manual:
				case GKStateBit.SetRegime_Off:
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
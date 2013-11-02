using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI.Models;

namespace DevicesModule.Converters
{
	public class ZoneLogicStateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//return Brushes.Red;

			ZoneLogicState? zoneLogicState = (ZoneLogicState?)value;
			if (zoneLogicState == null)
				return Brushes.White;

			switch(zoneLogicState.Value)
			{
				case ZoneLogicState.Fire:
					return Brushes.Red;

				case ZoneLogicState.Attention:
					return Brushes.Yellow;

				case ZoneLogicState.MPTAutomaticOn:
					return Brushes.DarkRed;

				case ZoneLogicState.MPTOn:
					return Brushes.DarkRed;

				case ZoneLogicState.Firefighting:
					return Brushes.DarkRed;

				case ZoneLogicState.DoubleFire:
					return Brushes.Pink;


				case ZoneLogicState.Alarm:
					return Brushes.Red;

				case ZoneLogicState.GuardSet:
					return Brushes.Blue;

				case ZoneLogicState.GuardUnSet:
					return Brushes.Blue;

				case ZoneLogicState.Lamp:
					return Brushes.Red;

				case ZoneLogicState.PCN:
					return Colors.Red;


				case ZoneLogicState.Failure:
					return Brushes.Green;

				case ZoneLogicState.AM1TOn:
					return Brushes.DarkRed;

				case ZoneLogicState.PumpStationAutomaticOff:
					return Brushes.Green;

				case ZoneLogicState.PumpStationOn:
					return Brushes.Green;

				case ZoneLogicState.ShuzOn:
					return Brushes.Green;
			}
			return Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
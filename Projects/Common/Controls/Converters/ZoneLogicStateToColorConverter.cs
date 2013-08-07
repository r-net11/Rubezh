using System;
using System.Windows.Data;
using System.Windows.Media;
using FiresecAPI.Models;

namespace Controls.Converters
{
	public class ZoneLogicStateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Colors.Green;

			ZoneLogicState? zoneLogicState = (ZoneLogicState?)value;
			if (zoneLogicState == null)
				return Colors.Green;

			switch (zoneLogicState.Value)
			{
				case ZoneLogicState.Fire:
					return Colors.Red;

				case ZoneLogicState.Attention:
					return Colors.Yellow;

				case ZoneLogicState.Alarm:
					return Colors.Red;



				case ZoneLogicState.AM1TOn:
					return Colors.Green;

				case ZoneLogicState.DoubleFire:
					return Colors.Green;

				case ZoneLogicState.Failure:
					return Colors.Green;

				case ZoneLogicState.Firefighting:
					return Colors.Green;

				case ZoneLogicState.GuardSet:
					return Colors.Green;

				case ZoneLogicState.GuardUnSet:
					return Colors.Green;

				case ZoneLogicState.Lamp:
					return Colors.Green;

				case ZoneLogicState.MPTAutomaticOn:
					return Colors.Green;

				case ZoneLogicState.MPTOn:
					return Colors.Green;

				case ZoneLogicState.PCN:
					return Colors.Green;

				case ZoneLogicState.PumpStationAutomaticOff:
					return Colors.Green;

				case ZoneLogicState.PumpStationOn:
					return Colors.Green;

				case ZoneLogicState.ShuzOn:
					return Colors.Green;
			}
			return Colors.Green;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
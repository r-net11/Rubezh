using System.Windows.Data;
using Localization.SKD.Common;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	public class CardTypeToLongStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch((CardType)value)
			{
				case CardType.Constant:
					return CommonResources.UnlimitedPasscard;
				case CardType.Temporary:
			        return CommonResources.LimitedPasscard;
				case CardType.Guest:
					return CommonResources.BoundedPasscard;
				case CardType.Blocked:
					return CommonResources.AccessDeniedPasscard;
				case CardType.Duress:
					return CommonResources.ForcedAlarmPasscard;

			}
			return null;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}
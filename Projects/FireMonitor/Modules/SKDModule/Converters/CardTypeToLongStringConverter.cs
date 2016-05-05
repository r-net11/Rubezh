using System.Windows.Data;
using FiresecAPI.SKD;

namespace SKDModule.Converters
{
	public class CardTypeToLongStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch((CardType)value)
			{
				case CardType.Constant:
					return "Пропуск с неограниченным сроком действия";
				case CardType.Temporary:
					return "Пропуск с ограниченным сроком действия";
				case CardType.Guest:
					return "Пропуск, позволяющий выполнить не более заданного количества проходов";
				case CardType.Blocked:
					return "Пропуск, доступ по которому запрещен";
				case CardType.Duress:
					return "Использование пропуска вызывает тревогу по принуждению";

			}
			return null;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}
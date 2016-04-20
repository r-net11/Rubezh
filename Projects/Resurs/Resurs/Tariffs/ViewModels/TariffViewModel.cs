using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;

namespace Resurs.ViewModels
{
	public class TariffViewModel : BaseViewModel
	{
		public TariffViewModel (Tariff tariff)
		{
			Tariff = tariff;
		}

		Tariff _tariff;
		public Tariff Tariff
		{
			get { return _tariff; }
			set
			{
				_tariff = value;
				OnPropertyChanged(() => Tariff);
			}
		}
	}
}
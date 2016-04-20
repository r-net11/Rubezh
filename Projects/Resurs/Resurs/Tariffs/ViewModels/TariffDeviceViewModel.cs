using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;

namespace Resurs.ViewModels
{
	public class TariffDeviceViewModel : BaseViewModel
	{
		public TariffDeviceViewModel(Device device)
		{
			Device = device;
		}
		public Device Device { get; private set; }

		bool _isChecked;
		public bool IsChecked { get { return _isChecked; }
			set { 
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		private bool _hasTariff;
		public bool HasTariff
		{
			get { return _hasTariff; }
			set 
			{ 
				_hasTariff = value;
				OnPropertyChanged(() => HasTariff);
			}
		}
	}
}

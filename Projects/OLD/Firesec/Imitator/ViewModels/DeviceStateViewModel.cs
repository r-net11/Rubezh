using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Firesec.Imitator.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public DeviceStateViewModel(DriverState driverState)
		{
			DriverState = driverState;
		}

		public DriverState DriverState { get; private set; }

		public bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(() => IsActive);
					ImitatorViewModel.Current.Update();
				}
			}
		}
	}
}
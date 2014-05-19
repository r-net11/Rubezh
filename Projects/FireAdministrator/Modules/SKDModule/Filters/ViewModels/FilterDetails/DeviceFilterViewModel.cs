using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DeviceFilterViewModel : BaseViewModel
	{
		public DeviceFilterViewModel(SKDDevice device)
		{
			Device = device;
		}

		public SKDDevice Device { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
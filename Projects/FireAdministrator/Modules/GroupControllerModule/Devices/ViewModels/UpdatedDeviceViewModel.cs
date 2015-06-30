using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class UpdatedDeviceViewModel : BaseViewModel
	{
		public string Name { get; private set; }
		public string Address { get; private set; }
		public GKDevice Device;
		public string ImageSource { get; private set; }
		public UpdatedDeviceViewModel(GKDevice device)
		{
			Device = device;
			Name = device.ShortName;
			Address = device.DottedPresentationAddress;
			ImageSource = device.Driver.ImageSource;
		}

		bool isChecked;
		public bool IsChecked
		{
			get
			{
				return isChecked;
			}
			set
			{
				isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}
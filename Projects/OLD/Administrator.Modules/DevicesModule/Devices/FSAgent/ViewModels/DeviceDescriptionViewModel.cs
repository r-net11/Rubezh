using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceDescriptionViewModel : DialogViewModel
	{
		public DeviceDescriptionViewModel(Device device, string description)
		{
			Title = "Описание устройства " + device.PresentationAddressAndName;
			CloseCommand = new RelayCommand(OnClose);
			Description = description;
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			Close(true);
		}
	}
}
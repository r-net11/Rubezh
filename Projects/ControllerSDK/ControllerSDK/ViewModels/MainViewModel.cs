using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriver;
using System.Windows;
using Infrastructure.Common;

namespace ControllerSDK.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public static Wrapper Wrapper { get; set; }

		public CardsViewModel CardsViewModel { get; private set; }
		public CardRecsViewModel CardRecsViewModel { get; private set; }
		public PasswordsViewModel PasswordsViewModel { get; private set; }
		public HolidaysViewModel HolidaysViewModel { get; private set; }
		public TimeShedulesViewModel TimeShedulesViewModel { get; private set; }
		public ControlViewModel ControlViewModel { get; private set; }
		public JournalViewModel JournalViewModel { get; private set; }

		public MainViewModel()
		{
			Wrapper = new Wrapper();

			CardsViewModel = new CardsViewModel();
			CardRecsViewModel = new CardRecsViewModel();
			PasswordsViewModel = new PasswordsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			TimeShedulesViewModel = new TimeShedulesViewModel();
			ControlViewModel = new ControlViewModel();
			JournalViewModel = new JournalViewModel();

			GetDeviceSoftwareInfoCommand = new RelayCommand(OnGetDeviceSoftwareInfo);
			GetDeviceNetInfoCommand = new RelayCommand(OnGetDeviceNetInfo);
		}

		public RelayCommand GetDeviceSoftwareInfoCommand { get; private set; }
		void OnGetDeviceSoftwareInfo()
		{
			string text;
			var deviceSoftwareInfo = MainViewModel.Wrapper.GetDeviceSoftwareInfo();
			if (deviceSoftwareInfo != null)
			{
				text = "SoftwareBuildDate = " + deviceSoftwareInfo.SoftwareBuildDate.ToString() + "\n";
				text += "DeviceType = " + deviceSoftwareInfo.DeviceType + "\n";
				text += "SoftwareBuildDate = " + deviceSoftwareInfo.SoftwareBuildDate + "\n";
			}
			else
			{
				text = "Error" + "\n";
			}
			MessageBox.Show(text);
		}

		public RelayCommand GetDeviceNetInfoCommand { get; private set; }
		void OnGetDeviceNetInfo()
		{
			string text;
			var deviceNetInfo = MainViewModel.Wrapper.GetDeviceNetInfo();
			if (deviceNetInfo != null)
			{
				text = "IP = " + deviceNetInfo.IP + "\n";
				text += "SubnetMask = " + deviceNetInfo.SubnetMask + "\n";
				text += "DefaultGateway = " + deviceNetInfo.DefaultGateway + "\n";
				text += "MTU = " + deviceNetInfo.MTU.ToString() + "\n";
			}
			else
			{
				text = "Error" + "\n";
			}
			MessageBox.Show(text);
		}
	}
}
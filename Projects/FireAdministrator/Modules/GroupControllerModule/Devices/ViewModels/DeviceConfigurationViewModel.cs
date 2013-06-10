using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		XDevice KauDevice;
		List<XDevice> ChildDevices;

		public DeviceConfigurationViewModel(XDevice kauDevice, List<XDevice> devices)
		{
			Title = "Конфигурация устройств";
			KauDevice = kauDevice;
			ChildDevices = devices;
			Devices = new ObservableCollection<XDevice>(devices);
			ChangeCommand = new RelayCommand(OnChange);
		}

		public ObservableCollection<XDevice> Devices { get; private set; }

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			ChildDevices.RemoveAll(x => x.Driver.IsKauOrRSR2Kau);
			KauDevice.Children = new List<XDevice>();
			KauDevice.Children.AddRange(ChildDevices);
			ServiceFactory.SaveService.GKChanged = true;
			Close(true);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		public DeviceConfigurationViewModel(List<XDevice> devices)
		{
			Title = "Конфигурация устройств";
			Devices = new ObservableCollection<XDevice>(devices);
		}

		public ObservableCollection<XDevice> Devices { get; private set; }
	}
}
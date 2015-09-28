using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public DeviceViewModel(Device device)
		{
			Update(device);
		}

		public ObservableCollection<ParameterViewModel> Parameters { get; private set; }

		Device _device;
		public Device Device
		{
			get { return _device; }
			set
			{
				_device = value;
				OnPropertyChanged(() => Device);
			}
		}

		public void Update(Device device)
		{
			Device = device;
			Parameters = new ObservableCollection<ParameterViewModel>(Device.Parameters.Select(x => new ParameterViewModel(x)));
		}
	}
}
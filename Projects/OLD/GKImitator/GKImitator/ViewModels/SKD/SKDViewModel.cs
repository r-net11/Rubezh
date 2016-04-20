using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI;
using XFiresecAPI;

namespace GKImitator.ViewModels
{
	public class SKDViewModel : BaseViewModel
	{
		public SKDViewModel()
		{
			Controllers = new ObservableCollection<ControllerViewModel>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
				{
					var skdDeviceViewModel = new ControllerViewModel(device);
					Controllers.Add(skdDeviceViewModel);
				}
			}
			SelectedController = Controllers.FirstOrDefault();
		}

		public ObservableCollection<ControllerViewModel> Controllers { get; private set; }

		ControllerViewModel _selectedController;
		public ControllerViewModel SelectedController
		{
			get { return _selectedController; }
			set
			{
				_selectedController = value;
				OnPropertyChanged("SelectedController");
			}
		}
	}
}
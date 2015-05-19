using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using System.Collections.ObjectModel;

namespace PowerCalculator.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel()
		{
			Title = "Новое устройство";
			DeviceTypes = new ObservableCollection<DeviceTypeViewModel>();
			DeviceTypes.Add(new DeviceTypeViewModel(AlsDeviceType.SmokeDetector));
			DeviceTypes.Add(new DeviceTypeViewModel(AlsDeviceType.HeatDetector));
			SelectedDeviceType = DeviceTypes.LastOrDefault();
		}

		public ObservableCollection<DeviceTypeViewModel> DeviceTypes { get; private set; }

		DeviceTypeViewModel _selectedDeviceType;
		public DeviceTypeViewModel SelectedDeviceType
		{
			get { return _selectedDeviceType; }
			set
			{
				_selectedDeviceType = value;
				OnPropertyChanged(() => SelectedDeviceType);
			}
		}

		protected override bool CanSave()
		{
			return true;
		}
	}
}
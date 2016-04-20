using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using MonitorTestClientFS2.ViewModels;
using ServerFS2;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel
	{
		public Device Device { get; private set; }
		public DeviceState DeviceState { get; private set; }

		public DeviceDetailsViewModel(Device device)
		{
			ExecuteCommand = new RelayCommand(OnExecute);
			DeviceCommands = new List<DeviceCommandViewModel>();
			Device = device;
			DeviceState = Device.DeviceState;
			DeviceState.StateChanged += new Action(OnStateChanged);
			DeviceState.ParametersChanged += new Action(OnParametersChanged);
			OnStateChanged();

			Title = Device.DottedPresentationAddressAndName;
			TopMost = true;

			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			var metadataDeviceCommands = MetadataHelper.Metadata.devicePropInfos.Where(x => x.tableType == tableNo);
			foreach (var metadataDeviceCommand in metadataDeviceCommands)
			{
				var deviceCommandViewModel = new DeviceCommandViewModel()
				{
					Name = metadataDeviceCommand.name,
					Caption = metadataDeviceCommand.caption,
				};
				DeviceCommands.Add(deviceCommandViewModel);
			}
			SelectedDeviceCommand = DeviceCommands.FirstOrDefault();
		}

		public List<DeviceCommandViewModel> DeviceCommands { get; private set; }

		DeviceCommandViewModel _selectedDeviceCommand;
		public DeviceCommandViewModel SelectedDeviceCommand
		{
			get { return _selectedDeviceCommand; }
			set
			{
				_selectedDeviceCommand = value;
				OnPropertyChanged("SelectedDeviceCommand");
			}
		}

		public RelayCommand ExecuteCommand { get; private set; }
		void OnExecute()
		{
			if (SelectedDeviceCommand != null)
			{
				ServerHelper.ExecuteCommand(Device, SelectedDeviceCommand.Name);
			}
		}

		void OnStateChanged()
		{
		}

		void OnParametersChanged()
		{
		}
	}
}
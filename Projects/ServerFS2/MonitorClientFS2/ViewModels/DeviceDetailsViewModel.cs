using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using ServerFS2;
using Infrastructure.Common;
using ServerFS2.Processor;

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
					Command1 = metadataDeviceCommand.command1,
					ShiftInMemory = metadataDeviceCommand.shiftInMemory,
					CommandDev = metadataDeviceCommand.commandDev,
					MaskCmdDev = metadataDeviceCommand.maskCmdDev
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
				MainManager.AddCommand(Device, SelectedDeviceCommand.Name);
				//ServerHelper.ExecuteCommand(Device, SelectedDeviceCommand.Name);
			}
		}

		void OnStateChanged()
		{
		}

		void OnParametersChanged()
		{
		}

		public class DeviceCommandViewModel
		{
			public string Name { get; set; }
			public string Caption { get; set; }
			public string Command1 { get; set; }
			public string ShiftInMemory { get; set; }
			public string CommandDev { get; set; }
			public string MaskCmdDev { get; set; }
		}
	}
}
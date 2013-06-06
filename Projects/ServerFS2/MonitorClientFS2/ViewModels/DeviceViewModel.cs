using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.TreeList;
using Infrastructure.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ServerFS2.Processor;
using Infrastructure.Common.Windows;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }

		public DeviceViewModel(Device device)
		{
			ResetFireCommand = new RelayCommand(OnResetFire);
			ResetCommand = new RelayCommand(OnReset);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Device = device;
			device.DeviceState.StateChanged += new System.Action(DeviceState_StateChanged);
		}

		void DeviceState_StateChanged()
		{
			Trace.WriteLine("DeviceState_StateChanged");
			OnPropertyChanged("Device");
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("States");
		}

		public DeviceState DeviceState
		{
			get { return Device.DeviceState; }
		}

		public List<DeviceDriverState> States
		{
			get { return Device.DeviceState.States; }
		}

		public string UsbChannel
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "UsbChannel");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string SerialNo
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string Version
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "Version");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string Address
		{
			get { return Device.PresentationAddress; }
		}

		public Driver Driver
		{
			get { return Device.Driver; }
		}

		public int ShleifNo
		{
			get { return Device.IntAddress / 256; }
		}

		public int AddressOnShleif
		{
			get { return Device.IntAddress % 256; }
		}

		public string ToolTip
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Device.PresentationAddressAndName);

				if (DeviceState.ParentStringStates != null)
				{
					foreach (var parentState in DeviceState.ParentStringStates)
					{
						stringBuilder.AppendLine(parentState);
					}
				}

				foreach (var state in DeviceState.ThreadSafeStates)
				{
					stringBuilder.AppendLine(state.DriverState.Name);
				}

				foreach (var parameter in DeviceState.ThreadSafeParameters)
				{
					if (!parameter.IsIgnore && parameter.Visible && parameter.Value != "NAN")
					{
						stringBuilder.Append(parameter.Caption);
						stringBuilder.Append(" - ");
						stringBuilder.AppendLine(parameter.Value);
					}
				}

				var result = stringBuilder.ToString();
				if (result.EndsWith("\r\n"))
					result = result.Remove(result.Length - 2);
				return result;
			}
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			MainManager.ResetFire(Device);
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			var status = MainManager.GetDeviceStatus(Device);
			if (status == null)
				return;
			MainManager.ResetTest(Device, status);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			MainManager.AddDeviceToCheckList(Device);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			MainManager.RemoveDeviceFromCheckList(Device);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
	}
}
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.TreeList;
using Infrastructure.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ServerFS2.Processor;
using Infrastructure.Common.Windows;
using System.Collections;
using ServerFS2.ConfigurationWriter;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }

		public DeviceViewModel(Device device)
		{
			ResetFireCommand = new RelayCommand(OnResetFire);
			ResetCommand = new RelayCommand<string>(OnReset, CanReset);
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

		public RelayCommand<string> ResetCommand { get; private set; }
		void OnReset(string stateName)
		{
			var resetItems = new List<ResetItem>();
			var resetItem = new ResetItem()
			{
				DeviceState = DeviceState
			};
			var deviceDriverState = DeviceState.ThreadSafeStates.FirstOrDefault(x => x.DriverState.Name == stateName);
			resetItem.States.Add(deviceDriverState);
			resetItems.Add(resetItem);
		}
		void OnResetOld(string stateName)
		{
			var statusBytes = MainManager.GetDeviceStatus(Device);
			if (statusBytes == null)
				return;

			var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };

			Trace.WriteLine("statusBytesArray = " + BytesHelper.BytesToString(statusBytesArray.ToList()));
			var bitArray = new BitArray(statusBytesArray);
			var hasTest = bitArray[17];
			Trace.WriteLine("hasTest = " + hasTest);
			for (int i = 0; i < bitArray.Count; i++)
			{
				var hasBit = bitArray[i] ? "1" : "0";
				Trace.WriteLine("bitArray[] " + i + "\t" + hasBit);
			}

			MainManager.ResetPanelBit(Device, statusBytes, 17);
		}
		bool CanReset(string stateName)
		{
			return DeviceState.ThreadSafeStates.Any(x => (x.DriverState.Name == stateName && x.DriverState.IsManualReset));
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
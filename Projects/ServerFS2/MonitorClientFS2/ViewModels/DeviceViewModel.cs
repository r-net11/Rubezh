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
using ServerFS2;
using FS2Api;
using ServerFS2.Monitor;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }

		public DeviceViewModel(Device device)
		{
			ResetCommand = new RelayCommand<DriverState>(OnReset, CanReset);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Device = device;
			device.DeviceState.StateChanged += new System.Action(DeviceState_StateChanged);
		}

		void DeviceState_StateChanged()
		{
			OnPropertyChanged("Device");
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("States");
			OnPropertyChanged("ToolTip");
		}

		public DeviceState DeviceState
		{
			get { return Device.DeviceState; }
		}

		public List<DeviceDriverState> States
		{
			get { return Device.DeviceState.States; }
		}

		public Driver Driver
		{
			get { return Device.Driver; }
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
					if (state.DriverState != null)
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

		public RelayCommand<DriverState> ResetCommand { get; private set; }
		void OnReset(DriverState driverState)
		{
			var panelResetItems = new List<PanelResetItem>();
			var panelResetItem = new PanelResetItem()
			{
				PanelUID = DeviceState.Device.UID
			};
			panelResetItem.Ids.Add(driverState.Code);
			panelResetItems.Add(panelResetItem);
			MainManager.ResetStates(panelResetItems);
		}
		bool CanReset(DriverState state)
		{
			return DeviceState.ThreadSafeStates.Any(x => (x.DriverState != null && x.DriverState == state && x.DriverState.IsManualReset));
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			MonitoringProcessor.AddTaskIgnore(Device);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			MonitoringProcessor.AddTaskResetIgnore(Device);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
	}
}
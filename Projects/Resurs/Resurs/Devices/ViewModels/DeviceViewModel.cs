﻿using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
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

		public List<ParameterViewModel> Parameters { get; private set; }

		public Device Device { get; private set;}
		public DeviceState State { get; private set; }
		public bool IsActive { get; private set; }

		public void Update()
		{
			var device = DBCash.GetDevice(Device.UID);
			if (device != null)
				Update(device);
		}

		public void Update(Device device)
		{
			Device = device;
			OnPropertyChanged(() => Device);
			Parameters = new List<ParameterViewModel>(Device.Parameters.Select(x => new ParameterViewModel(x)));
			OnPropertyChanged(() => Parameters);
			IsActive = device.IsActive || device.Parent == null;
			if (IsActive)
				State = DeviceState.Norm;
			else
				State = DeviceState.Disabled;
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => IsActive);
		}
	}
}
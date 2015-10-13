using Infrastructure.Common;
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
		public string FullAddress { get; private set; }
		public bool IsActive { get; private set; }

		public void Load()
		{
			var children = Device.Children;
			var device = DBCash.GetDeivce(Device.UID);
			if (device != null)
				Update(device);
			Device.Children = children;
		}

		public void Update()
		{
			OnPropertyChanged(() => Device);
			IsActive = Device.IsActive || Device.Parent == null;
			if (IsActive)
				State = DeviceState.Norm;
			else
				State = DeviceState.Disabled;
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => IsActive);
			OnPropertyChanged(() => Parameters);
			FullAddress = Device.FullAddress;
			OnPropertyChanged(() => FullAddress);
		}

		public void Update(Device device)
		{
			Device = device;
			FullAddress = device.FullAddress;
			OnPropertyChanged(() => FullAddress);
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
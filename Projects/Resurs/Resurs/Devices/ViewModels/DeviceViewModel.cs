using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.TreeList;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using ResursNetwork.Networks;
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
			Errors = new List<DeviceError>();
			Update(device);
		}

		public List<ParameterViewModel> Parameters { get; private set; }

		public Device Device { get; private set;}
		public DeviceState State 
		{
			get
			{
				if (!IsActive)
					return DeviceState.Disabled;
				if (Errors.Any(x => x == DeviceError.Communication))
					return DeviceState.ConnectionLost;
				if (Errors.Any())
					return DeviceState.Error;
				return DeviceState.Norm;
			}
		}
		List<DeviceError> _errors;
		public List<DeviceError> Errors 
		{
			get { return _errors; }
			set
			{
				_errors = value;
				OnPropertyChanged(() => Errors);
				OnPropertyChanged(() => State);
			}
		}
		public bool IsActive { get; private set; }

		public void Load()
		{
			var children = Device.Children;
			var device = DbCache.GetDevice(Device.UID);
			if (device != null)
				Update(device);
			Device.Children = children;
		}

		public void Update()
		{
			OnPropertyChanged(() => Device);
			IsActive = Device.IsActive || Device.Parent == null;
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => IsActive);
			OnPropertyChanged(() => Parameters);
		}

		public void Update(Device device)
		{
			Device = device;
			OnPropertyChanged(() => Device);
			Parameters = new List<ParameterViewModel>(Device.Parameters.Select(x => new ParameterViewModel(x)));
			OnPropertyChanged(() => Parameters);
			IsActive = device.IsActive || device.ParentUID == null;
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => IsActive);
		}
	}
}
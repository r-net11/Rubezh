using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public partial class MPTDeviceViewModel : BaseViewModel
	{
		public MPTDevice MPTDevice { get; private set; }
		public XDevice Device { get; private set; }

		public MPTDeviceViewModel(MPTDevice mptDevice)
		{
			MPTDevice = mptDevice;
			Device = mptDevice.Device;
			AvailableMPTDeviceTypes = new ObservableCollection<MPTDeviceType>(MPTDevice.GetAvailableMPTDeviceTypes(MPTDevice.Device.DriverType));
			MPTDeviceType = MPTDevice.MPTDeviceType;
		}

		public string PresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				return XManager.GetPresentationZone(Device);
			}
		}

		public ObservableCollection<MPTDeviceType> AvailableMPTDeviceTypes { get; private set; }

		MPTDeviceType _mptDeviceType;
		public MPTDeviceType MPTDeviceType
		{
			get { return _mptDeviceType; }
			set
			{
				_mptDeviceType = value;
				OnPropertyChanged("MPTDeviceType");

				MPTDevice.MPTDeviceType = value;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}
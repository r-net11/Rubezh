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
			if (HasHoldOrDelay)
			{
				Delay = Delay;
				Hold = Hold;
			}
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

		public MPTDeviceType MPTDeviceType
		{
			get { return MPTDevice.MPTDeviceType; }
			set
			{
				MPTDevice.MPTDeviceType = value;
				OnPropertyChanged("MPTDeviceType");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				Device.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool HasHoldOrDelay
		{
			get { return Device.DriverType != XDriverType.RSR2_AM_1; }
		}

		public int Delay
		{
			get { return MPTDevice.Delay; }
			set
			{
				MPTDevice.Delay = value;
				OnPropertyChanged("Delay");
				SetDeviceProperty("Задержка на включение, с", value);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public int Hold
		{
			get { return MPTDevice.Hold; }
			set
			{
				MPTDevice.Hold = value;
				OnPropertyChanged("Hold");
				SetDeviceProperty("Время удержания, с", value);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		void SetDeviceProperty(string propertyName, int value)
		{
			var property = Device.Properties.FirstOrDefault(x => x.Name == propertyName);
			if (property == null)
			{
				property = new XProperty()
				{
					Name = propertyName,
					DriverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == propertyName)
				};
				Device.Properties.Add(property);
			}
			property.Value = (ushort)value;
			Device.OnChanged();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
	public class InformationViewModel : DialogViewModel
	{
		public InformationViewModel(Device device)
		{
			Title = "Информация об устройстве";
			selectedDeivce = device;
			
			var tempProperties = new List<Property>();
			tempProperties.Add(device.Properties.FirstOrDefault(x => x.Name == "DriverType"));
			tempProperties.Add(device.Properties.FirstOrDefault(x => x.Name == "SerialNo"));
			tempProperties.Add(device.Properties.FirstOrDefault(x => x.Name == "SoftVersion"));
			if ((device.Driver.DriverType != DriverType.MS_1) && (device.Driver.DriverType != DriverType.MS_2))
				tempProperties.Add(device.Properties.FirstOrDefault(x => x.Name == "BDVersion"));
			Properties = tempProperties;
		}
		private Device selectedDeivce;
		private List<Property> properties;
		public List<Property> Properties
		{
			get { return properties; }
			set
			{
				properties = value;
				OnPropertyChanged("Properties");
			}
		}
	}
}

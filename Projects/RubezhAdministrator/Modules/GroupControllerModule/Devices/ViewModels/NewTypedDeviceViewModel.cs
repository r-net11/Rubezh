using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.Devices.ViewModels
{
	public class NewTypedDeviceViewModel : TreeNodeViewModel<NewTypedDeviceViewModel>
	{
		public NewTypedDeviceViewModel(GKDriver.DriverClassifications driverClassification)
		{
			ShortName = driverClassification.ToDescription();
			Name = "";
			ImageSource = "/Controls;component/Images/Blank.png";
			DriverClassification = driverClassification;
		}
		public NewTypedDeviceViewModel(GKDriver driver)
		{
			Name = driver.Name;
			ShortName = driver.ShortName;
			Driver = driver;
			ImageSource = driver.ImageSource;
			DriverClassification = driver.DriverClassification;
		}

		public string Name { get; private set; }
		public string ShortName { get; private set; }
		public GKDriver Driver { get; set; }
		public string ImageSource { get; private set; }
		public GKDriver.DriverClassifications DriverClassification {get;private set;}
	}
}

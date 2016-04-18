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
	public class NewTypedDeviceViewModel: TreeNodeViewModel<NewTypedDeviceViewModel> 
	{
		public NewTypedDeviceViewModel(GKDriver.TypesOfBranches typeOfBranche)
		{
			ShortName = typeOfBranche.ToDescription();
			Name = "";
			IsSelected = false;

		}
		public NewTypedDeviceViewModel(GKDriver driver)
		{
			Name = driver.Name;
			ShortName = driver.ShortName;
			Driver = driver;
			ImageSource = driver.ImageSource;
			IsSelected = true;

		}

		public string Name { get; private set; }
		public string ShortName { get; private set; }
		public GKDriver Driver { get; set; }
		public string ImageSource { get; private set; }

		public bool IsSelected { get; set; }

	}
}

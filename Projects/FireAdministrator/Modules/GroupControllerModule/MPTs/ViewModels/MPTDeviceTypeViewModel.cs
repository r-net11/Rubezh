using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class MPTDeviceTypeViewModel : BaseViewModel
	{
		public MPTDeviceTypeViewModel(MPTDeviceType mptDeviceType)
		{
			MPTDeviceType = mptDeviceType;
		}

		public MPTDeviceType MPTDeviceType { get; private set; }
	}
}
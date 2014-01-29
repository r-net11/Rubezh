using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class SubsystemTypeViewModel : CheckBoxItemViewModel
	{
		public SubsystemTypeViewModel(XSubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
		}

		public XSubsystemType SubsystemType { get; private set; }
	}
}
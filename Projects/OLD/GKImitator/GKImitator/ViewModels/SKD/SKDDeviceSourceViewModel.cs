using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKImitator.ViewModels
{
	public class SKDDeviceSourceViewModel : BaseViewModel
	{
		public SKDDeviceSourceViewModel(int no, string name)
		{
			No = no;
			Name = name;
		}

		public string Name { get; private set; }
		public int No { get; private set; }
	}
}
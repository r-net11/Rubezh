using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ParameterViewModel : BaseViewModel
	{
		public string Name { get; set; }
		public string Caption { get; set; }
		public string SystemValue { get; set; }
		public string DeviceValue { get; set; }
	}
}
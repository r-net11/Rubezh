using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace MonitorClientFS2.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public string DeviceName { get; set; }
		public DriverState DriverState { get; set; }
	}
}
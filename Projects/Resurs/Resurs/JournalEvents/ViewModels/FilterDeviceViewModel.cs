using Infrastructure.Common.TreeList;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class FilterDeviceViewModel : TreeNodeViewModel<FilterDeviceViewModel>
	{
		public Device Device { get; set; }
		public FilterDeviceViewModel(Device device)
		{
			Device = device;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Client.Plans.ViewModels;

namespace SKDModule.Plans.ViewModels
{
	public class DeviceTooltipViewModel : ImageTextTooltipViewModel
	{
		private string _parentImageSource;
		public string ParentImageSource
		{
			get { return _parentImageSource; }
			set
			{
				_parentImageSource = value;
				OnPropertyChanged(() => ParentImageSource);
			}
		}
		private string _parentTitle;
		public string ParentTitle
		{
			get { return _parentTitle; }
			set
			{
				_parentTitle = value;
				OnPropertyChanged(() => ParentTitle);
			}
		}
		private bool _isDeviceExists;
		public bool IsDeviceExists
		{
			get { return _isDeviceExists; }
			set
			{
				_isDeviceExists = value;
				OnPropertyChanged(() => IsDeviceExists);
			}
		}
	}
}
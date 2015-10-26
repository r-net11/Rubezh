using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{

	public class TariffDeviceViewModel : BaseViewModel
	{
		public TariffDeviceViewModel(Device device)
		{
			Device = device;
		}

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
	
			set { 
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		Device _device;
		public Device Device
		{
			get { return _device; }
			set
			{
				_device = value;
			}
		}

		private bool _hasTariff;

		public bool HasTariff
		{
			get { return _hasTariff; }
			set { 
				_hasTariff = value;
				OnPropertyChanged(() => HasTariff);
			}
		}

	}
}

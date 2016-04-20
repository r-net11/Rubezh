using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class AutoSearchDeviceViewModel : BaseViewModel
	{
		public Device Device { get; private set; }

		public AutoSearchDeviceViewModel(Device device)
		{
			Device = device;
			Children = new List<AutoSearchDeviceViewModel>();
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(() => IsSelected);

				Children.ForEach(x => x.IsSelected = value);
			}
		}

		public bool CanSelect
		{
			get { return !FiresecManager.Devices.Any(x => x.PathId == Device.PathId); }
		}

		public string Name
		{
			get
			{
				if (Device.Driver.HasAddress && Device.Parent != null && Device.Parent.Driver.DriverType != DriverType.Computer)
					return Device.PresentationAddressAndName;

				var serialNo = "";
				var property = Device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (property != null)
				{
					if (property.Value != null)
						serialNo = property.Value;
				}
				return Device.Driver.Name + " " + serialNo;
			}
		}

		public List<AutoSearchDeviceViewModel> Children { get; set; }
	}
}
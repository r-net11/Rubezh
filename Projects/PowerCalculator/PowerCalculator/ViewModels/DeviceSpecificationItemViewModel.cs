using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;

namespace PowerCalculator.ViewModels
{
	public class DeviceSpecificationItemViewModel : BaseViewModel
	{
		public DeviceSpecificationItem DeviceSpecificationItem { get; private set; }

		public DeviceSpecificationItemViewModel(DeviceSpecificationItem deviceSpecificationItem)
		{
			DeviceSpecificationItem = deviceSpecificationItem;
            Drivers = new ObservableCollection<DriverViewModel>(DriversHelper.Drivers.Where(x=>x.CanAdd).Select(x => new DriverViewModel(x)));
			_selectedDriver = Drivers.FirstOrDefault(x => x.Driver.DriverType == deviceSpecificationItem.DriverType);
			_count = deviceSpecificationItem.Count;
		}

		public ObservableCollection<DriverViewModel> Drivers { get; private set; }

		DriverViewModel _selectedDriver;
		public DriverViewModel SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
				DeviceSpecificationItem.DriverType = value.Driver.DriverType;
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value > 0 ? value : 1;
				OnPropertyChanged(() => Count);
				DeviceSpecificationItem.Count = _count;
			}
		}
	}
}
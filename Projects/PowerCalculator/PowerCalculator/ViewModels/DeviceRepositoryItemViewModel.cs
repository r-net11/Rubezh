using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;

namespace PowerCalculator.ViewModels
{
	public class DeviceRepositoryItemViewModel : BaseViewModel
	{
		public DeviceRepositoryItem DeviceRepositoryItem { get; private set; }

		public DeviceRepositoryItemViewModel(DeviceRepositoryItem deviceRepositoryItem)
		{
			DeviceRepositoryItem = deviceRepositoryItem;

			Drivers = new ObservableCollection<DriverViewModel>(DriversHelper.Drivers.Select(x => new DriverViewModel(x)));
			_selectedDriver = Drivers.FirstOrDefault(x => x.Driver.DriverType == deviceRepositoryItem.DriverType);
			_count = deviceRepositoryItem.Count;
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
				DeviceRepositoryItem.DriverType = value.Driver.DriverType;
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
				DeviceRepositoryItem.Count = value;
			}
		}
	}
}
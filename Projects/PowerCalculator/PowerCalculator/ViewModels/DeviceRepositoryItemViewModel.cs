using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DeviceRepositoryItemViewModel : BaseViewModel
	{
		public DeviceRepositoryItem DeviceRepositoryItem { get; private set; }

		public DeviceRepositoryItemViewModel(DeviceRepositoryItem deviceRepositoryItem)
		{
			DeviceRepositoryItem = deviceRepositoryItem;

			DriverTypes = new ObservableCollection<DriverTypeViewModel>(Enum.GetValues(typeof(DriverType)).Cast<DriverType>().Select(x => new DriverTypeViewModel(x)));
			_selectedDriverType = DriverTypes.FirstOrDefault(x => x.DriverType == deviceRepositoryItem.DriverType);
			_count = deviceRepositoryItem.Count;
		}

		public ObservableCollection<DriverTypeViewModel> DriverTypes { get; private set; }

		DriverTypeViewModel _selectedDriverType;
		public DriverTypeViewModel SelectedDriverType
		{
			get { return _selectedDriverType; }
			set
			{
				_selectedDriverType = value;
				OnPropertyChanged(() => SelectedDriverType);
				DeviceRepositoryItem.DriverType = value.DriverType;
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
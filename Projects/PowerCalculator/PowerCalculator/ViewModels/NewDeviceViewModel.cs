using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel()
		{
			Title = "Добавление нового устройства";
			DeviceTypes = new ObservableCollection<DriverTypeViewModel>(Enum.GetValues(typeof(DriverType)).Cast<DriverType>().Select(x => new DriverTypeViewModel(x)));
			SelectedDeviceType = DeviceTypes.FirstOrDefault();
			Count = 1;
		}

		public ObservableCollection<DriverTypeViewModel> DeviceTypes { get; private set; }

		DriverTypeViewModel _selectedDeviceType;
		public DriverTypeViewModel SelectedDeviceType
		{
			get { return _selectedDeviceType; }
			set
			{
				_selectedDeviceType = value;
				OnPropertyChanged(() => SelectedDeviceType);
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
			}
		}
	}
}
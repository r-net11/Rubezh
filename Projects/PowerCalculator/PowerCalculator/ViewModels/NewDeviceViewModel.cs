using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;

namespace PowerCalculator.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel()
		{
			Title = "Добавление нового устройства";
			Drivers = new ObservableCollection<Driver>(DriversHelper.Drivers.Where(x=>x.CanAdd));
			SelectedDriver = Drivers.FirstOrDefault();
			CableResistivity = 1;
			CableLenght = 1;
			Count = 1;
		}

		public ObservableCollection<Driver> Drivers { get; private set; }

		Driver _selectedDriver;
		public Driver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
			}
		}

		double _cableResistivity;
		public double CableResistivity
		{
			get { return _cableResistivity; }
			set
			{
				_cableResistivity = value;
				OnPropertyChanged(() => CableResistivity);
			}
		}

		double _cableLenght;
		public double CableLenght
		{
			get { return _cableLenght; }
			set
			{
				_cableLenght = value;
				OnPropertyChanged(() => CableLenght);
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
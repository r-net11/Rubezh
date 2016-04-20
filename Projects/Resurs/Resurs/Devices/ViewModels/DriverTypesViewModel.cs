using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DriverTypesViewModel : SaveCancelDialogViewModel
	{
		public DriverTypesViewModel(DriverType parentDriverType)
		{
			Title = "Выбор типа устройства";

			DriverTypes = new ObservableCollection<DriverType>();
			var driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == parentDriverType);
			foreach(var childDriver in driver.Children)
			{
				DriverTypes.Add(childDriver);
			}
			SelectedDriverType = DriverTypes.FirstOrDefault();
		}

		public ObservableCollection<DriverType> DriverTypes { get; private set; }

		DriverType _selectedDriverType;
		public DriverType SelectedDriverType
		{
			get { return _selectedDriverType; }
			set
			{
				_selectedDriverType = value;
				OnPropertyChanged(() => SelectedDriverType);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}
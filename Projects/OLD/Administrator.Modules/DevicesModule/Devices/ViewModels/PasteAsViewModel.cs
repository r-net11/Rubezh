using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class PasteAsViewModel : SaveCancelDialogViewModel
	{
		public PasteAsViewModel(DriverType parentDriverType)
		{
			Title = "Выберите устройство";
			Drivers = new ObservableCollection<Driver>();
			var driverTypes = new List<DriverType>();

			switch (parentDriverType)
			{
				case DriverType.Computer:
					driverTypes = DriversHelper.UsbPanelDrivers;
					break;
				case DriverType.USB_Channel_1:
				case DriverType.USB_Channel_2:
					driverTypes = DriversHelper.PanelDrivers;
					break;
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				if (driverTypes.Contains(driver.DriverType))
					Drivers.Add(driver);
			}
			SelectedDriver = Drivers.FirstOrDefault();
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

		protected override bool Save()
		{
			return SelectedDriver != null;
		}
	}
}
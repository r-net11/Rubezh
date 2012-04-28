using System.Collections.Generic;
using GKModule.Converter;
using GKModule.Converter.Binary;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceConverterViewModel : DialogContent
	{
		public DeviceConverterViewModel()
		{
			Title = "Бинарный формат конфигурации";

			AllDB allDB = new AllDB();
			allDB.Build();
			DBViewModels = new List<DBViewModel>();
			foreach (var gkDB in allDB.GkDBList)
			{
				var dBViewModel = new DBViewModel(gkDB);
				DBViewModels.Add(dBViewModel);
			}
			foreach (var kauDB in allDB.KauDBList)
			{
				var dBViewModel = new DBViewModel(kauDB);
				DBViewModels.Add(dBViewModel);
			}

			if (DBViewModels.Count > 0)
				SelectedDBViewModel = DBViewModels[0];
		}

		public List<DBViewModel> DBViewModels { get; private set; }

		DBViewModel _selectedDBViewModel;
		public DBViewModel SelectedDBViewModel
		{
			get { return _selectedDBViewModel; }
			set
			{
				_selectedDBViewModel = value;
				OnPropertyChanged("SelectedDBViewModel");
				InitializeSelectedDB();
			}
		}

		void InitializeSelectedDB()
		{
			List<XDevice> devices = null;
			List<XZone> zones = null;

			if (SelectedDBViewModel.KauDB != null)
			{
				devices = SelectedDBViewModel.KauDB.Devices;
				zones = SelectedDBViewModel.KauDB.Zones;
			}
			if (SelectedDBViewModel.GkDB != null)
			{
				devices = SelectedDBViewModel.GkDB.Devices;
				zones = SelectedDBViewModel.GkDB.Zones;
			}

			BinObjects = new List<BinObjectViewModel>();
			foreach (var device in devices)
			{
				var binObjectViewModel = new BinObjectViewModel(device);
				BinObjects.Add(binObjectViewModel);
			}
			foreach (var zone in zones)
			{
				var binObjectViewModel = new BinObjectViewModel(zone);
				BinObjects.Add(binObjectViewModel);
			}
			if (BinObjects.Count > 0)
				SelectedBinObject = BinObjects[0];
		}

		List<BinObjectViewModel> _binObjects;
		public List<BinObjectViewModel> BinObjects
		{
			get { return _binObjects; }
			set
			{
				_binObjects = value;
				OnPropertyChanged("BinObjects");
			}
		}

		BinObjectViewModel _selectedBinObject;
		public BinObjectViewModel SelectedBinObject
		{
			get { return _selectedBinObject; }
			set
			{
				_selectedBinObject = value;
				OnPropertyChanged("SelectedBinObject");
			}
		}
	}
}
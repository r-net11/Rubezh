using Infrastructure.Common;
using System.Collections.ObjectModel;
using GKModule.Models;
using System.Collections.Generic;
using XFiresecAPI;
using GKModule.Converter;
using FiresecClient;
using GKModule.Converter.Binary;

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
			foreach (var gkDB in allDB.GKDBs)
			{
				var dBViewModel = new DBViewModel(gkDB);
				DBViewModels.Add(dBViewModel);
			}
			foreach (var kauDB in allDB.KauDBs)
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

			if (SelectedDBViewModel.KauDB != null)
				devices = SelectedDBViewModel.KauDB.Devices;
			if (SelectedDBViewModel.GkDB != null)
				devices = SelectedDBViewModel.GkDB.Devices;

			BinObjects = new List<BinObjectViewModel>();
			foreach (var device in devices)
			{
				var binObjectViewModel = new BinObjectViewModel(device);
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
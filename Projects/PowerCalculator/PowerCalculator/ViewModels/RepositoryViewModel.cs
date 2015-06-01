using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using Infrastructure.Common.Windows;
using System;

namespace PowerCalculator.ViewModels
{
	public class RepositoryViewModel : SaveCancelDialogViewModel
	{
		Configuration Configuration;

		public RepositoryViewModel(Configuration configuration)
		{
			Title = "Репозиторий устройств и кабелей";
			Configuration = configuration;
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
			AddCableCommand = new RelayCommand(OnAddCable);
			RemoveCableCommand = new RelayCommand(OnRemoveCable, CanRemoveCable);

			CollectToRepositoryCommand = new RelayCommand(OnCollectToRepository);
			GenerateFromRepositoryCommand = new RelayCommand(OnGenerateFromRepository);

			DeviceRepositoryItems = new ObservableCollection<DeviceRepositoryItemViewModel>(Configuration.DeviceRepositoryItems.Select(x => new DeviceRepositoryItemViewModel(x)));
			CableRepositoryItems = new ObservableCollection<CableRepositoryItemViewModel>(Configuration.CableRepositoryItems.Select(x => new CableRepositoryItemViewModel(x)));
		}

		public ObservableCollection<DeviceRepositoryItemViewModel> DeviceRepositoryItems { get; private set; }
		public ObservableCollection<CableRepositoryItemViewModel> CableRepositoryItems { get; private set; }

		DeviceRepositoryItemViewModel _selectedDeviceRepositoryItem;
		public DeviceRepositoryItemViewModel SelectedDeviceRepositoryItem
		{
			get { return _selectedDeviceRepositoryItem; }
			set
			{
				_selectedDeviceRepositoryItem = value;
				OnPropertyChanged(() => SelectedDeviceRepositoryItem);
			}
		}

		CableRepositoryItemViewModel _selectedCableRepositoryItem;
		public CableRepositoryItemViewModel SelectedCableRepositoryItem
		{
			get { return _selectedCableRepositoryItem; }
			set
			{
				_selectedCableRepositoryItem = value;
				OnPropertyChanged(() => SelectedCableRepositoryItem);
			}
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		void OnAddDevice()
		{
			var deviceRepositoryItem = new DeviceRepositoryItem();
			var deviceRepositoryItemViewModel = new DeviceRepositoryItemViewModel(deviceRepositoryItem);
			DeviceRepositoryItems.Add(deviceRepositoryItemViewModel);
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			DeviceRepositoryItems.Remove(SelectedDeviceRepositoryItem);
		}
		bool CanRemoveDevice()
		{
			return SelectedDeviceRepositoryItem != null;
		}

		public RelayCommand AddCableCommand { get; private set; }
		void OnAddCable()
		{
			var cableRepositoryItem = new CableRepositoryItem();
			var cableRepositoryItemViewModel = new CableRepositoryItemViewModel(cableRepositoryItem);
			CableRepositoryItems.Add(cableRepositoryItemViewModel);
		}

		public RelayCommand RemoveCableCommand { get; private set; }
		void OnRemoveCable()
		{
			CableRepositoryItems.Remove(SelectedCableRepositoryItem);
		}
		bool CanRemoveCable()
		{
			return SelectedCableRepositoryItem != null;
		}

		public RelayCommand CollectToRepositoryCommand { get; private set; }
		void OnCollectToRepository()
		{
			Processor.Processor.CollectToRepository(Configuration);
		}

		public RelayCommand GenerateFromRepositoryCommand { get; private set; }
		void OnGenerateFromRepository()
		{
			if (Configuration.DeviceRepositoryItems.Count == 0)
			{
				MessageBoxService.ShowError("Репозиторий устройств не содержит элементов!");
				return;
			}

			if (Configuration.CableRepositoryItems.Count == 0)
			{
				MessageBoxService.ShowError("Репозиторий кабелей не содержит элементов!");
				return;
			}

			var cableRemains = Processor.Processor.GenerateFromRepository(Configuration);
			if (cableRemains.Count() > 0)
			{
				string msg = "Неиспользованный кабель:\n";
				foreach (CableRepositoryItem cablePiece in cableRemains)
					msg += String.Format("Длина = {0} м, Сопротивление = {1} Ом;\n", cablePiece.Length, cablePiece.Resistivity);
				MessageBoxService.Show(msg);
			}
		}

		protected override bool Save()
		{
            Configuration.DeviceRepositoryItems = (from d in DeviceRepositoryItems
                                                    group d by d.DeviceRepositoryItem.DriverType into g
                                                    select new DeviceRepositoryItem { DriverType = g.Key, Count  = g.Sum(x => x.DeviceRepositoryItem.Count) }).ToList();

            Configuration.CableRepositoryItems = (from c in CableRepositoryItems
                                                   group c by c.CableRepositoryItem.Resistivity into g
                                                   select new CableRepositoryItem { Resistivity = g.Key, Length = g.Sum(x => x.CableRepositoryItem.Length) }).ToList();

			return base.Save();
		}
	}
}
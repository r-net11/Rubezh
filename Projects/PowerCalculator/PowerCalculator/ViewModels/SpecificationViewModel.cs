using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using Infrastructure.Common.Windows;
using System;

namespace PowerCalculator.ViewModels
{
	public class SpecificationViewModel : SaveCancelDialogViewModel
	{
		Configuration Configuration;

		public SpecificationViewModel(Configuration configuration)
		{
			Title = "Спецификация устройств и кабелей";
			Configuration = configuration;
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
			AddCableCommand = new RelayCommand(OnAddCable);
			RemoveCableCommand = new RelayCommand(OnRemoveCable, CanRemoveCable);

			CollectToSpecificationCommand = new RelayCommand(OnCollectToSpecification);
			GenerateFromSpecificationCommand = new RelayCommand(OnGenerateFromSpecification);

			DeviceSpecificationItems = new ObservableCollection<DeviceSpecificationItemViewModel>(Configuration.DeviceSpecificationItems.Select(x => new DeviceSpecificationItemViewModel(x)));
			CableSpecificationItems = new ObservableCollection<CableSpecificationItemViewModel>(Configuration.CableSpecificationItems.Select(x => new CableSpecificationItemViewModel(x)));
		}

		public ObservableCollection<DeviceSpecificationItemViewModel> DeviceSpecificationItems { get; private set; }
		public ObservableCollection<CableSpecificationItemViewModel> CableSpecificationItems { get; private set; }

		DeviceSpecificationItemViewModel _selectedDeviceSpecificationItem;
		public DeviceSpecificationItemViewModel SelectedDeviceSpecificationItem
		{
			get { return _selectedDeviceSpecificationItem; }
			set
			{
				_selectedDeviceSpecificationItem = value;
				OnPropertyChanged(() => SelectedDeviceSpecificationItem);
			}
		}

		CableSpecificationItemViewModel _selectedCableSpecificationItem;
		public CableSpecificationItemViewModel SelectedCableSpecificationItem
		{
			get { return _selectedCableSpecificationItem; }
			set
			{
				_selectedCableSpecificationItem = value;
				OnPropertyChanged(() => SelectedCableSpecificationItem);
			}
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		void OnAddDevice()
		{
			var deviceSpecificationItem = new DeviceSpecificationItem();
			var deviceSpecificationItemViewModel = new DeviceSpecificationItemViewModel(deviceSpecificationItem);
			DeviceSpecificationItems.Add(deviceSpecificationItemViewModel);
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			DeviceSpecificationItems.Remove(SelectedDeviceSpecificationItem);
		}
		bool CanRemoveDevice()
		{
			return SelectedDeviceSpecificationItem != null;
		}

		public RelayCommand AddCableCommand { get; private set; }
		void OnAddCable()
		{
			var cableSpecificationItem = new CableSpecificationItem();
			var cableSpecificationItemViewModel = new CableSpecificationItemViewModel(cableSpecificationItem);
			CableSpecificationItems.Add(cableSpecificationItemViewModel);
		}

		public RelayCommand RemoveCableCommand { get; private set; }
		void OnRemoveCable()
		{
			CableSpecificationItems.Remove(SelectedCableSpecificationItem);
		}
		bool CanRemoveCable()
		{
			return SelectedCableSpecificationItem != null;
		}

		public RelayCommand CollectToSpecificationCommand { get; private set; }
		void OnCollectToSpecification()
		{
			Processor.Processor.CollectToSpecification(Configuration);
		}

		public RelayCommand GenerateFromSpecificationCommand { get; private set; }
		void OnGenerateFromSpecification()
		{
			if (Configuration.DeviceSpecificationItems.Count == 0)
			{
				MessageBoxService.ShowError("Спецификация устройств не содержит элементов!");
				return;
			}

			if (Configuration.CableSpecificationItems.Count == 0)
			{
				MessageBoxService.ShowError("Спецификация кабелей не содержит элементов!");
				return;
			}

			var cableRemains = Processor.Processor.GenerateFromSpecification(Configuration);
			if (cableRemains.Count() > 0)
			{
				string msg = "Неиспользованный кабель:\n";
				foreach (CableSpecificationItem cablePiece in cableRemains)
					msg += String.Format("Длина = {0} м, Сопротивление = {1} Ом;\n", cablePiece.Length, cablePiece.Resistivity);
				MessageBoxService.Show(msg);
			}
		}

		protected override bool Save()
		{
            Configuration.DeviceSpecificationItems = (from d in DeviceSpecificationItems
                                                    group d by d.DeviceSpecificationItem.DriverType into g
                                                    select new DeviceSpecificationItem { DriverType = g.Key, Count  = g.Sum(x => x.DeviceSpecificationItem.Count) }).ToList();

            Configuration.CableSpecificationItems = (from c in CableSpecificationItems
                                                   group c by c.CableSpecificationItem.Resistivity into g
                                                   select new CableSpecificationItem { Resistivity = g.Key, Length = g.Sum(x => x.CableSpecificationItem.Length) }).ToList();

			return base.Save();
		}
	}
}
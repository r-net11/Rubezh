using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using Infrastructure.Common.Windows;
using System;
using System.Collections.Generic;

namespace PowerCalculator.ViewModels
{
	public class SpecificationViewModel : SaveCancelDialogViewModel
	{
		Configuration Configuration;
        Action InitializeConfiguration;

        public SpecificationViewModel(Configuration configuration, Action initializeConfiguration, 
            List<DeviceSpecificationItem> deviceSpecificationItems = null,
            List<CableSpecificationItem> cableSpecificationItems = null)
		{
			Title = "Спецификация устройств и кабелей";
			Configuration = configuration;
            InitializeConfiguration = initializeConfiguration;
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
			AddCableCommand = new RelayCommand(OnAddCable);
			RemoveCableCommand = new RelayCommand(OnRemoveCable, CanRemoveCable);

			GenerateFromSpecificationCommand = new RelayCommand(OnGenerateFromSpecification);

            DeviceSpecificationItems =
                new ObservableCollection<DeviceSpecificationItemViewModel>(
                    (deviceSpecificationItems == null ? Configuration.DeviceSpecificationItems : deviceSpecificationItems).Select(x => new DeviceSpecificationItemViewModel(x)));

            CableSpecificationItems =
                new ObservableCollection<CableSpecificationItemViewModel>(
                    (cableSpecificationItems == null ? Configuration.CableSpecificationItems : cableSpecificationItems).Select(x => new CableSpecificationItemViewModel(x)));
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
            SelectedDeviceSpecificationItem = DeviceSpecificationItems.LastOrDefault();
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
            var index = DeviceSpecificationItems.IndexOf(SelectedDeviceSpecificationItem);
			DeviceSpecificationItems.Remove(SelectedDeviceSpecificationItem);
            index = Math.Min(index, DeviceSpecificationItems.Count - 1);
            if (index > -1)
                SelectedDeviceSpecificationItem = DeviceSpecificationItems[index];
		}
		bool CanRemoveDevice()
		{
			return SelectedDeviceSpecificationItem != null;
		}

		public RelayCommand AddCableCommand { get; private set; }
		void OnAddCable()
		{
            var cableSpecificationItem = new CableSpecificationItem() { CableType = Processor.CableTypesRepository.CustomCableType};
			var cableSpecificationItemViewModel = new CableSpecificationItemViewModel(cableSpecificationItem);
			CableSpecificationItems.Add(cableSpecificationItemViewModel);
            SelectedCableSpecificationItem = CableSpecificationItems.LastOrDefault();
		}

		public RelayCommand RemoveCableCommand { get; private set; }
		void OnRemoveCable()
		{
            var index = CableSpecificationItems.IndexOf(SelectedCableSpecificationItem);
            CableSpecificationItems.Remove(SelectedCableSpecificationItem);
            index = Math.Min(index, CableSpecificationItems.Count - 1);
            if (index > -1)
                SelectedCableSpecificationItem = CableSpecificationItems[index];
		}
		bool CanRemoveCable()
		{
			return SelectedCableSpecificationItem != null;
		}

		public RelayCommand GenerateFromSpecificationCommand { get; private set; }
		void OnGenerateFromSpecification()
		{
			if (DeviceSpecificationItems.Count == 0)
			{
				MessageBoxService.ShowError("Спецификация устройств не содержит элементов!");
				return;
			}

			if (CableSpecificationItems.Count == 0)
			{
				MessageBoxService.ShowError("Спецификация кабелей не содержит элементов!");
				return;
			}

            var cableRemains = Processor.Processor.GenerateFromSpecification(Configuration, DeviceSpecificationItems.Select(x => x.DeviceSpecificationItem).ToList(), CableSpecificationItems.Select(x => x.CableSpecificationItem).ToList());
			if (cableRemains.Count() > 0)
			{
				string msg = "Неиспользованный кабель:\n";
				foreach (CableSpecificationItem cablePiece in cableRemains)
					msg += String.Format("Тип: {0}, Длина = {1} м, Сопротивление = {2} Ом;\n", cablePiece.CableType.ToString(), cablePiece.Length, cablePiece.Resistivity);
				MessageBoxService.Show(msg);
			}
            InitializeConfiguration();
		}

		protected override bool Save()
		{
            Configuration.DeviceSpecificationItems = DeviceSpecificationItems.Select(x => x.DeviceSpecificationItem).ToList();
            Configuration.CableSpecificationItems = CableSpecificationItems.Select(x => x.CableSpecificationItem).ToList();

            return base.Save();
		}
	}
}
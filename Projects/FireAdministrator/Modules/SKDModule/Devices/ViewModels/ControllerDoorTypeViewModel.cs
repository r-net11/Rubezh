using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class ControllerDoorTypeViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }
		bool HasChanged { get; set; }

		public ControllerDoorTypeViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Задание типа контроллера";
			DeviceViewModel = deviceViewModel;

			GetDoorTypeCommand = new RelayCommand(OnGetDoorType);
			SetDoorTypeCommand = new RelayCommand(OnSetDoorType);

			AvailableDoorTypes = new ObservableCollection<DoorType>();
			AvailableDoorTypes.Add(DoorType.OneWay);
			AvailableDoorTypes.Add(DoorType.TwoWay);
			SelectedDoorType = DeviceViewModel.Device.DoorType;
			HasChanged = false;
		}

		public ObservableCollection<DoorType> AvailableDoorTypes { get; private set; }

		DoorType _selectedDoorType;
		public DoorType SelectedDoorType
		{
			get { return _selectedDoorType; }
			set
			{
				_selectedDoorType = value;
				OnPropertyChanged(() => SelectedDoorType);
				HasChanged = true;
			}
		}

		public RelayCommand GetDoorTypeCommand { get; private set; }
		void OnGetDoorType()
		{
			var result = FiresecManager.FiresecService.GetControllerDoorType(DeviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				SelectedDoorType = result.Result;
				HasChanged = false;
			}
		}

		public RelayCommand SetDoorTypeCommand { get; private set; }
		void OnSetDoorType()
		{
			var result = FiresecManager.FiresecService.SetControllerDoorType(DeviceViewModel.Device, SelectedDoorType);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				HasChanged = false;
			}
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				DeviceViewModel.Device.DoorType = SelectedDoorType;
				ServiceFactory.SaveService.SKDChanged = true;
			}

			if (HasChanged)
			{
				if (!MessageBoxService.ShowConfirmation2("Настройки не записаны в прибор. вы уверены, что хотите закрыть окно без записи в прибор?"))
					return false;
			}
			return base.Save();
		}
	}
}
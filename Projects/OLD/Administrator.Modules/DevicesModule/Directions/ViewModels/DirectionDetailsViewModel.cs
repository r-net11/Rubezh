using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DirectionDetailsViewModel : SaveCancelDialogViewModel
	{
		bool _isNew;
		public Direction Direction { get; private set; }

		public DirectionDetailsViewModel(Direction direction = null)
		{
			ResetRmCommand = new RelayCommand(OnResetRm);
			ResetButtonCommand = new RelayCommand(OnResetButton);
			ChooseRmCommand = new RelayCommand(OnChooseRm);
			ChooseButtonCommand = new RelayCommand(OnChooseButton);

			_isNew = direction == null;
			Title = _isNew ? "Создать направление" : "Редактировать направление";
			if (direction == null)
				CreateNew();
			else
				Direction = direction;

			CopyProperties();
		}

		void CreateNew()
		{
			Direction = new Direction();
			if (FiresecManager.Directions.Count > 0)
				Direction.Id = FiresecManager.Directions.Max(x => x.Id) + 1;
			else
				Direction.Id = 1;

			Direction.Name = "Новое направление " + Direction.Id.ToString();
		}

		void CopyProperties()
		{
			Id = Direction.Id;
			Name = Direction.Name;
			Description = Direction.Description;

			if (Direction.DeviceRm != null)
				DeviceRm = FiresecManager.Devices.FirstOrDefault(x => x.UID == Direction.DeviceRm);
			if (Direction.DeviceButton != null)
				DeviceButton = FiresecManager.Devices.FirstOrDefault(x => x.UID == Direction.DeviceButton);
		}

		int _id;
		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
				OnPropertyChanged(() => Id);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		Device _deviceRm;
		public Device DeviceRm
		{
			get { return _deviceRm; }
			set
			{
				_deviceRm = value;
				OnPropertyChanged(() => DeviceRm);
			}
		}

		Device _deviceButton;
		public Device DeviceButton
		{
			get { return _deviceButton; }
			set
			{
				_deviceButton = value;
				OnPropertyChanged(() => DeviceButton);
			}
		}

		void SaveModel()
		{
			Direction.Id = Id;
			Direction.Name = Name;
			Direction.Description = Description;

			Direction.DeviceRm = Guid.Empty;
			Direction.DeviceButton = Guid.Empty;

			if (DeviceRm != null)
				Direction.DeviceRm = DeviceRm.UID;
			if (DeviceButton != null)
				Direction.DeviceButton = DeviceButton.UID;
		}

		public RelayCommand ResetRmCommand { get; private set; }
		void OnResetRm()
		{
			DeviceRm = null;
		}

		public RelayCommand ResetButtonCommand { get; private set; }
		void OnResetButton()
		{
			DeviceButton = null;
		}

		public RelayCommand ChooseRmCommand { get; private set; }
		void OnChooseRm()
		{
			var directionDeviceSelectorViewModel = new DirectionDeviceSelectorViewModel(Direction, DriverType.RM_1);

			if (DialogService.ShowModalWindow(directionDeviceSelectorViewModel))
				DeviceRm = directionDeviceSelectorViewModel.SelectedDevice.Device;
		}

		public RelayCommand ChooseButtonCommand { get; private set; }
		void OnChooseButton()
		{
			var directionDeviceSelectorViewModel = new DirectionDeviceSelectorViewModel(Direction, DriverType.ShuzUnblockButton);

			if (DialogService.ShowModalWindow(directionDeviceSelectorViewModel))
				DeviceButton = directionDeviceSelectorViewModel.SelectedDevice.Device;
		}

		protected override bool Save()
		{
			if (_isNew)
			{
				if (FiresecManager.Directions.Any(x => x.Id == Id))
				{
					MessageBoxService.Show("Невозможно сохранить. Номера направдений совпадают");
					return false;
				}
				SaveModel();
			}
			else
			{
				if (Id != Direction.Id && FiresecManager.Directions.Any(x => x.Id == Id))
				{
					MessageBoxService.Show("Невозможно сохранить. Номера направдений совпадают");
					return false;
				}
				SaveModel();
			}
			return base.Save();
		}
	}
}
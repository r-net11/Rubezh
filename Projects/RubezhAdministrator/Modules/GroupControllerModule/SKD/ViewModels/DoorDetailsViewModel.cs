using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DoorDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKDoor Door { get; private set; }

		public bool IsEdit { get; private set; }

		public DoorDetailsViewModel(GKDoor door = null)
		{
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);
			ResetPropertiesCommand = new RelayCommand(OnResetProperties);

			if (door == null)
			{
				Title = "Создание новой точки доступа";
				IsEdit = false;

				Door = new GKDoor()
				{
					Name = "Новая точка доступа",
					No = 1,
					PlanElementUIDs = new List<Guid>(),
				};
				if (GKManager.DeviceConfiguration.Doors.Count != 0)
					Door.No = (GKManager.DeviceConfiguration.Doors.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства точки доступа: {0}", door.PresentationName);
				Door = door;
				IsEdit = true;
			}

			AvailableDoorTypes = new ObservableCollection<GKDoorType>(Enum.GetValues(typeof(GKDoorType)).Cast<GKDoorType>());
			SelectedDoorType = Door.DoorType;
			AntipassbackOn = Door.AntipassbackOn;

			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDoor in GKManager.DeviceConfiguration.Doors)
			{
				availableNames.Add(existingDoor.Name);
				availableDescription.Add(existingDoor.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = ClientManager.RubezhService.GKGetSingleParameter(Door);
			if (!result.HasError && result.Result != null)
			{
				Delay = result.Result[0].Value;
				Hold = result.Result[1].Value;

				OnPropertyChanged(() => Delay);
				OnPropertyChanged(() => Hold);
			}
			else
			{
				MessageBoxService.ShowError(result.Error);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand WritePropertiesCommand { get; private set; }
		void OnWriteProperties()
		{
			Door.No = No;
			Door.Name = Name;
			Door.Description = Description;
			Door.Delay = Delay;
			Door.Hold = Hold;
			Door.EnterLevel = EnterLevel;
			Door.DoorType = SelectedDoorType;
			Door.AntipassbackOn = AntipassbackOn;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(Door);
			if (baseDescriptor != null)
			{
				var result = ClientManager.RubezhService.GKSetSingleParameter(Door, baseDescriptor.Parameters);
				if (result.HasError)
				{
					MessageBoxService.ShowError(result.Error);
				}
			}
			else
			{
				MessageBoxService.ShowError("Ошибка. Отсутствуют параметры");
			}
		}

		public RelayCommand ResetPropertiesCommand { get; private set; }
		void OnResetProperties()
		{
			Delay = 2;
			Hold = 20;
		}

		bool CompareLocalWithRemoteHashes()
		{
			if (Door.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("ТД не относится ни к одному ГК");
				return false;
			}

			var result = ClientManager.RubezhService.GKGKHash(Door.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			GKManager.DeviceConfiguration.PrepareDescriptors();
			var localHash = GKFileInfo.CreateHash1(Door.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}

		void CopyProperties()
		{
			No = Door.No;
			Name = Door.Name;
			Description = Door.Description;
			Delay = Door.Delay;
			Hold = Door.Hold;
			EnterLevel = Door.EnterLevel;
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
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

		public ObservableCollection<GKDoorType> AvailableDoorTypes { get; private set; }

		GKDoorType _selectedDoorType;
		public GKDoorType SelectedDoorType
		{
			get { return _selectedDoorType; }
			set
			{
				if (value == GKDoorType.OneWay || value == GKDoorType.TwoWay || value == GKDoorType.AirlockBooth || value == GKDoorType.Barrier)
				{
					Delay = 2;
					if (value == GKDoorType.OneWay || value == GKDoorType.TwoWay)
						Hold = 20;

					if (value == GKDoorType.AirlockBooth || value == GKDoorType.Barrier)
						Hold = 30;
					Delay = 15;
				}
				if (value == GKDoorType.Turnstile)
				{
					Delay = 5;
					Hold = 5;
				}
				_selectedDoorType = value;
				OnPropertyChanged(() => SelectedDoorType);
				OnPropertyChanged(() => HasNotAntipassback);
			}
		}

		int _delay;
		public int Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		int _hold;
		public int Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged(() => Hold);
			}
		}

		int _enterLevel;
		public int EnterLevel
		{
			get { return _enterLevel; }
			set
			{
				_enterLevel = value;
				OnPropertyChanged(() => EnterLevel);
			}
		}

		bool _antipassbackOn;
		public bool AntipassbackOn
		{
			get { return _antipassbackOn; }
			set
			{
				_antipassbackOn = value;
				OnPropertyChanged(() => AntipassbackOn);
			}
		}

		public bool HasNotAntipassback
		{
			get { return SelectedDoorType == GKDoorType.OneWay; }
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (Door.No != No && GKManager.DeviceConfiguration.Doors.Any(x => x.No == No))
			{
				MessageBoxService.Show("Точка доступа с таким номером уже существует");
				return false;
			}

			Door.No = No;
			Door.Name = Name;
			Door.Description = Description;
			Door.Delay = Delay;
			Door.Hold = Hold;
			Door.EnterLevel = EnterLevel;
			Door.DoorType = SelectedDoorType;
			Door.AntipassbackOn = AntipassbackOn;
			if (SelectedDoorType == GKDoorType.OneWay)
				Door.ExitZoneUID = Guid.Empty;
			return base.Save();
		}
	}
}
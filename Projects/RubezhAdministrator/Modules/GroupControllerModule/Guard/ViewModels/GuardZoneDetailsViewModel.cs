using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.ViewModels
{
	public class GuardZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKGuardZone Zone;
		public bool IsEdit { get; private set; }
		public GuardZoneDetailsViewModel(GKGuardZone zone = null)
		{
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);
			ResetPropertiesCommand = new RelayCommand(OnResetProperties);
			if (zone == null)
			{
				IsEdit = false;
				Title = "Создание новой зоны";

				Zone = new GKGuardZone()
				{
					Name = "Новая зона",
					No = 1
				};
				if (GKManager.DeviceConfiguration.GuardZones.Count != 0)
					Zone.No = (GKManager.DeviceConfiguration.GuardZones.Select(x => x.No).Max() + 1);
			}
			else
			{
				IsEdit = true;
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}

			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingZone in GKManager.DeviceConfiguration.GuardZones)
			{
				availableNames.Add(existingZone.Name);
				availableDescription.Add(existingZone.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			No = Zone.No;
			Name = Zone.Name;
			Description = Zone.Description;
			SetDelay = Zone.SetDelay;
			ResetDelay = Zone.ResetDelay;
			AlarmDelay = Zone.AlarmDelay;
			IsExtraProtected = Zone.IsExtraProtected;
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

		int _setDelay;
		public int SetDelay
		{
			get { return _setDelay; }
			set
			{
				_setDelay = value;
				OnPropertyChanged(() => SetDelay);
			}
		}

		int _resetDelay;
		public int ResetDelay
		{
			get { return _resetDelay; }
			set
			{
				_resetDelay = value;
				OnPropertyChanged(() => ResetDelay);
			}
		}

		int _alarmDelay;
		public int AlarmDelay
		{
			get { return _alarmDelay; }
			set
			{
				_alarmDelay = value;
				if (AlarmDelay == 0)
					AlarmDelay = 1;
				OnPropertyChanged(() => AlarmDelay);
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		bool _isExtraProtected;
		public bool IsExtraProtected
		{
			get { return _isExtraProtected; }
			set
			{
				_isExtraProtected = value;
				OnPropertyChanged(() => IsExtraProtected);
			}
		}

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (Zone.No != No && GKManager.DeviceConfiguration.GuardZones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Zone.No = No;
			Zone.Name = Name;
			Zone.Description = Description;
			Zone.SetDelay = SetDelay;
			Zone.ResetDelay = ResetDelay;
			Zone.AlarmDelay = AlarmDelay;
			Zone.IsExtraProtected = IsExtraProtected;
			return base.Save();
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = ClientManager.RubezhService.GKGetSingleParameter(Zone);
			if (!result.HasError && result.Result != null)
			{
				SetDelay = result.Result[0].Value;
				ResetDelay = result.Result[1].Value;
				AlarmDelay = result.Result[2].Value;
				OnPropertyChanged(() => SetDelay);
				OnPropertyChanged(() => ResetDelay);
				OnPropertyChanged(() => AlarmDelay);
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
			Zone.Name = Name;
			Zone.No = No;
			Zone.Description = Description;
			Zone.AlarmDelay = AlarmDelay;
			Zone.ResetDelay = ResetDelay;
			Zone.SetDelay = SetDelay;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(Zone);
			if (baseDescriptor != null)
			{
				var result = ClientManager.RubezhService.GKSetSingleParameter(Zone, baseDescriptor.Parameters);
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
			SetDelay = 0;
			ResetDelay = 0;
			AlarmDelay = 1;
		}

		bool CompareLocalWithRemoteHashes()
		{
			if (Zone.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("Зона не относится ни к одному ГК");
				return false;
			}

			var result = ClientManager.RubezhService.GKGKHash(Zone.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			GKManager.DeviceConfiguration.PrepareDescriptors();
			var localHash = GKFileInfo.CreateHash1(Zone.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}
	}
}
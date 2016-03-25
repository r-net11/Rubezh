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
	public class DirectionDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKDirection Direction { get; private set; }

		public bool IsEdit { get; private set; }

		public DirectionDetailsViewModel(GKDirection direction = null)
		{
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);
			ResetPropertiesCommand = new RelayCommand(OnResetProperties);
			DelayRegimes = Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>().ToList();

			if (direction == null)
			{
				Title = "Создание нового направления";
				IsEdit = false;

				Direction = new GKDirection()
				{
					Name = "Новое направление",
					No = 1
				};
				if (GKManager.Directions.Count != 0)
					Direction.No = (GKManager.Directions.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства направления: {0}", direction.PresentationName);
				Direction = direction;
				IsEdit = true;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDirection in GKManager.Directions)
			{
				availableNames.Add(existingDirection.Name);
				availableDescription.Add(existingDirection.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			Name = Direction.Name;
			No = Direction.No;
			Delay = Direction.Delay;
			Hold = Direction.Hold;
			DelayRegime = Direction.DelayRegime;
			Description = Direction.Description;
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

		ushort _delay;
		public ushort Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		ushort _hold;
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged(() => Hold);
			}
		}

		public List<DelayRegime> DelayRegimes { get; private set; }

		DelayRegime _delayRegime;
		public DelayRegime DelayRegime
		{
			get { return _delayRegime; }
			set
			{
				_delayRegime = value;
				OnPropertyChanged(() => DelayRegime);
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (No <= 0)
			{
                ServiceFactory.MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (Direction.No != No && GKManager.Directions.Any(x => x.No == No))
			{
				ServiceFactory.MessageBoxService.Show("Направление с таким номером уже существует");
				return false;
			}

			Direction.Name = Name;
			Direction.No = No;
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.DelayRegime = DelayRegime;
			Direction.Description = Description;
			return base.Save();
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = ClientManager.FiresecService.GKGetSingleParameter(Direction);
			if (!result.HasError && result.Result != null && result.Result.Count == 3)
			{
				Delay = result.Result[0].Value;
				Hold = result.Result[1].Value;
				DelayRegime = (DelayRegime)result.Result[2].Value;
				OnPropertyChanged(() => Delay);
				OnPropertyChanged(() => Hold);
				OnPropertyChanged(() => DelayRegime);
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
			Direction.Name = Name;
			Direction.No = No;
			Direction.Description = Description;
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.DelayRegime = DelayRegime;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(Direction);
			if (baseDescriptor != null)
			{
				var result = ClientManager.FiresecService.GKSetSingleParameter(Direction, baseDescriptor.Parameters);
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
			Delay = 0;
			Hold = 0;
			DelayRegime = DelayRegime.On;
		}

		bool CompareLocalWithRemoteHashes()
		{
			if (Direction.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("Направление не относится ни к одному ГК");
				return false;
			}

			var result = ClientManager.FiresecService.GKGKHash(Direction.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			GKManager.DeviceConfiguration.PrepareDescriptors();
			var localHash = GKFileInfo.CreateHash1(Direction.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}
	}
}
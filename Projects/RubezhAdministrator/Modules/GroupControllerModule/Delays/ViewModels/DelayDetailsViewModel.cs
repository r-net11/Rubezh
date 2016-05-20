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
	public class DelayDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKDelay Delay { get; private set; }
		public bool IsEdit { get; private set; }

		public DelayDetailsViewModel(GKDelay delay = null)
		{
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);
			ResetPropertiesCommand = new RelayCommand(OnResetProperties);
			DelayRegimes = Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>().ToList();

			if (delay == null)
			{
				Title = "Создание новой задержки";
				IsEdit = false;
				Delay = new GKDelay()
				{
					Name = "Задержка",
					No = 1,
				};
				if (GKManager.Delays.Count != 0)
					Delay.No = (GKManager.Delays.Select(x => x.No).Max() + 1);
			}
			else
			{
				IsEdit = true;
				Title = string.Format("Свойства задержки: {0}", delay.PresentationName);
				Delay = delay;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescriptions = new HashSet<string>();
			foreach (var existingDelay in GKManager.Delays)
			{
				availableNames.Add(existingDelay.Name);
				availableDescriptions.Add(existingDelay.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescriptions = new ObservableCollection<string>(availableDescriptions);
		}

		void CopyProperties()
		{
			No = Delay.No;
			Name = Delay.Name;
			Description = Delay.Description;
			DelayTime = Delay.DelayTime;
			Hold = Delay.Hold;
			DelayRegime = Delay.DelayRegime;
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

		ushort _delayTime;
		public ushort DelayTime
		{
			get { return _delayTime; }
			set
			{
				_delayTime = value;
				OnPropertyChanged(() => DelayTime);
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
		public ObservableCollection<string> AvailableDescriptions { get; private set; }

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (GKManager.Delays.Any(x => x.No == No && x.UID != Delay.UID))
			{
				ServiceFactory.MessageBoxService.Show("Задержка с таким номером уже существует");
				return false;
			}

			Delay.No = No;
			Delay.Name = Name;
			Delay.Description = Description;
			Delay.DelayTime = DelayTime;
			Delay.Hold = Hold;
			Delay.DelayRegime = DelayRegime;
			return base.Save();
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = ClientManager.RubezhService.GKGetSingleParameter(Delay);
			if (!result.HasError && result.Result != null && result.Result.Count == 3)
			{
				DelayTime = result.Result[0].Value;
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
			Delay.Name = Name;
			Delay.No = No;
			Delay.Description = Description;
			Delay.DelayTime = DelayTime;
			Delay.Hold = Hold;
			Delay.DelayRegime = DelayRegime;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(Delay);
			if (baseDescriptor != null)
			{
				var result = ClientManager.RubezhService.GKSetSingleParameter(Delay, baseDescriptor.Parameters);
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
			DelayTime = 0;
			Hold = 0;
			DelayRegime = DelayRegime.On;
		}

		bool CompareLocalWithRemoteHashes()
		{
			if (Delay.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("Задержка не относится ни к одному ГК");
				return false;
			}

			var result = ClientManager.RubezhService.GKGKHash(Delay.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			GKManager.DeviceConfiguration.PrepareDescriptors();
			var localHash = GKFileInfo.CreateHash1(Delay.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}
	}
}
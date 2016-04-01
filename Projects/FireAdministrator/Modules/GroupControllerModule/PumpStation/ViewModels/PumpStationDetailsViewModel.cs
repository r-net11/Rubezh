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
	public class PumpStationDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKPumpStation PumpStation { get; private set; }
		public bool IsEdit { get; private set; }

		public PumpStationDetailsViewModel(GKPumpStation pumpStation = null)
		{
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);
			ResetPropertiesCommand = new RelayCommand(OnResetProperties);

			IsEdit = pumpStation != null;
			if (pumpStation == null)
			{
				Title = "Создание новой насосной станции";

				PumpStation = new GKPumpStation()
				{
					Name = "Насосная станция",
					No = 1
				};
				if (GKManager.PumpStations.Count != 0)
					PumpStation.No = (GKManager.PumpStations.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства Насосной станции: {0}", pumpStation.PresentationName);
				PumpStation = pumpStation;
			}
			CopyProperties();

			AvailableDelayRegimeTypes = new ObservableCollection<DelayRegime>(Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>());
			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingPumpStation in GKManager.PumpStations)
			{
				availableNames.Add(existingPumpStation.Name);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = ClientManager.FiresecService.GKGetSingleParameter(PumpStation);
			if (!result.HasError && result.Result != null)
			{
				Delay = result.Result[0].Value;
				Hold = result.Result[1].Value;
				DelayRegime = (DelayRegime)result.Result[2].Value;
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
			PumpStation.Name = Name;
			PumpStation.No = No;
			PumpStation.Description = Description;
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			PumpStation.DelayRegime = DelayRegime;
			PumpStation.NSDeltaTime = NSDeltaTime;
			PumpStation.NSPumpsCount = NSPumpsCount;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(PumpStation);
			if (baseDescriptor != null)
			{
				var result = ClientManager.FiresecService.GKSetSingleParameter(PumpStation, baseDescriptor.Parameters);
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
			Hold = 600;
			DelayRegime = DelayRegime.Off;
			NSDeltaTime = 5;
			NSPumpsCount = 1;
		}

		bool CompareLocalWithRemoteHashes()
		{
			if (PumpStation.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("НС не относится ни к одному ГК");
				return false;
			}

			var result = ClientManager.FiresecService.GKGKHash(PumpStation.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			GKManager.DeviceConfiguration.PrepareDescriptors();
			var localHash = GKFileInfo.CreateHash1(PumpStation.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}

		void CopyProperties()
		{
			Name = PumpStation.Name;
			No = PumpStation.No;
			Delay = PumpStation.Delay;
			Hold = PumpStation.Hold;
			DelayRegime = PumpStation.DelayRegime;
			Description = PumpStation.Description;
			NSPumpsCount = PumpStation.NSPumpsCount;
			NSDeltaTime = PumpStation.NSDeltaTime;
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

		public ObservableCollection<DelayRegime> AvailableDelayRegimeTypes { get; private set; }
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

		public ObservableCollection<string> AvailableNames { get; private set; }

		int _mainPumpsCount;
		public int NSPumpsCount
		{
			get { return _mainPumpsCount; }
			set
			{
				_mainPumpsCount = value;
				OnPropertyChanged(() => NSPumpsCount);
			}
		}

		int _pumpsDeltaTime;
		public int NSDeltaTime
		{
			get { return _pumpsDeltaTime; }
			set
			{
				_pumpsDeltaTime = value;
				OnPropertyChanged("PumpsDeltaTime");
			}
		}

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (PumpStation.No != No && GKManager.PumpStations.Any(x => x.No == No))
			{
				MessageBoxService.Show("НС с таким номером уже существует");
				return false;
			}

			PumpStation.Name = Name;
			PumpStation.No = No;
			PumpStation.Delay = Delay;
			PumpStation.DelayRegime = DelayRegime;
			PumpStation.Hold = Hold;
			PumpStation.Description = Description;
			PumpStation.NSPumpsCount = NSPumpsCount;
			PumpStation.NSDeltaTime = NSDeltaTime;
			return base.Save();
		}
	}
}
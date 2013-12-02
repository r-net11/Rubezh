using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class PumpStationDetailsViewModel : SaveCancelDialogViewModel
	{
		public XPumpStation PumpStation { get; private set; }

		public PumpStationDetailsViewModel(XPumpStation pumpStation = null)
		{
			ResetAUPropertiesCommand = new RelayCommand(OnResetAUProperties);
			if (pumpStation == null)
			{
				Title = "Создание новой насосоной станции";

				PumpStation = new XPumpStation()
				{
					Name = "Насосная станция",
					No = 1
				};
				if (XManager.PumpStations.Count != 0)
					PumpStation.No = (ushort)(XManager.PumpStations.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства Насосной станции: {0}", pumpStation.PresentationName);
				PumpStation = pumpStation;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingPumpStation in XManager.PumpStations)
			{
				availableNames.Add(existingPumpStation.Name);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
		}

		void ChangeParameter(ushort delay, ushort hold, ushort regime)
		{
			Delay = delay;
			Hold = hold;
			Regime = regime;
			OnPropertyChanged("Delay");
			OnPropertyChanged("Hold");
			OnPropertyChanged("Regime");
		}

		void CopyProperties()
		{
			Name = PumpStation.Name;
			No = PumpStation.No;
			Delay = PumpStation.Delay;
			Hold = PumpStation.Hold;
			Regime = PumpStation.Regime;
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
				OnPropertyChanged("Name");
			}
		}

		ushort _no;
		public ushort No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged("No");
			}
		}

		ushort _delay;
		public ushort Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged("Delay");
			}
		}

		ushort _hold;
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged("Hold");
			}
		}

		ushort _regime;
		public ushort Regime
		{
			get { return _regime; }
			set
			{
				_regime = value;
				OnPropertyChanged("Regime");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
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
				OnPropertyChanged("MainPumpsCount");
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
			if (PumpStation.No != No && XManager.PumpStations.Any(x => x.No == No))
			{
				MessageBoxService.Show("Направление с таким номером уже существует");
				return false;
			}

			PumpStation.Name = Name;
			PumpStation.No = No;
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			PumpStation.Regime = Regime;
			PumpStation.Description = Description;
			PumpStation.NSPumpsCount = NSPumpsCount;
			PumpStation.NSDeltaTime = NSDeltaTime;
			return base.Save();
		}

		public RelayCommand ResetAUPropertiesCommand { get; private set; }
		void OnResetAUProperties()
		{
			Delay = 10;
			Hold = 10;
			Regime = 1;
		}
	}
}
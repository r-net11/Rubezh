using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PumpStationDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKPumpStation PumpStation { get; private set; }

		public PumpStationDetailsViewModel(GKPumpStation pumpStation = null)
		{
			if (pumpStation == null)
			{
				Title = "Создание новой насосоной станции";

				PumpStation = new GKPumpStation()
				{
					Name = "Насосная станция",
					No = 1
				};
				if (GKManager.PumpStations.Count != 0)
					PumpStation.No = (ushort)(GKManager.PumpStations.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства Насосной станции: {0}", pumpStation.PresentationName);
				PumpStation = pumpStation;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingPumpStation in GKManager.PumpStations)
			{
				availableNames.Add(existingPumpStation.Name);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
		}

		void CopyProperties()
		{
			Name = PumpStation.Name;
			No = PumpStation.No;
			Delay = PumpStation.Delay;
			Hold = PumpStation.Hold;
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
				MessageBoxService.ShowExtended("Номер должен быть положительным числом");
				return false;
			}
			if (PumpStation.No != No && GKManager.PumpStations.Any(x => x.No == No))
			{
				MessageBoxService.ShowExtended("НС с таким номером уже существует");
				return false;
			}

			PumpStation.Name = Name;
			PumpStation.No = No;
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			PumpStation.Description = Description;
			PumpStation.NSPumpsCount = NSPumpsCount;
			PumpStation.NSDeltaTime = NSDeltaTime;
			return base.Save();
		}
	}
}
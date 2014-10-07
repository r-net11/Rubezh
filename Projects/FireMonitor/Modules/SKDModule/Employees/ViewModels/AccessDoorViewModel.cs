using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessDoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
		public List<CardDoor> CardDoors { get; private set; }
		Action<AccessDoorViewModel> OnChecked;

		public AccessDoorViewModel(SKDDoor door, List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			Door = door;
			CardDoors = cardDoors;
			OnChecked = onChecked;

			TimeCreterias = new ObservableCollection<IntervalType>();
			TimeCreterias.Add(IntervalType.Weekly);

			if (CardDoors == null)
				CardDoors = new List<CardDoor>();

			var cardDoor = CardDoors.FirstOrDefault(x => x.DoorUID == door.UID);
			if (cardDoor != null)
			{
				_isChecked = true;
				SelectedEnterTimeCreteria = TimeCreterias.FirstOrDefault(x => x == cardDoor.EnterIntervalType);
				SelectedExitTimeCreteria = TimeCreterias.FirstOrDefault(x => x == cardDoor.ExitIntervalType);
				SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault(x => x.ScheduleID == cardDoor.EnterIntervalID);
				if (SelectedEnterTimeType == null)
					SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault();
				SelectedExitTimeType = ExitTimeTypes.FirstOrDefault(x => x.ScheduleID == cardDoor.ExitIntervalID);
				if (SelectedExitTimeType == null)
					SelectedExitTimeType = ExitTimeTypes.FirstOrDefault();
			}
			else
			{
				SelectedEnterTimeCreteria = TimeCreterias.FirstOrDefault();
				SelectedExitTimeCreteria = TimeCreterias.FirstOrDefault();
				SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault();
				SelectedExitTimeType = ExitTimeTypes.FirstOrDefault();
			}
		}

		public bool HasEnter
		{
			get { return Door.InDeviceUID != Guid.Empty; }
		}

		public bool HasExit
		{
			get { return Door.OutDeviceUID != Guid.Empty && Door.DoorType == DoorType.TwoWay; }
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (OnChecked != null)
					OnChecked(this);
			}
		}

		public ObservableCollection<IntervalType> TimeCreterias { get; private set; }

		IntervalType _selectedEnterTimeCreteria;
		public IntervalType SelectedEnterTimeCreteria
		{
			get { return _selectedEnterTimeCreteria; }
			set
			{
				_selectedEnterTimeCreteria = value;
				OnPropertyChanged(() => SelectedEnterTimeCreteria);
				EnterTimeTypes = GetTimeTypes(value);
			}
		}

		IntervalType _selectedExitTimeCreteria;
		public IntervalType SelectedExitTimeCreteria
		{
			get { return _selectedExitTimeCreteria; }
			set
			{
				_selectedExitTimeCreteria = value;
				OnPropertyChanged(() => SelectedExitTimeCreteria);
				ExitTimeTypes = GetTimeTypes(value);
			}
		}

		ObservableCollection<CardTimeItem> _enterTimeTypes;
		public ObservableCollection<CardTimeItem> EnterTimeTypes
		{
			get { return _enterTimeTypes; }
			set
			{
				_enterTimeTypes = value;
				OnPropertyChanged(() => EnterTimeTypes);
			}
		}

		CardTimeItem _selectedEnterTimeType;
		public CardTimeItem SelectedEnterTimeType
		{
			get { return _selectedEnterTimeType; }
			set
			{
				_selectedEnterTimeType = value;
				OnPropertyChanged(() => SelectedEnterTimeType);
			}
		}

		ObservableCollection<CardTimeItem> _exitTimeTypes;
		public ObservableCollection<CardTimeItem> ExitTimeTypes
		{
			get { return _exitTimeTypes; }
			set
			{
				_exitTimeTypes = value;
				OnPropertyChanged(() => ExitTimeTypes);
			}
		}

		CardTimeItem _selectedExitTimeType;
		public CardTimeItem SelectedExitTimeType
		{
			get { return _selectedExitTimeType; }
			set
			{
				_selectedExitTimeType = value;
				OnPropertyChanged(() => SelectedExitTimeType);
			}
		}

		ObservableCollection<CardTimeItem> GetTimeTypes(IntervalType intervalType)
		{
			var result = new ObservableCollection<CardTimeItem>();
			//result.Add(new CardTimeItem(IntervalType.Time, 0, "<Никогда>"));
			//result.Add(new CardTimeItem(IntervalType.Time, 1, "<Всегда>"));
			if (intervalType != null)
			{
				switch (intervalType)
				{
					//case IntervalType.Time:
					//    foreach (var dayInterval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.DayIntervals)
					//        result.Add(new CardTimeItem(IntervalType.Time, dayInterval.ID, dayInterval.Name));
					//    break;
					case IntervalType.Weekly:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
							result.Add(new CardTimeItem(IntervalType.Weekly, interval.ID, interval.Name));
						break;
					case IntervalType.SlideDay:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.SlideDayIntervals)
							result.Add(new CardTimeItem(IntervalType.SlideDay, interval.ID, interval.Name));
						break;
					case IntervalType.SlideWeekly:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.SlideWeeklyIntervals)
							result.Add(new CardTimeItem(IntervalType.SlideWeekly, interval.ID, interval.Name));
						break;
				};
			};
			return result;
		}
	}

	public class CardTimeItem
	{
		public CardTimeItem(IntervalType scheduleType, int scheduleID, string name)
		{
			ScheduleType = scheduleType;
			ScheduleID = scheduleID;
			Name = name;
		}

		public IntervalType ScheduleType { get; private set; }
		public int ScheduleID { get; private set; }
		public string Name { get; private set; }
	}
}
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

			TimeCreterias = new ObservableCollection<IntervalTypeViewModel>();
			TimeCreterias.Add(new IntervalTypeViewModel(IntervalType.Weekly));
			//foreach (IntervalType item in Enum.GetValues(typeof(IntervalType)))
			//{
			//	TimeCreterias.Add(new IntervalTypeViewModel(item));
			//}

			if (CardDoors == null)
				CardDoors = new List<CardDoor>();

			var cardDoor = CardDoors.FirstOrDefault(x => x.DoorUID == door.UID);
			if (cardDoor != null)
			{
				_isChecked = true;
				IsAntiPassback = cardDoor.IsAntiPassback;
				IsComission = cardDoor.IsComission;
				SelectedEnterTimeCreteria = TimeCreterias.FirstOrDefault(x => x.IntervalType == cardDoor.EnterIntervalType);
				SelectedExitTimeCreteria = TimeCreterias.FirstOrDefault(x => x.IntervalType == cardDoor.ExitIntervalType);
				SelectedEnterTimeType = EnterTimeTypes.FirstOrDefault(x => x.ScheduleID == cardDoor.EnterIntervalID);
				SelectedExitTimeType = ExitTimeTypes.FirstOrDefault(x => x.ScheduleID == cardDoor.ExitIntervalID);
			}
			else
			{
				SelectedEnterTimeCreteria = TimeCreterias.FirstOrDefault();
				SelectedExitTimeCreteria = TimeCreterias.FirstOrDefault();
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

		bool _isAntiPassback;
		public bool IsAntiPassback
		{
			get { return _isAntiPassback; }
			set
			{
				_isAntiPassback = value;
				OnPropertyChanged(() => IsAntiPassback);
			}
		}

		bool _isComission;
		public bool IsComission
		{
			get { return _isComission; }
			set
			{
				_isComission = value;
				OnPropertyChanged(() => IsComission);
			}
		}

		public ObservableCollection<IntervalTypeViewModel> TimeCreterias { get; private set; }

		IntervalTypeViewModel _selectedEnterTimeCreteria;
		public IntervalTypeViewModel SelectedEnterTimeCreteria
		{
			get { return _selectedEnterTimeCreteria; }
			set
			{
				_selectedEnterTimeCreteria = value;
				OnPropertyChanged(() => SelectedEnterTimeCreteria);
				EnterTimeTypes = GetTimeTypes(value);
			}
		}

		IntervalTypeViewModel _selectedExitTimeCreteria;
		public IntervalTypeViewModel SelectedExitTimeCreteria
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

		ObservableCollection<CardTimeItem> GetTimeTypes(IntervalTypeViewModel timeCriteria)
		{
			var result = new ObservableCollection<CardTimeItem>();
			if (timeCriteria != null)
			{
				switch (timeCriteria.IntervalType)
				{
					case IntervalType.Time:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.TimeIntervals)
							result.Add(new CardTimeItem(IntervalType.Time, interval.ID, interval.Name));
						break;
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

	public class IntervalTypeViewModel
	{
		public IntervalTypeViewModel(IntervalType intervalType)
		{
			IntervalType = intervalType;
		}

		public IntervalType IntervalType { get; private set; }

		public string Description
		{
			get { return IntervalType.ToDescription(); }
		}
	}

	public class CardTimeItem
	{
		public CardTimeItem(IntervalType scheduleType, int scheduleID, string name)
		{
			ScheduleID = scheduleID;
			ScheduleType = scheduleType;
			Name = name;
		}

		public string Name { get; private set; }
		public IntervalType ScheduleType { get; private set; }
		public int ScheduleID { get; private set; }
	}
}
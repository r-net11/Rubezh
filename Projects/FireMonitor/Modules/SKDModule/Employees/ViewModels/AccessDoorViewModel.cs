using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class AccessDoorViewModel : TreeNodeViewModel<AccessDoorViewModel>
	{
		public Door Door { get; private set; }
		public List<CardDoor> CardDoors { get; private set; }
		Action<AccessDoorViewModel> OnChecked;

		public AccessDoorViewModel(Door door, List<CardDoor> cardDoors, Action<AccessDoorViewModel> onChecked)
		{
			Door = door;
			CardDoors = cardDoors;
			OnChecked = onChecked;

			TimeCreterias = new ObservableCollection<IntervalTypeViewModel>();
			foreach (IntervalType item in Enum.GetValues(typeof(IntervalType)))
			{
				TimeCreterias.Add(new IntervalTypeViewModel(item));
			}
			SelectedTimeCreteria = TimeCreterias.FirstOrDefault();

			if (CardDoors == null)
				CardDoors = new List<CardDoor>();

			var cardDoor = CardDoors.FirstOrDefault(x => x.DoorUID == door.UID);
			if (cardDoor != null)
			{
				_isChecked = true;
				IsAntiPassback = cardDoor.IsAntiPassback;
				IsComission = cardDoor.IsComission;
				SelectedTimeCreteria = TimeCreterias.FirstOrDefault(x => x.IntervalType == cardDoor.IntervalType);
				SelectedTimeType = TimeTypes.FirstOrDefault(x => x.UID == cardDoor.IntervalUID);
			}
		}


		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
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
				OnPropertyChanged("IsAntiPassback");
			}
		}

		bool _isComission;
		public bool IsComission
		{
			get { return _isComission; }
			set
			{
				_isComission = value;
				OnPropertyChanged("IsComission");
			}
		}

		public ObservableCollection<IntervalTypeViewModel> TimeCreterias { get; private set; }

		IntervalTypeViewModel _selectedTimeCreteria;
		public IntervalTypeViewModel SelectedTimeCreteria
		{
			get { return _selectedTimeCreteria; }
			set
			{
				_selectedTimeCreteria = value;
				OnPropertyChanged(() => SelectedTimeCreteria);
				TimeTypes = new ObservableCollection<CardTimeItem>();
				switch (value.IntervalType)
				{
					case IntervalType.Time:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.TimeIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case IntervalType.Weekly:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case IntervalType.SlideDay:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.SlideDayIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case IntervalType.SlideWeekly:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.SlideWeeklyIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;
				}
				SelectedTimeType = TimeTypes.FirstOrDefault();
			}
		}

		ObservableCollection<CardTimeItem> _timeTypes;
		public ObservableCollection<CardTimeItem> TimeTypes
		{
			get { return _timeTypes; }
			set
			{
				_timeTypes = value;
				OnPropertyChanged("TimeTypes");
			}
		}

		CardTimeItem _selectedTimeType;
		public CardTimeItem SelectedTimeType
		{
			get { return _selectedTimeType; }
			set
			{
				_selectedTimeType = value;
				OnPropertyChanged("SelectedTimeType");

			}
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
		public CardTimeItem(int id, string name)
		{
			ID = id;
			Name = name;
		}

		public int ID { get; set; }
		public Guid UID { get; set; }
		public string Name { get; set; }
	}
}
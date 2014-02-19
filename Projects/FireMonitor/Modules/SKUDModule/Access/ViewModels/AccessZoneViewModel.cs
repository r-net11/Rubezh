using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.TreeList;
using XFiresecAPI;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class AccessZoneViewModel : TreeNodeViewModel<AccessZoneViewModel>
	{
		public SKDZone Zone { get; private set; }
		public List<CardZone> CardZones { get; private set; }

		public AccessZoneViewModel(SKDZone zone, List<CardZone> cardZones)
		{
			Zone = zone;
			CardZones = cardZones;

			CanSelect = false;
			foreach (var device in SKDManager.Devices)
			{
				if(device.OuterZoneUID == Zone.UID)
					CanSelect = true;
			}

			TimeCreterias = new ObservableCollection<IntervalTypeViewModel>();
			foreach (IntervalType item in Enum.GetValues(typeof(IntervalType)))
			{
				TimeCreterias.Add(new IntervalTypeViewModel(item));
			}
			SelectedTimeCreteria = TimeCreterias.FirstOrDefault();

			if (CardZones == null)
				CardZones = new List<CardZone>();

			var cardZone = CardZones.FirstOrDefault(x => x.ZoneUID == zone.UID);
			if (cardZone != null)
			{
				IsChecked = true;
				IsAntiPassback = cardZone.IsAntiPassback;
				IsComission = cardZone.IsComission;
				SelectedTimeCreteria = TimeCreterias.FirstOrDefault(x => x.IntervalType == cardZone.IntervalType);
				SelectedTimeType = TimeTypes.FirstOrDefault(x => x.UID == cardZone.IntervalUID);
			}
		}

		public bool CanSelect { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
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
				OnPropertyChanged(()=> SelectedTimeCreteria);
				TimeTypes = new ObservableCollection<CardTimeItem>();
				switch (value.IntervalType)
				{
					case IntervalType.Time:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case IntervalType.Weekly:
						foreach (var interval in SKDManager.SKDConfiguration.WeeklyIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case IntervalType.SlideDay:
						foreach (var interval in SKDManager.SKDConfiguration.SlideDayIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case IntervalType.SlideWeekly:
						foreach (var interval in SKDManager.SKDConfiguration.SlideWeeklyIntervals)
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
			get{ return IntervalType.ToDescription(); }
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
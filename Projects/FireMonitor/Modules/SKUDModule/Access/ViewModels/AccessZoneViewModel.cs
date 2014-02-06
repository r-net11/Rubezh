using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;
using FiresecAPI;
using XFiresecAPI;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class AccessZoneViewModel : TreeNodeViewModel<AccessZoneViewModel>
	{
		public SKDZone Zone { get; private set; }

		public AccessZoneViewModel(SKDZone zone)
		{
			Zone = zone;

			CanSelect = false;
			foreach (var device in zone.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
					CanSelect = true;
			}

			TimeCreterias = new ObservableCollection<CardTimeItem>();
			TimeCreterias.Add(new CardTimeItem(1, "Временные зоны"));
			TimeCreterias.Add(new CardTimeItem(2, "Недельные графики"));
			TimeCreterias.Add(new CardTimeItem(3, "Скользящие посуточные графики"));
			TimeCreterias.Add(new CardTimeItem(4, "Скользящие понедельные графики"));
			SelectedTimeCreteria = TimeCreterias.FirstOrDefault();
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

		public ObservableCollection<CardTimeItem> TimeCreterias { get; private set; }

		CardTimeItem _selectedTimeCreteria;
		public CardTimeItem SelectedTimeCreteria
		{
			get { return _selectedTimeCreteria; }
			set
			{
				_selectedTimeCreteria = value;
				OnPropertyChanged("SelectedTimeCreteria");
				TimeTypes = new ObservableCollection<CardTimeItem>();
				switch (value.ID)
				{
					case 1:
						foreach (var interval in SKDManager.SKDConfiguration.TimeIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case 2:
						foreach (var interval in SKDManager.SKDConfiguration.WeeklyIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case 3:
						foreach (var interval in SKDManager.SKDConfiguration.SlideDayIntervals)
						{
							TimeTypes.Add(new CardTimeItem(0, interval.Name) { UID = interval.UID });
						}
						break;

					case 4:
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
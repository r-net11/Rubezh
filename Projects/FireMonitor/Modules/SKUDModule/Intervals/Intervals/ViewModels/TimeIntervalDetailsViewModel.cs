using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		private bool _isNew;
		private NamedInterval _namedInterval;
		public TimeInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(NamedInterval namedInterval, TimeInterval timeInterval = null)
		{
			_namedInterval = namedInterval;
			if (timeInterval == null)
			{
				Title = "Новый интервал";
				_isNew = true;
				timeInterval = new TimeInterval();
			}
			else
			{
				Title = "Редактирование интервала";
				_isNew = false;
			}
			TimeInterval = timeInterval;

			AvailableTransitions = new ObservableCollection<IntervalTransitionType>(Enum.GetValues(typeof(IntervalTransitionType)).OfType<IntervalTransitionType>());
			StartTime = timeInterval.StartTime;
			EndTime = timeInterval.EndTime;
			SelectedTransition = timeInterval.IntervalTransitionType;
		}

		private DateTime _startTime;
		public DateTime StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		private DateTime _endTime;
		public DateTime EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		private ObservableCollection<IntervalTransitionType> _availableTransitions;
		public ObservableCollection<IntervalTransitionType> AvailableTransitions
		{
			get { return _availableTransitions; }
			set
			{
				_availableTransitions = value;
				OnPropertyChanged(() => AvailableTransitions);
			}
		}

		private IntervalTransitionType _selectedTransition;
		public IntervalTransitionType SelectedTransition
		{
			get { return _selectedTransition; }
			set
			{
				_selectedTransition = value;
				OnPropertyChanged(() => SelectedTransition);
			}
		}

		protected override bool Save()
		{
			if (!Validate())
				return false;
			TimeInterval.StartTime = StartTime;
			TimeInterval.EndTime = EndTime;
			TimeInterval.IntervalTransitionType = SelectedTransition;
			return true;
		}

		private bool Validate()
		{
			var timeIntervals = CloneNamedInterval();
			if (timeIntervals[0].IntervalTransitionType == IntervalTransitionType.NextDay)
			{
				MessageBoxService.ShowWarning("Последовательность интервалов не может начинаться со следующего дня");
				return false;
			}
			var currentDateTime = DateTime.MinValue;
			foreach (var timeInterval in timeIntervals)
			{
				var startTime = timeInterval.StartTime;
				if (timeInterval.IntervalTransitionType == IntervalTransitionType.NextDay)
					startTime = startTime.AddDays(1);
				var endTime = timeInterval.EndTime;
				if (timeInterval.IntervalTransitionType != IntervalTransitionType.Day)
					endTime = endTime.AddDays(1);
				if (startTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Последовательность интервалов не должна быть пересекающейся");
					return false;
				}
				if (startTime == currentDateTime)
				{
					MessageBoxService.ShowWarning("Пауза между интервалами не должна быть нулевой");
					return false;
				}
				currentDateTime = startTime;
				if (endTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Последовательность интервалов не должна быть пересекающейся");
					return false;
				}
				if (endTime == currentDateTime)
				{
					MessageBoxService.ShowWarning("Интервал не может иметь нулевую длительность");
					return false;
				}
				currentDateTime = endTime;
			}
			return true;
		}
		private List<TimeInterval> CloneNamedInterval()
		{
			var timeIntervals = new List<TimeInterval>();
			foreach (var timeInterval in _namedInterval.TimeIntervals)
			{
				var clonedTimeInterval = new TimeInterval()
				{
					UID = timeInterval.UID,
					StartTime = timeInterval.StartTime,
					EndTime = timeInterval.EndTime,
					IntervalTransitionType = timeInterval.IntervalTransitionType
				};
				timeIntervals.Add(clonedTimeInterval);
			}
			if (_isNew)
			{
				var newEmployeeTimeInterval = new TimeInterval()
				{
					StartTime = StartTime,
					EndTime = EndTime,
					IntervalTransitionType = SelectedTransition
				};
				timeIntervals.Add(newEmployeeTimeInterval);
			}
			else
			{
				var deitingTimeInterval = timeIntervals.FirstOrDefault(x => x.UID == TimeInterval.UID);
				if (deitingTimeInterval != null)
				{
					deitingTimeInterval.StartTime = StartTime;
					deitingTimeInterval.EndTime = EndTime;
					deitingTimeInterval.IntervalTransitionType = SelectedTransition;
				}
			}
			return timeIntervals;
		}
	}
}
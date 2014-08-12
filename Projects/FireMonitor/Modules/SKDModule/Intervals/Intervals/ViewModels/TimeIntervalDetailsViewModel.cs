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
		bool _isNew;
		NamedInterval _namedInterval;
		public TimeInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(NamedInterval namedInterval, TimeInterval timeInterval = null)
		{
			_namedInterval = namedInterval;
			if (timeInterval == null)
			{
				Title = "Новый интервал";
				_isNew = true;
				timeInterval = new TimeInterval()
				{
					NamedIntervalUID = namedInterval.UID,
				};
			}
			else
			{
				Title = "Редактирование интервала";
				_isNew = false;
			}
			TimeInterval = timeInterval;

			AvailableTransitions = new ObservableCollection<IntervalTransitionType>(Enum.GetValues(typeof(IntervalTransitionType)).OfType<IntervalTransitionType>());
			BeginTime = timeInterval.BeginTime;
			EndTime = timeInterval.EndTime;
			SelectedTransition = timeInterval.IntervalTransitionType;
		}

		TimeSpan _beginTime;
		public TimeSpan BeginTime
		{
			get { return _beginTime; }
			set
			{
				_beginTime = value;
				OnPropertyChanged(() => BeginTime);
			}
		}

		TimeSpan _endTime;
		public TimeSpan EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		ObservableCollection<IntervalTransitionType> _availableTransitions;
		public ObservableCollection<IntervalTransitionType> AvailableTransitions
		{
			get { return _availableTransitions; }
			set
			{
				_availableTransitions = value;
				OnPropertyChanged(() => AvailableTransitions);
			}
		}

		IntervalTransitionType _selectedTransition;
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
			TimeInterval.BeginTime = BeginTime;
			TimeInterval.EndTime = EndTime;
			TimeInterval.IntervalTransitionType = SelectedTransition;
			return true;
		}

		bool Validate()
		{
			var timeIntervals = CloneNamedInterval();
			var currentDateTime = new TimeSpan(0, 0, -1);
			foreach (var timeInterval in timeIntervals)
			{
				var beginTime = timeInterval.BeginTime;
				var endTime = timeInterval.EndTime;
				if (timeInterval.IntervalTransitionType != IntervalTransitionType.Day)
					endTime = endTime.Add(TimeSpan.FromDays(1));
				if (beginTime > endTime)
				{
					MessageBoxService.ShowWarning("Время окончания интервала должно быть позже времени начала");
					return false;
				}
				if (beginTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Последовательность интервалов не должна быть пересекающейся");
					return false;
				}
				if (beginTime == currentDateTime)
				{
					MessageBoxService.ShowWarning("Пауза между интервалами не должна быть нулевой");
					return false;
				}
				currentDateTime = beginTime;
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
		List<TimeInterval> CloneNamedInterval()
		{
			var timeIntervals = new List<TimeInterval>();
			foreach (var timeInterval in _namedInterval.TimeIntervals)
			{
				var clonedTimeInterval = new TimeInterval()
				{
					UID = timeInterval.UID,
					BeginTime = timeInterval.BeginTime,
					EndTime = timeInterval.EndTime,
					IntervalTransitionType = timeInterval.IntervalTransitionType,
					NamedIntervalUID = timeInterval.NamedIntervalUID,
				};
				timeIntervals.Add(clonedTimeInterval);
			}
			if (_isNew)
			{
				var newEmployeeTimeInterval = new TimeInterval()
				{
					BeginTime = BeginTime,
					EndTime = EndTime,
					IntervalTransitionType = SelectedTransition,
					NamedIntervalUID = _namedInterval.UID,
				};
				timeIntervals.Add(newEmployeeTimeInterval);
			}
			else
			{
				var deitingTimeInterval = timeIntervals.FirstOrDefault(x => x.UID == TimeInterval.UID);
				if (deitingTimeInterval != null)
				{
					deitingTimeInterval.BeginTime = BeginTime;
					deitingTimeInterval.EndTime = EndTime;
					deitingTimeInterval.IntervalTransitionType = SelectedTransition;
				}
			}
			return timeIntervals;
		}
	}
}
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
		DayInterval _namedInterval;
		public DayIntervalPart TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(DayInterval namedInterval, DayIntervalPart timeInterval = null)
		{
			_namedInterval = namedInterval;
			if (timeInterval == null)
			{
				Title = "Новый интервал";
				_isNew = true;
				timeInterval = new DayIntervalPart()
				{
					DayIntervalUID = namedInterval.UID,
				};
			}
			else
			{
				Title = "Редактирование интервала";
				_isNew = false;
			}
			TimeInterval = timeInterval;

			AvailableTransitions = new ObservableCollection<DayIntervalPartTransitionType>(Enum.GetValues(typeof(DayIntervalPartTransitionType)).OfType<DayIntervalPartTransitionType>());
			BeginTime = timeInterval.BeginTime;
			EndTime = timeInterval.EndTime;
			SelectedTransition = timeInterval.TransitionType;
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

		ObservableCollection<DayIntervalPartTransitionType> _availableTransitions;
		public ObservableCollection<DayIntervalPartTransitionType> AvailableTransitions
		{
			get { return _availableTransitions; }
			set
			{
				_availableTransitions = value;
				OnPropertyChanged(() => AvailableTransitions);
			}
		}

		DayIntervalPartTransitionType _selectedTransition;
		public DayIntervalPartTransitionType SelectedTransition
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
			TimeInterval.TransitionType = SelectedTransition;
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
				if (timeInterval.TransitionType != DayIntervalPartTransitionType.Day)
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
		List<DayIntervalPart> CloneNamedInterval()
		{
			var timeIntervals = new List<DayIntervalPart>();
			foreach (var timeInterval in _namedInterval.DayIntervalParts)
			{
				var clonedTimeInterval = new DayIntervalPart()
				{
					UID = timeInterval.UID,
					BeginTime = timeInterval.BeginTime,
					EndTime = timeInterval.EndTime,
					TransitionType = timeInterval.TransitionType,
					DayIntervalUID = timeInterval.DayIntervalUID,
				};
				timeIntervals.Add(clonedTimeInterval);
			}
			if (_isNew)
			{
				var newEmployeeTimeInterval = new DayIntervalPart()
				{
					BeginTime = BeginTime,
					EndTime = EndTime,
					TransitionType = SelectedTransition,
					DayIntervalUID = _namedInterval.UID,
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
					deitingTimeInterval.TransitionType = SelectedTransition;
				}
			}
			return timeIntervals;
		}
	}
}
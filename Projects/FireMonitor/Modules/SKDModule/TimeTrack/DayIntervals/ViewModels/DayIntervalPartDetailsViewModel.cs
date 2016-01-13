using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class DayIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		bool IsNew;
		DayInterval DayInterval;
		Guid OrganisationUID;
		public DayIntervalPart DayIntervalPart { get; private set; }
		public DayIntervalPartDetailsViewModel(DayInterval dayInterval, Guid organisationUID, DayIntervalPart dayIntervalPart = null)
		{
			DayInterval = dayInterval;
			if (dayIntervalPart == null)
			{
				Title = "Новый интервал";
				IsNew = true;
				dayIntervalPart = new DayIntervalPart()
				{
					DayIntervalUID = dayInterval.UID,
				};
			}
			else
			{
				Title = "Редактирование интервала";
				IsNew = false;
			}
			DayIntervalPart = dayIntervalPart;

			OrganisationUID = organisationUID;
			AvailableTransitions = new ObservableCollection<DayIntervalPartTransitionType>(Enum.GetValues(typeof(DayIntervalPartTransitionType)).OfType<DayIntervalPartTransitionType>());
			BeginTime = dayIntervalPart.BeginTime;
			EndTime = dayIntervalPart.EndTime;
			SelectedTransition = dayIntervalPart.TransitionType;
			oldBeginTime = BeginTime;
			oldEndTime = EndTime;
			oldTransitionType = SelectedTransition;
		}
		TimeSpan oldBeginTime;
		TimeSpan oldEndTime;
		DayIntervalPartTransitionType oldTransitionType;
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
			if (!Validate() || IsIntersection())
			{
				SelectedTransition = oldTransitionType;
				BeginTime = oldBeginTime;
				EndTime = oldEndTime;
				return false;
			}
			DayIntervalPart.BeginTime = BeginTime;
			DayIntervalPart.EndTime = EndTime;
			DayIntervalPart.TransitionType = SelectedTransition;
			return true;
		}

		bool Validate()
		{
			var dayIntervalParts = CloneDayIntervalPart();
			var currentDateTime = TimeSpan.Zero;
			var firstBeginTime = dayIntervalParts.First().BeginTime;
			foreach (var dayIntervalPart in dayIntervalParts)
			{
				var beginTime = dayIntervalPart.BeginTime;
				var endTime = dayIntervalPart.EndTime;
				if (dayIntervalPart.TransitionType != DayIntervalPartTransitionType.Day)
				{
					if (endTime >= firstBeginTime)
					{
						MessageBoxService.ShowWarning("Последовательность интервалов не должна быть пересекающейся");
						return false;
					}
					endTime = endTime.Add(TimeSpan.FromDays(1));
				}
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
				if (beginTime == currentDateTime && beginTime != TimeSpan.Zero)
				{
					MessageBoxService.ShowWarning("Пауза между интервалами не должна быть нулевой");
					return false;
				}
				currentDateTime = beginTime;
				if (endTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Начало интервала не может быть раньше его окончания");
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
		List<DayIntervalPart> CloneDayIntervalPart()
		{
			var dayIntervalParts = new List<DayIntervalPart>();
			foreach (var dayIntervalPart in DayInterval.DayIntervalParts)
			{
				var clonedDayIntervalPart = new DayIntervalPart()
				{
					UID = dayIntervalPart.UID,
					BeginTime = dayIntervalPart.BeginTime,
					EndTime = dayIntervalPart.EndTime,
					TransitionType = dayIntervalPart.TransitionType,
					DayIntervalUID = dayIntervalPart.DayIntervalUID,
				};
				dayIntervalParts.Add(clonedDayIntervalPart);
			}
			if (IsNew)
			{
				var newEmployeeDayIntervalPart = new DayIntervalPart()
				{
					BeginTime = BeginTime,
					EndTime = EndTime,
					TransitionType = SelectedTransition,
					DayIntervalUID = DayInterval.UID,
				};
				dayIntervalParts.Add(newEmployeeDayIntervalPart);
			}
			else
			{
				var deitingDayIntervalPart = dayIntervalParts.FirstOrDefault(x => x.UID == DayIntervalPart.UID);
				if (deitingDayIntervalPart != null)
				{
					deitingDayIntervalPart.BeginTime = BeginTime;
					deitingDayIntervalPart.EndTime = EndTime;
					deitingDayIntervalPart.TransitionType = SelectedTransition;
				}
			}
			dayIntervalParts = dayIntervalParts.OrderBy(item => item.BeginTime).ToList();
			return dayIntervalParts;


		}
		bool IsIntersection()
		{
			var scheduleSchemesViewModel = ScheduleSchemesViewModel.Current;
			scheduleSchemesViewModel.Initialize(new ScheduleSchemeFilter());
			scheduleSchemesViewModel.ReloadDayIntervals();
			var scheduleSchemes = scheduleSchemesViewModel.Organisations.FirstOrDefault(x => x.UID == OrganisationUID).Children;
			{
				foreach (var sheduleScheme in scheduleSchemes)
				{
					sheduleScheme.Initialize();
					var dayIntervals = sheduleScheme.SheduleDayIntervals;
					for (int i = 0; i < dayIntervals.Count; i++)
					{
						if (dayIntervals[i].SelectedDayInterval.UID == DayIntervalPart.DayIntervalUID)
						{
							if (SelectedTransition == DayIntervalPartTransitionType.Night && i != dayIntervals.Count - 1)
							{
								var tomorrowIntervalPart = dayIntervals[i + 1].SelectedDayInterval.DayIntervalParts.FirstOrDefault(x => x.Number == 1);
								if (tomorrowIntervalPart != null && EndTime >= tomorrowIntervalPart.BeginTime)
								{
									MessageBoxService.ShowWarning("Графики. Интервал имеет пересечение с графиком другого дня.");
									return true;
								}
							}
							if (i != 0)
							{
								var yesterdayIntervalPart = dayIntervals[i - 1].SelectedDayInterval.DayIntervalParts.FirstOrDefault(x => x.TransitionType == DayIntervalPartTransitionType.Night);
								if (yesterdayIntervalPart != null && BeginTime <= yesterdayIntervalPart.EndTime)
								{
									MessageBoxService.ShowWarning("Графики. Интервал имеет пересечение с графиком другого дня.");
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
	}
}
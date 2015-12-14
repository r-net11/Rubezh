using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		#region Fields
		readonly bool _isNew;
		readonly DayInterval _dayInterval;
		#endregion

		#region Properties
		public DayIntervalPart DayIntervalPart { get; private set; }

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

		#endregion

		#region Constructors
		public DayIntervalPartDetailsViewModel(DayInterval dayInterval, DayIntervalPart dayIntervalPart = null)
		{
			_dayInterval = dayInterval;
			if (dayIntervalPart == null)
			{
				Title = "Новый интервал";
				_isNew = true;
				dayIntervalPart = new DayIntervalPart
				{
					DayIntervalUID = dayInterval.UID,
				};
			}
			else
			{
				Title = "Редактирование интервала";
				_isNew = false;
			}
			DayIntervalPart = dayIntervalPart;

			BeginTime = dayIntervalPart.BeginTime;
			EndTime = dayIntervalPart.EndTime;
		}
		#endregion

		#region Commands
		protected override bool Save()
		{
			if (!Validate())
				return false;

			DayIntervalPart.BeginTime = BeginTime;
			DayIntervalPart.EndTime = EndTime;
			DayIntervalPart.TransitionType = BeginTime < EndTime ? DayIntervalPartTransitionType.Day : DayIntervalPartTransitionType.Night;
			return DayIntervalPartHelper.Save(DayIntervalPart, _isNew, _dayInterval.Name);
		}
		#endregion

		#region Methods
		bool Validate()
		{
			var dayIntervalParts = CloneDayIntervalPart();

			var currentDateTime = TimeSpan.Zero;
			foreach (var dayIntervalPart in dayIntervalParts)
			{
				if (dayIntervalPart.BeginTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Интервалы должны идти последовательно");
					return false;
				}
				currentDateTime = dayIntervalPart.BeginTime;
			}

			currentDateTime = TimeSpan.Zero;
			foreach (var dayIntervalPart in dayIntervalParts)
			{
				var beginTime = dayIntervalPart.BeginTime;
				var endTime = dayIntervalPart.EndTime;
				if (dayIntervalPart.TransitionType != DayIntervalPartTransitionType.Day)
					endTime = endTime.Add(TimeSpan.FromDays(1));
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
			var dayIntervalParts = _dayInterval.DayIntervalParts
				.Select(dayIntervalPart => new DayIntervalPart
				{
					UID = dayIntervalPart.UID,
					BeginTime = dayIntervalPart.BeginTime,
					EndTime = dayIntervalPart.EndTime,
					TransitionType = dayIntervalPart.TransitionType,
					DayIntervalUID = dayIntervalPart.DayIntervalUID,
				})
				.ToList();

			if (_isNew)
			{
				var newEmployeeDayIntervalPart = new DayIntervalPart
				{
					BeginTime = BeginTime,
					EndTime = EndTime,
					DayIntervalUID = _dayInterval.UID,
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
				}
			}
			return dayIntervalParts;
		}
		#endregion
	}
}
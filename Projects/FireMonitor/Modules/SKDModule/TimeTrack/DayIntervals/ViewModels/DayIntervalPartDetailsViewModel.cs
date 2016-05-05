using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EntitiesValidation;
using StrazhAPI;
using StrazhAPI.SKD;
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
			DayIntervalPart.BeginTime = BeginTime;
			DayIntervalPart.EndTime = EndTime;
			DayIntervalPart.TransitionType = BeginTime < EndTime ? DayIntervalPartTransitionType.Day : DayIntervalPartTransitionType.Night;

			var validationResult = Validate();
			if (validationResult.HasError)
			{
				MessageBoxService.ShowWarning(validationResult.Error);
				return false;
			}

			return DayIntervalPartHelper.Save(DayIntervalPart, _isNew, _dayInterval.Name);
		}
		#endregion

		#region Methods
		private OperationResult<bool> Validate()
		{
			return _isNew ? ValidateAdding() : ValidateEditing();
		}

		private OperationResult<bool> ValidateAdding()
		{
			// Время начала интервала равно времени окончания?
			var validationesult = DayIntervalPartValidator.ValidateNewDayIntervalPartLength(DayIntervalPart);
			if (validationesult.HasError)
				return validationesult;

			// Добавляемый интервал заканчивается ранее, чем уже добавленные интервалы?
			validationesult = DayIntervalPartValidator.ValidateNewDayIntervalPartOrder(DayIntervalPart, _dayInterval.DayIntervalParts);
			if (validationesult.HasError)
				return validationesult;

			// Добавляемый интервал пересекается с уже добавленными интервалами?
			validationesult = DayIntervalPartValidator.ValidateNewDayIntervalPartIntersection(DayIntervalPart, _dayInterval.DayIntervalParts);
			if (validationesult.HasError)
				return validationesult;

			return new OperationResult<bool>(true);
		}

		private OperationResult<bool> ValidateEditing()
		{
			// Время начала интервала равно времени окончания?
			var validationesult = DayIntervalPartValidator.ValidateNewDayIntervalPartLength(DayIntervalPart);
			if (validationesult.HasError)
				return validationesult;

			var otherDayIntervalParts = _dayInterval.DayIntervalParts.Where(dayIntervalPart => dayIntervalPart.UID != DayIntervalPart.UID);

			// Редактируемый интервал заканчивается ранее, чем остальные интервалы?
			//validationesult = DayIntervalPartValidator.ValidateNewDayIntervalPartOrder(DayIntervalPart, otherDayIntervalParts);
			//if (validationesult.HasError)
			//	return validationesult;

			// Редактируемый интервал пересекается с остальными интервалами?
			validationesult = DayIntervalPartValidator.ValidateNewDayIntervalPartIntersection(DayIntervalPart, otherDayIntervalParts);
			if (validationesult.HasError)
				return validationesult;

			return new OperationResult<bool>(true);
		}
		#endregion
	}
}
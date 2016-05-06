using System;
using System.Collections.ObjectModel;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDoorDayIntervalPart DayIntervalPart { get; private set; }

		public DoorDayIntervalPartDetailsViewModel(SKDDoorDayIntervalPart dayIntervalPart = null)
		{
			Title = "Задание интервала";
			DayIntervalPart = dayIntervalPart ?? new SKDDoorDayIntervalPart() { DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD };

			MinStartTime = DateTime.MinValue.TimeOfDay;
			MaxEndTime = DateTime.MaxValue.TimeOfDay;
			StartTime = TimeSpan.FromMilliseconds(DayIntervalPart.StartMilliseconds);
			EndTime = TimeSpan.FromMilliseconds(DayIntervalPart.EndMilliseconds);
			InitAvailableDoorOpenMethods();
			SelectedDoorOpenMethod = DayIntervalPart.DoorOpenMethod;
		}

		TimeSpan _minStartTime;
		public TimeSpan MinStartTime
		{
			get { return _minStartTime; }
			set
			{
				if (_minStartTime == value)
					return;
				_minStartTime = value;
				if (StartTime < _minStartTime)
					StartTime = _minStartTime;
			}
		}

		TimeSpan _maxEndTime;
		public TimeSpan MaxEndTime
		{
			get { return _maxEndTime; }
			set
			{
				if (_maxEndTime == value)
					return;
				_maxEndTime = value;
				if (EndTime > _maxEndTime)
					EndTime = _maxEndTime;
			}
		}

		TimeSpan _startTime;
		public TimeSpan StartTime
		{
			get { return _startTime; }
			set
			{
				if (_startTime == value)
					return;
				if (value < MinStartTime)
					return;
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		TimeSpan _endTime;
		public TimeSpan EndTime
		{
			get { return _endTime; }
			set
			{
				if (_endTime == value)
					return;
				if (value > MaxEndTime)
					return;
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		public ObservableCollection<SKDDoorConfiguration_DoorOpenMethod> AvailableDoorOpenMethods { get; private set; }

		private void InitAvailableDoorOpenMethods()
		{
			AvailableDoorOpenMethods = new ObservableCollection<SKDDoorConfiguration_DoorOpenMethod>
			{
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST
			};
		}

		SKDDoorConfiguration_DoorOpenMethod _selectedDoorOpenMethod;
		/// <summary>
		/// Метод открытия двери
		/// </summary>
		public SKDDoorConfiguration_DoorOpenMethod SelectedDoorOpenMethod
		{
			get { return _selectedDoorOpenMethod; }
			set
			{
				_selectedDoorOpenMethod = value;
				OnPropertyChanged(() => SelectedDoorOpenMethod);
			}
		}

		protected override bool CanSave()
		{
			return EndTime > StartTime;
		}
		protected override bool Save()
		{
			DayIntervalPart.StartMilliseconds = StartTime.TotalMilliseconds;
			DayIntervalPart.EndMilliseconds = EndTime.TotalMilliseconds;
			DayIntervalPart.DoorOpenMethod = SelectedDoorOpenMethod;
			return true;
		}
	}
}
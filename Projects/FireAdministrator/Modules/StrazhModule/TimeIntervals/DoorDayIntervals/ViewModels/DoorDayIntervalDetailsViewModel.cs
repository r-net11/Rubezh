using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		private IEnumerable<DoorDayIntervalViewModel> _dayIntervalViewModels;
		public SKDDoorDayInterval DayInterval { get; private set; }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="otherDayIntervals">Коллекция ViewModel'ей для ранее добавленных дневных интервалов времени</param>
		/// <param name="dayInterval">Редактируемый дневной интервал времени. Равен null для вновь добавляемого</param>
		public DoorDayIntervalDetailsViewModel(IEnumerable<DoorDayIntervalViewModel> otherDayIntervals, SKDDoorDayInterval dayInterval = null)
		{
			_dayIntervalViewModels = otherDayIntervals;

			if (dayInterval == null)
			{
				Title = "Создание нового дневного расписания";
				DayInterval = new SKDDoorDayInterval()
				{
					Name = "Новое дневное расписание",
				};
				DayInterval.DayIntervalParts.Add(new SKDDoorDayIntervalPart()
				{
					StartMilliseconds = 0,
					EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds,
					DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD
				});
			}
			else
			{
				Title = string.Format("Свойства дневного расписания: {0}", dayInterval.Name);
				DayInterval = dayInterval;
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = DayInterval.Name;
			Description = DayInterval.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			// Проверяем что заданное название дневного графика замка не совпадает с названием других дневных графиков замка
			if (_dayIntervalViewModels.FirstOrDefault(x => x.Name == Name) != null)
			{
				MessageBoxService.ShowWarning("Дневное расписание замка с таким названием уже существует");
				return false;
			}
			DayInterval.Name = Name;
			DayInterval.Description = Description;
			return true;
		}
	}
}
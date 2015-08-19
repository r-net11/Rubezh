using System;
using FiresecAPI.SKD;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDoorDayInterval DayInterval { get; private set; }

		public DoorDayIntervalDetailsViewModel(SKDDoorDayInterval dayInterval = null)
		{
			if (dayInterval == null)
			{
				Title = "Создание нового дневного графика";
				DayInterval = new SKDDoorDayInterval()
				{
					Name = "Новый дневной график",
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
				Title = string.Format("Свойства дневного графика: {0}", dayInterval.Name);
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
			if (Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard
				|| Name == TimeIntervalsConfiguration.PredefinedIntervalNamePassword
				|| Name == TimeIntervalsConfiguration.PredefinedIntervalNameCardAndPassword)
			{
				MessageBoxService.ShowWarning("Запрещенное назваине");
				return false;
			}
			DayInterval.Name = Name;
			DayInterval.Description = Description;
			return true;
		}
	}
}
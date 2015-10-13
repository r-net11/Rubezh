using RubezhAPI.GK;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class DayScheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKDaySchedule DaySchedule { get; private set; }

		public DayScheduleDetailsViewModel(GKDaySchedule daySchedule = null)
		{
			if (daySchedule == null)
			{
				Title = "Создание нового дневного графика";
				DaySchedule = new GKDaySchedule()
				{
					Name = "Новый дневной график",
				};
			}
			else
			{
				Title = string.Format("Свойства дневного графика: {0}", daySchedule.Name);
				DaySchedule = daySchedule;
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = DaySchedule.Name;
			Description = DaySchedule.Description;
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
			if (Name == "<Никогда>" || Name == "<Всегда>")
			{
				MessageBoxService.ShowWarning("Запрещенное назваине");
				return false;
			}
			DaySchedule.Name = Name;
			DaySchedule.Description = Description;
			return true;
		}
	}
}
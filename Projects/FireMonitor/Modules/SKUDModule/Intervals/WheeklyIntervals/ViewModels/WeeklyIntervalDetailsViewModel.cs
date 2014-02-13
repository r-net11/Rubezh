using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		WeeklyIntervalsViewModel WeeklyIntervalsViewModel;
		bool IsNew;
		public EmployeeWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalDetailsViewModel(WeeklyIntervalsViewModel weeklyIntervalsViewModel, EmployeeWeeklyInterval weeklyInterval = null)
		{
			WeeklyIntervalsViewModel = weeklyIntervalsViewModel;
			if (weeklyInterval == null)
			{
				Title = "Новый недельный график";
				weeklyInterval = new EmployeeWeeklyInterval()
				{
					Name = "Недельный график",
				};
				foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
				{
					var neverTimeInterval = (EmployeeTimeInterval)null; // SKDManager.SKDConfiguration.TimeIntervals.FirstOrDefault(x => x.Name == "Никогда");
					if (neverTimeInterval != null)
					{
						weeklyIntervalPart.TimeIntervalUID = neverTimeInterval.UID;
					}
				}
			}
			else
			{
				Title = "Редактирование недельногор графика";
			}
			WeeklyInterval = weeklyInterval;
			Name = WeeklyInterval.Name;
			Description = WeeklyInterval.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Доступ запрещен" && Name != "Доступ разрешен";
		}

		protected override bool Save()
		{
			if (WeeklyIntervalsViewModel.WeeklyIntervals.Any(x => x.WeeklyInterval.Name == Name && x.WeeklyInterval.UID != WeeklyInterval.UID))
			{
				MessageBoxService.ShowWarning("Название интервала совпадает с введенным ранее");
				return false;
			}

			WeeklyInterval.Name = Name;
			WeeklyInterval.Description = Description;
			return true;
		}
	}
}
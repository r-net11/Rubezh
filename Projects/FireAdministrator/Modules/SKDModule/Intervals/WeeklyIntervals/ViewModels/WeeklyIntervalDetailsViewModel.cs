using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalDetailsViewModel(SKDWeeklyInterval weeklyInterval = null)
		{
			if (weeklyInterval == null)
			{
				Title = "Новый понедельный график";
				weeklyInterval = new SKDWeeklyInterval()
				{
					Name = "Понедельный график",
				};
				foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
				{
					var neverTimeInterval = SKDManager.TimeIntervalsConfiguration.TimeIntervals.FirstOrDefault(x => x.Name == "Никогда");
					if (neverTimeInterval != null)
					{
						weeklyIntervalPart.TimeIntervalID = neverTimeInterval.ID;
					}
				}
			}
			else
			{
				Title = "Редактирование понедельногор графика";
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
			WeeklyInterval.Name = Name;
			WeeklyInterval.Description = Description;
			return true;
		}
	}
}
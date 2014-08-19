using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemeDetailsViewModel : SaveCancelDialogViewModel
	{
		FiresecAPI.SKD.Organisation Organisation;
		ScheduleSchemeType ScheduleSchemeType;
		public ScheduleScheme ScheduleScheme { get; private set; }

		public ScheduleSchemeDetailsViewModel(FiresecAPI.SKD.Organisation organisation, ScheduleSchemeType scheduleSchemeType, ScheduleScheme scheduleScheme = null)
		{
			Organisation = organisation;
			ScheduleSchemeType = scheduleSchemeType;
			var dayCount = 0;
			var name = string.Empty;
			switch (scheduleSchemeType)
			{
				case ScheduleSchemeType.Month:
					name = "Месячный график работы";
					Title = scheduleScheme == null ? "Новый месячный график работы" : "Редактирование месячного графика работы";
					dayCount = 31;
					break;
				case ScheduleSchemeType.SlideDay:
					name = "Суточный график работы";
					Title = scheduleScheme == null ? "Новый суточный график работы" : "Редактирование скользящего посуточного графика работы";
					dayCount = 1;
					break;
				case ScheduleSchemeType.Week:
					name = "Недельный график работы";
					Title = scheduleScheme == null ? "Новый недельный график работы" : "Редактирование недельногор графика работы";
					dayCount = 7;
					break;
			}
			if (scheduleScheme == null)
			{
				scheduleScheme = new ScheduleScheme()
				{
					OrganisationUID = Organisation.UID,
					Type = scheduleSchemeType,
					Name = name,
				};
				for (int i = 0; i < dayCount; i++)
					scheduleScheme.DayIntervals.Add(new ScheduleDayInterval()
					{
						Number = i,
						ScheduleSchemeUID = scheduleScheme.UID,
						DayIntervalUID = Guid.Empty,
					});
			}
			ScheduleScheme = scheduleScheme;
			Name = ScheduleScheme.Name;
			Description = ScheduleScheme.Description;
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
			ScheduleScheme.Name = Name;
			ScheduleScheme.Description = Description;
			return ScheduleSchemaHelper.Save(ScheduleScheme);
		}
	}
}
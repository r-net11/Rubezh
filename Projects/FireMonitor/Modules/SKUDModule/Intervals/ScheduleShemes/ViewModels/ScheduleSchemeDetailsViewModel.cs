using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public class ScheduleSchemeDetailsViewModel : SaveCancelDialogViewModel
	{
		private OrganisationScheduleSchemasViewModel _organisation;
		public ScheduleScheme ScheduleSchema { get; private set; }

		public ScheduleSchemeDetailsViewModel(OrganisationScheduleSchemasViewModel organisation, ScheduleScheme scheduleScheme = null)
		{
			_organisation = organisation;
			var dayCount = 0;
			var name = string.Empty;
			switch (organisation.Type)
			{
				case ScheduleSchemeType.Month:
					name = "Месячный график работы";
					Title = scheduleScheme == null ? "Новый месячный график работы" : "Редактирование месячного графика работы";
					dayCount = 31;
					break;
				case ScheduleSchemeType.SlideDay:
					name = "Скользящий посуточный график работы";
					Title = scheduleScheme == null ? "Новый скользящий посуточный график работы" : "Редактирование скользящего посуточного графика работы";
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
					OrganisationUID = _organisation.Organisation.UID,
					Type = _organisation.Type,
					Name = name,
				};
				for (int i = 0; i < dayCount; i++)
					scheduleScheme.DayIntervals.Add(new DayInterval()
					{
						Number = i,
						ScheduleSchemeUID = scheduleScheme.UID,
						NamedIntervalUID = Guid.Empty,
					});
			}
			ScheduleSchema = scheduleScheme;
			Name = ScheduleSchema.Name;
			Description = ScheduleSchema.Description;
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
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
			if (_organisation.ViewModels.Any(x => x.Model.Name == Name && x.Model.UID != ScheduleSchema.UID))
			{
				MessageBoxService.ShowWarning("Название графика совпадает с введенным ранее");
				return false;
			}
			ScheduleSchema.Name = Name;
			ScheduleSchema.Description = Description;
			return true;
		}
	}
}
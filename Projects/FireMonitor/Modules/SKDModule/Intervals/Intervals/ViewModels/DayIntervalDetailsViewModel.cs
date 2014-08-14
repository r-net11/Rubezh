using System;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		FiresecAPI.SKD.Organisation Organisation;
		public DayInterval DayInterval { get; private set; }

		public DayIntervalDetailsViewModel(FiresecAPI.SKD.Organisation organisation, DayInterval dayInterval = null)
		{
			Organisation = organisation;
			if (dayInterval == null)
			{
				Title = "Новый дневной график";
				dayInterval = new DayInterval()
				{
					Name = "Дневной график",
					OrganisationUID = organisation.UID,
				};
				dayInterval.DayIntervalParts.Add(new DayIntervalPart() { BeginTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), DayIntervalUID = dayInterval.UID });
			}
			else
				Title = "Редактирование дневного графика";
			DayInterval = dayInterval;
			Name = DayInterval.Name;
			Description = DayInterval.Description;
			ConstantSlideTime = DayInterval.SlideTime;
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

		TimeSpan _constantSlideTime;
		public TimeSpan ConstantSlideTime
		{
			get { return _constantSlideTime; }
			set
			{
				_constantSlideTime = value;
				OnPropertyChanged(() => ConstantSlideTime);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Всегда" && Name != "Никогда";
		}
		protected override bool Save()
		{
			DayInterval.Name = Name;
			DayInterval.Description = Description;
			DayInterval.SlideTime = ConstantSlideTime;
			return DayIntervalHelper.Save(DayInterval);
		}
	}
}
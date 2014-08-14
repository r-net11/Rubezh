using System;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class NamedIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		FiresecAPI.SKD.Organisation Organisation;
		public DayInterval NamedInterval { get; private set; }

		public NamedIntervalDetailsViewModel(FiresecAPI.SKD.Organisation organisation, DayInterval namedInterval = null)
		{
			Organisation = organisation;
			if (namedInterval == null)
			{
				Title = "Новый дневной график";
				namedInterval = new DayInterval()
				{
					Name = "Дневной график",
					OrganisationUID = organisation.UID,
				};
				namedInterval.DayIntervalParts.Add(new DayIntervalPart() { BeginTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), DayIntervalUID = namedInterval.UID });
			}
			else
				Title = "Редактирование дневного графика";
			NamedInterval = namedInterval;
			Name = NamedInterval.Name;
			Description = NamedInterval.Description;
			ConstantSlideTime = NamedInterval.SlideTime;
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
			NamedInterval.Name = Name;
			NamedInterval.Description = Description;
			NamedInterval.SlideTime = ConstantSlideTime;
			return DayIntervalHelper.Save(NamedInterval);
		}
	}
}
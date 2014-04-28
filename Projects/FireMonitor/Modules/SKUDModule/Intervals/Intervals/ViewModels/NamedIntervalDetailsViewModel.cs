using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class NamedIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		private FiresecAPI.Organisation Organisation;
		public NamedInterval NamedInterval { get; private set; }

		public NamedIntervalDetailsViewModel(FiresecAPI.Organisation organisation, NamedInterval namedInterval = null)
		{
			Organisation = organisation;
			if (namedInterval == null)
			{
				Title = "Новый именованный интервал";
				namedInterval = new NamedInterval()
				{
					Name = "Именованный интервал",
					OrganisationUID = organisation.UID,
				};
				namedInterval.TimeIntervals.Add(new TimeInterval() { BeginTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), NamedIntervalUID = namedInterval.UID });
			}
			else
				Title = "Редактирование именованного интервала";
			NamedInterval = namedInterval;
			Name = NamedInterval.Name;
			Description = NamedInterval.Description;
			ConstantSlideTime = NamedInterval.SlideTime;
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

		private TimeSpan _constantSlideTime;
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
			return NamedIntervalHelper.Save(NamedInterval);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SkudModule.ViewModels
{
	public class SlideWeekIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDSlideWeekInterval SlideWeekInterval { get; private set; }

		public SlideWeekIntervalDetailsViewModel(SKDSlideWeekInterval slideWeekInterval = null)
		{
			if (slideWeekInterval == null)
			{
				Title = "Новый скользящий понедельный график";
				slideWeekInterval = new SKDSlideWeekInterval()
				{
					Name = "Скользящий понедельный график"
				};
			}
			else
			{
				Title = "Редактирование скользящего понедельного графика";
			}
			SlideWeekInterval = slideWeekInterval;
			Name = SlideWeekInterval.Name;
			Description = SlideWeekInterval.Description;
			StartDate = SlideWeekInterval.StartDate;
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

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged("StartDate");
			}
		}

		protected override bool Save()
		{
			SlideWeekInterval.Name = Name;
			SlideWeekInterval.Description = Description;
			SlideWeekInterval.StartDate = StartDate;
			return true;
		}
	}
}
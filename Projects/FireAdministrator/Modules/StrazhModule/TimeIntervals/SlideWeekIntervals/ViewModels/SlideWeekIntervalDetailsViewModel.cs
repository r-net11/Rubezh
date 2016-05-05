using System;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class SlideWeekIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDSlideWeeklyInterval SlideWeekInterval { get; private set; }

		public SlideWeekIntervalDetailsViewModel(SKDSlideWeeklyInterval slideWeekInterval)
		{
			Title = "Редактирование скользящего понедельного графика";
			SlideWeekInterval = slideWeekInterval;
			Name = SlideWeekInterval.Name;
			Description = SlideWeekInterval.Description;
			StartDate = SlideWeekInterval.StartDate;
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

		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
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
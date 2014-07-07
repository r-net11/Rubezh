using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDSlideDayInterval SlideDayInterval { get; private set; }

		public SlideDayIntervalDetailsViewModel(SKDSlideDayInterval slideDayInterval)
		{
			Title = "Редактирование скользящего посуточного графика";
			SlideDayInterval = slideDayInterval;
			Name = SlideDayInterval.Name;
			Description = SlideDayInterval.Description;
			StartDate = SlideDayInterval.StartDate;
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
			return !string.IsNullOrEmpty(Name) && Name != "Доступ запрещен";
		}
		protected override bool Save()
		{
			SlideDayInterval.Name = Name;
			SlideDayInterval.Description = Description;
			SlideDayInterval.StartDate = StartDate;
			return true;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDSlideDayInterval SlideDayInterval { get; private set; }

		public SlideDayIntervalDetailsViewModel(SKDSlideDayInterval slideDayInterval = null)
		{
			if (slideDayInterval == null)
			{
				Title = "Новый скользящий посуточный график";
				slideDayInterval = new SKDSlideDayInterval()
				{
					Name = "Именованный интервал"
				};
			}
			else
			{
				Title = "Редактирование скользящего посуточного графика";
			}
			SlideDayInterval = slideDayInterval;
			Name = SlideDayInterval.Name;
			Description = SlideDayInterval.Description;
			StartDate = SlideDayInterval.StartDate;
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
			SlideDayInterval.Name = Name;
			SlideDayInterval.Description = Description;
			SlideDayInterval.StartDate = StartDate;
			return true;
		}
	}
}
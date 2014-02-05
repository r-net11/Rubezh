using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class TimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDTimeInterval TimeInterval { get; private set; }

		public TimeIntervalDetailsViewModel(SKDTimeInterval timeInterval = null)
		{
			if (timeInterval == null)
			{
				Title = "Новый именованный интервал";
				timeInterval = new SKDTimeInterval()
				{
					Name = "Именованный интервал"
				};
			}
			else
			{
				Title = "Редактирование именованного интервала";
			}
			TimeInterval = timeInterval;
			Name = TimeInterval.Name;
			Description = TimeInterval.Description;
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

		protected override bool Save()
		{
			TimeInterval.Name = Name;
			TimeInterval.Description = Description;
			return true;
		}
	}
}
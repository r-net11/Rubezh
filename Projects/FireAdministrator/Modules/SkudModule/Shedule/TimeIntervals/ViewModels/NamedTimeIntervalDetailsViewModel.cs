using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SkudModule.ViewModels
{
	public class NamedTimeIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public NamedSKDTimeInterval NamedTimeInterval { get; private set; }

		public NamedTimeIntervalDetailsViewModel(NamedSKDTimeInterval namedTimeInterval = null)
		{
			if (namedTimeInterval == null)
			{
				Title = "Новый именованный интервал";
				namedTimeInterval = new NamedSKDTimeInterval()
				{
					Name = "Именованный интервал"
				};
			}
			else
			{
				Title = "Редактирование именованного интервала";
			}
			NamedTimeInterval = namedTimeInterval;
			Name = NamedTimeInterval.Name;
			Description = NamedTimeInterval.Description;
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
			NamedTimeInterval.Name = Name;
			NamedTimeInterval.Description = Description;
			return true;
		}
	}
}
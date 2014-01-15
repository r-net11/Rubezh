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

		protected override bool Save()
		{
			NamedTimeInterval.Name = Name;
			return true;
		}
	}
}
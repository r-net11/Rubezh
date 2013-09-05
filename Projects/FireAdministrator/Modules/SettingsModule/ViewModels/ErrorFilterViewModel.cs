using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ErrorFilterViewModel : BaseViewModel
	{
		public ErrorFilterViewModel(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
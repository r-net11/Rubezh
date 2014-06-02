using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ModuleViewModel : BaseViewModel
	{
		public ModuleViewModel(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
	}
}
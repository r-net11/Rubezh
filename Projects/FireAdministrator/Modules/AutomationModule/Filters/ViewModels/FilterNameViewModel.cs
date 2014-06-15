using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class FilterNameViewModel : TreeNodeViewModel<FilterNameViewModel>
	{
		public FilterNameViewModel(string name, string imageSource)
		{
			AddCommand = new RelayCommand(OnAdd);
			Name = name;
			ImageSource = imageSource;
		}

		public string Name { get; private set; }

		public string ImageSource { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{

		}
	}
}
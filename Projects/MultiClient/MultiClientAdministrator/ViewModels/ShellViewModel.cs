using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace MultiClient.ViewModels
{
	public class ShellViewModel : BaseViewModel
	{
		public ShellViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			AppItems = new ObservableCollection<AppItemViewModel>();
		}

		public ObservableCollection<AppItemViewModel> AppItems { get; private set; }

		AppItemViewModel _selectedAppItem;
		public AppItemViewModel SelectedAppItem
		{
			get { return _selectedAppItem; }
			set
			{
				_selectedAppItem = value;
				OnPropertyChanged("SelectedAppItem");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var appItemViewModel = new AppItemViewModel()
			{
				Name = "New Client"
			};
			AppItems.Add(appItemViewModel);
			SelectedAppItem = AppItems.LastOrDefault();
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			AppItems.Remove(SelectedAppItem);
		}

		bool CanRemove()
		{
			return SelectedAppItem != null;
		}
	}
}
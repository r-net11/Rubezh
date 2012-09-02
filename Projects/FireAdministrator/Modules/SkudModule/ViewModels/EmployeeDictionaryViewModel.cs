using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace SkudModule.ViewModels
{
	public class EmployeeDictionaryViewModel<T> : MenuViewPartViewModel, IEditingViewModel
	{
		public ObservableCollection<EmployeeDictionaryItemViewModel<T>> Dictionary { get; private set; }

		private EmployeeDictionaryItemViewModel<T> _selectedItem;
		public EmployeeDictionaryItemViewModel<T> SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged("SelectedItem");
			}
		}

		public EmployeeDictionaryViewModel()
		{
			Menu = new EmployeeDictionaryMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RefreshCommand = new RelayCommand(OnRefresh);
			Dictionary = new ObservableCollection<EmployeeDictionaryItemViewModel<T>>();
			Initialize();
		}

		public void Initialize()
		{
			var list = GetDictionary();
			Dictionary.Clear();
			if (list != null)
				foreach (var item in list)
					Dictionary.Add(new EmployeeDictionaryItemViewModel<T>(item));
			SelectedItem = Dictionary.FirstOrDefault();
		}
		protected virtual IEnumerable<T> GetDictionary()
		{
			return new List<T>();
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			MessageBoxService.ShowWarning("Under construction");
		}
		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			MessageBoxService.ShowWarning("Under construction");
		}
		bool CanEditRemove()
		{
			return SelectedItem != null;
		}
		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			MessageBoxService.ShowWarning("Under construction");
		}
		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.MessageBox;
using Infrastructure;
using Infrastructure.Common;

namespace SkudModule.ViewModels
{
	public class EmployeeDictionaryViewModel<T> : RegionViewModel, IEditingViewModel
	{
		public ObservableCollection<EmployeeDictionaryItemViewModel<T>> Dictionary { get; private set; }
		private EmployeeDictionaryItemViewModel<T> _selectedItem;
		private EmployeeDictionaryMenuViewModel _menuViewModel;

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
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RefreshCommand = new RelayCommand(OnRefresh);
			Dictionary = new ObservableCollection<EmployeeDictionaryItemViewModel<T>>();
			_menuViewModel = new EmployeeDictionaryMenuViewModel(this);
			Initialize();
		}

		public void Initialize()
		{
			var list = GetDictionary();
			Dictionary.Clear();
			if (list != null)
				foreach (var item in list)
					Dictionary.Add(new EmployeeDictionaryItemViewModel<T>(item));

			if (Dictionary.Count > 0)
				SelectedItem = Dictionary[0];
		}
		protected virtual IEnumerable<T> GetDictionary()
		{
			return new List<T>();
		}

		public override void OnShow()
		{
			ServiceFactory.Layout.ShowMenu(_menuViewModel);
		}
		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);
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
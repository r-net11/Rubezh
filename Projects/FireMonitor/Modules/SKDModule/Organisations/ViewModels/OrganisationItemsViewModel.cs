using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationItemsViewModel<T> : BaseViewModel
		where T : IOrganisationItemViewModel
	{
		public Organisation Organisation { get; protected set; }

		public OrganisationItemsViewModel(Organisation organisation)
		{
			Organisation = organisation;
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		ObservableCollection<T> items;
		public ObservableCollection<T> Items
		{
			get { return items; }
			protected set
			{
				items = value;
				OnPropertyChanged(() => Items);
			}
		}

		T _selectedItem;
		public T SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			foreach (var item in Items)
			{
				item.IsChecked = true;
			}
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			foreach (var item in Items)
			{
				item.IsChecked = false;
			}
		}
	}
}

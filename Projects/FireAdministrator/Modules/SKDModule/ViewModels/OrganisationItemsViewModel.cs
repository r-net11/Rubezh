using System.Collections.ObjectModel;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationItemsViewModel<T> : BaseViewModel
		where T : IOrganisationItemViewModel
	{
		public Organisation Organisation { get; protected set; }
		protected abstract PermissionType Permission { get; }

		protected OrganisationItemsViewModel(Organisation organisation)
		{
			Organisation = organisation;
			CanSelect = !organisation.IsDeleted && FiresecManager.CheckPermission(Permission);
			SelectAllCommand = new RelayCommand(OnSelectAll, () => CanSelect);
			SelectNoneCommand = new RelayCommand(OnSelectNone, () => CanSelect);
		}

		private ObservableCollection<T> _items;
		public ObservableCollection<T> Items
		{
			get { return _items; }
			protected set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}

		public RelayCommand SelectAllCommand { get; private set; }
		private void OnSelectAll()
		{
			foreach (var item in Items)
			{
				item.IsChecked = true;
			}
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		private void OnSelectNone()
		{
			foreach (var item in Items)
			{
				item.IsChecked = false;
			}
		}

		private bool _canSelect;
		public bool CanSelect
		{
			get { return _canSelect; }
			set
			{
				_canSelect = value;
				OnPropertyChanged(() => CanSelect);
			}
		}
	}
}

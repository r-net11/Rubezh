using System.Collections.ObjectModel;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Linq;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationItemsViewModel<T> : BaseViewModel
		where T : IOrganisationItemViewModel
	{
		public Organisation Organisation { get; protected set; }
		protected abstract PermissionType Permission { get; }

		public OrganisationItemsViewModel(Organisation organisation)
		{
			Organisation = organisation;
			CanSelect = !organisation.IsDeleted && ClientManager.CheckPermission(Permission);
			SelectAllCommand = new RelayCommand(OnSelectAll, () => CanSelect && IsConnected);
			SelectNoneCommand = new RelayCommand(OnSelectNone, () => CanSelect && IsConnected);
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

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			foreach (var item in Items.Where(x => x.CanChange))
			{
				item.IsChecked = true;
			}
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			foreach (var item in Items.Where(x => x.CanChange))
			{
				item.IsChecked = false;
			}
		}

		bool _canSelect;
		public bool CanSelect
		{
			get { return _canSelect; }
			set
			{
				_canSelect = value;
				OnPropertyChanged(() => CanSelect);
			}
		}

		public bool IsConnected
		{
			get { return ((SafeFiresecService)ClientManager.FiresecService).IsConnected; }
		}
	}
}
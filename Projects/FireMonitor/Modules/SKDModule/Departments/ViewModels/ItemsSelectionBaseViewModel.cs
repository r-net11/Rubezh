using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public abstract class ItemsSelectionBaseViewModel<TItem> : SaveCancelDialogViewModel where TItem : SKDModelBase
	{
		#region <Свойства и поля>

		protected Guid _organisationUID;

		private ObservableCollection<TItem> _items;
		private TItem _selectedItem;
		private string _releaseItemCommandText;
		private string _addItemCommandText;

		/// <summary>
		/// Список элементов
		/// </summary>
		public ObservableCollection<TItem> Items
		{
			get { return _items; }
			protected set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}

		/// <summary>
		/// Выделенный элемент
		/// </summary>
		public TItem SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}

		/// <summary>
		/// Название кнопки "Открепить элемент"
		/// </summary>
		public string ReleaseItemCommandText
		{
			get { return _releaseItemCommandText; }
			set
			{
				_releaseItemCommandText = value;
				OnPropertyChanged(() => ReleaseItemCommandText);
			}
		}

		/// <summary>
		/// Название для кнопки "Добавить элемент"
		/// </summary>
		public string AddItemCommandText
		{
			get { return _addItemCommandText; }
			set
			{
				_addItemCommandText = value;
				OnPropertyChanged(() => AddItemCommandText);
			}
		}

		/// <summary>
		/// Команда для кнопки "Открепить элемент
		/// </summary>
		public RelayCommand ReleaseItemCommand { get; private set; }

		/// <summary>
		/// Команда для кнопки "Добавить элемент"
		/// </summary>
		public RelayCommand AddItemCommand { get; private set; }

		#endregion </Свойства и поля>

		#region <Конструктор>

		protected ItemsSelectionBaseViewModel()
		{
			ReleaseItemCommand = new RelayCommand(OnReleaseItem, CanReleaseItem);
			ReleaseItemCommandText = "Открепить элемент";

			AddItemCommand = new RelayCommand(OnAddItem, CanAddItem);
			AddItemCommandText = "Добавить элемент";
		}

		#endregion </Конструктор>

		#region <Методы>

		protected virtual void OnReleaseItem()
		{
			SelectedItem = null;
		}

		protected virtual bool CanReleaseItem()
		{
			return SelectedItem != null;
		}

		protected abstract void OnAddItem();

		protected virtual bool CanAddItem()
		{
			return true;
		}

		public virtual void Initialize(Guid organisationUID, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active, TItem selectedItem = null)
		{
			_organisationUID = organisationUID;
			InitializeItems(organisationUID, logicalDeletationType);
			InitializeSelectedItem(selectedItem);
		}

		protected virtual void InitializeItems(Guid organisationUID, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
		{
			Items = new ObservableCollection<TItem>();
		}

		protected virtual void InitializeSelectedItem(TItem item)
		{
			SelectedItem = item == null
				? null
				: Items.FirstOrDefault(x => x.UID == item.UID);
		}

		#endregion </Методы>
	}
}
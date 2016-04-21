using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DiagnosticsModule.ViewModels
{
	public partial class ItemsBaseViewModel<T, ViewModelT> : MenuViewPartViewModel
		where T : ModelBase
		where ViewModelT : ItemBaseViewModel<T>, new()
	{
		HierarchicalCollection<T> ItemsCollection;
		HierarchicalItem<T> clipboard;

		public ItemsBaseViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			AddChildCommand = new RelayCommand(OnAddChild, CanAddChild);
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CutCommand = new RelayCommand(OnCut, CanCut);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			MoveDownCommand = new RelayCommand(OnMoveDown, CanMoveDown);
			MoveUpCommand = new RelayCommand(OnMoveUp, CanMoveUp);
		}

		public void Initialize(HierarchicalCollection<T> itemsCollection)
		{
			ItemsCollection = itemsCollection;
			SelectedItem = null;
			Items = new ObservableCollection<ViewModelT>();
			foreach (var rootItem in ItemsCollection.RootItems)
				AddBaseItemViewModel(rootItem, null);
			if (SelectedItem != null)
				SelectedItem.ExpandToThis();
		}

		ViewModelT AddBaseItemViewModel(HierarchicalItem<T> item, ViewModelT parentItem)
		{
			var itemBaseViewModel = (ViewModelT)Activator.CreateInstance(typeof(ViewModelT), item.Item);
			if (parentItem == null)
			{
				Items.Add(itemBaseViewModel);
			}
			else
			{
				parentItem.AddChild(itemBaseViewModel);
			}

			foreach (var child in item.Children)
				AddBaseItemViewModel(child, itemBaseViewModel);
			return itemBaseViewModel;
		}

		ObservableCollection<ViewModelT> _items;
		public ObservableCollection<ViewModelT> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}

		ViewModelT _selectedItem;
		public ViewModelT SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
				SelectionChanged();
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var t = OnAdding();
			Add(t);
			OnChanging();
		}

		public virtual T OnAdding()
		{
			return null;
		}

		public RelayCommand AddChildCommand { get; private set; }
		void OnAddChild()
		{
			var t = OnAddingChild();
			AddChild(t);
			OnChanging();
		}
		bool CanAddChild()
		{
			return SelectedItem != null;
		}

		void Add(T t)
		{
			ItemsCollection.Add(null, t);
			var itemViewModel = (ViewModelT)Activator.CreateInstance(typeof(ViewModelT), t);
			Items.Add(itemViewModel);
			SelectedItem = itemViewModel;
		}

		void AddChild(T t)
		{
			ItemsCollection.Add(SelectedItem.Item, t);
			var itemViewModel = (ViewModelT)Activator.CreateInstance(typeof(ViewModelT), t);
			SelectedItem.AddChild(itemViewModel);
			SelectedItem = itemViewModel;
		}

		public virtual T OnAddingChild()
		{
			return null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var itemBaseCancelArgs = new ItemBaseCancelArgs();
			OnRemoving(itemBaseCancelArgs);
			if (!itemBaseCancelArgs.IsCanceled)
			{
				Remove();
			}

			OnChanging();
		}
		bool CanRemove()
		{
			return SelectedItem != null;
		}

		void Remove()
		{
			ItemsCollection.Remove(SelectedItem.Item);
			if (SelectedItem.Parent != null)
			{
				SelectedItem.Parent.RemoveChild(SelectedItem);
			}
			else
			{
				Items.Remove(SelectedItem);
			}
			SelectedItem = Items.FirstOrDefault();
		}

		protected virtual void OnRemoving(ItemBaseCancelArgs itemBaseCancelArgs) { ;}

		public RelayCommand EditCommand { get; private set; }
		protected virtual void OnEdit()
		{
		}
		bool CanEdit()
		{
			return SelectedItem != null;
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			clipboard = ItemsCollection.Clone(SelectedItem.Item);
			Remove();
			OnChanging();
		}
		bool CanCut()
		{
			return SelectedItem != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			clipboard = ItemsCollection.Clone(SelectedItem.Item);
		}
		bool CanCopy()
		{
			return SelectedItem != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var copy = Utils.Clone<HierarchicalItem<T>>(clipboard);
			ItemsCollection.AddWithChild(copy, SelectedItem.Item);
			AddBaseItemViewModel(copy, SelectedItem);
			OnChanging();
		}
		bool CanPaste()
		{
			return clipboard != null;
		}

		public RelayCommand MoveDownCommand { get; private set; }
		void OnMoveDown()
		{
			Move(+1);
		}
		bool CanMoveDown()
		{
			if (SelectedItem == null)
				return false;
			if (SelectedItem.Parent == null)
			{
				var index = Items.IndexOf(SelectedItem);
				if (index > Items.Count - 2)
					return false;
			}
			else
			{
				return true;
			}
			return true;
		}

		public RelayCommand MoveUpCommand { get; private set; }
		void OnMoveUp()
		{
			Move(-1);
		}
		bool CanMoveUp()
		{
			if (SelectedItem == null)
				return false;
			if (SelectedItem.Parent == null)
			{
				var index = Items.IndexOf(SelectedItem);
				if (index < 1)
					return false;
			}
			else
			{
				return true;
			}
			return true;
		}

		void Move(int delta)
		{
			if (SelectedItem.Parent == null)
			{
				var itemViewModel = SelectedItem;
				var index = Items.IndexOf(SelectedItem);
				Items.Remove(SelectedItem);
				Items.Insert(index + delta, itemViewModel);
				SelectedItem = itemViewModel;

				var item = itemViewModel.Item;
			}
			else
			{
				var itemViewModel = SelectedItem;
				var parentViewModel = SelectedItem.Parent;
				var index = SelectedItem.Index;
				var parentIndex = parentViewModel.Index;
				parentViewModel.RemoveChild(SelectedItem);
				if (delta == 1)
				{
					if (parentViewModel.ChildrenCount <= (index + delta - 1))
					{
						if (parentViewModel.Parent == null)
						{
							Items.Insert(parentIndex + delta, itemViewModel);
						}
						else
						{
							parentViewModel.Parent[parentIndex + delta - 1].InsertChild(itemViewModel);
						}
					}
					else
					{
						parentViewModel[index + delta - 1].InsertChild(itemViewModel);
					}
				}
				else
				{
					if (index == 0)
					{
						if (parentViewModel.Parent == null)
						{
							Items.Insert(parentIndex + delta + 1, itemViewModel);
						}
						else
						{
							parentViewModel.Parent[parentIndex + delta + 1].InsertTo(itemViewModel);
						}
					}
					else
					{
						parentViewModel[index + delta].InsertTo(itemViewModel);
					}
				}
				SelectedItem = itemViewModel;
			}
		}

		public virtual void OnChanging() { ;}
		public virtual void SelectionChanged() { ;}

		public override void OnShow()
		{
			if (SelectedItem == null)
				SelectedItem = Items.FirstOrDefault();
			else
				SelectedItem = SelectedItem;
			base.OnShow();
		}
		public override void OnHide()
		{
			base.OnHide();
		}
	}

	public class ItemBaseCancelArgs
	{
		public bool IsCanceled { get; set; }
	}
}
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
		bool isClipboardCut;

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
			RootItems = new ObservableCollection<ViewModelT>();
			foreach (var rootItem in ItemsCollection.RootItems)
				AddBaseItemViewModel(rootItem, null);
			if (SelectedItem != null)
				SelectedItem.ExpandToThis();
		}

		ViewModelT AddBaseItemViewModel(HierarchicalItem<T> item, ViewModelT parentItem)
		{
			var itemBaseViewModel = (ViewModelT)Activator.CreateInstance(typeof(ViewModelT), item);
			if (parentItem == null)
			{
				RootItems.Add(itemBaseViewModel);
			}
			else
			{
				parentItem.AddChild(itemBaseViewModel);
			}

			foreach (var child in item.Children)
				AddBaseItemViewModel(child, itemBaseViewModel);
			return itemBaseViewModel;
		}

		ObservableCollection<ViewModelT> _rootItems;
		public ObservableCollection<ViewModelT> RootItems
		{
			get { return _rootItems; }
			set
			{
				_rootItems = value;
				OnPropertyChanged(() => RootItems);
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

			var hierarchicalItem = new HierarchicalItem<T>() { Item = t };
			ItemsCollection.Add(SelectedItem == null ? null : SelectedItem.HierarchicalItem, hierarchicalItem);
			var itemViewModel = (ViewModelT)Activator.CreateInstance(typeof(ViewModelT), hierarchicalItem);


			if (SelectedItem != null)
			{
				if (SelectedItem.Parent != null)
				{
					SelectedItem.InsertChild(itemViewModel);
				}
				else
				{
					var index = RootItems.IndexOf(SelectedItem);
					RootItems.Insert(index + 1, itemViewModel);
				}
			}
			else
			{
				RootItems.Add(itemViewModel);
			}

			itemViewModel.ExpandToThis();
			SelectedItem = itemViewModel;
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

			var hierarchicalItem = new HierarchicalItem<T>() { Item = t };
			ItemsCollection.AddChild(SelectedItem.HierarchicalItem, hierarchicalItem);
			var itemViewModel = (ViewModelT)Activator.CreateInstance(typeof(ViewModelT), hierarchicalItem);
			SelectedItem.AddChild(itemViewModel);

			itemViewModel.ExpandToThis();
			SelectedItem = itemViewModel;
			OnChanging();
		}
		bool CanAddChild()
		{
			return SelectedItem != null;
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

			var selectedItem = SelectedItem;
			ViewModelT parent = (ViewModelT)selectedItem.Parent;
			var item = SelectedItem.Item;
			var index = RootItems.IndexOf(selectedItem);
			var oldIndex = selectedItem.Index;

			if (parent == null)
			{
				RootItems.Remove(selectedItem);
				index = Math.Min(index, RootItems.Count - 1);
				if (index > -1)
					SelectedItem = RootItems[index];
			}
			else
			{
				parent.RemoveChild(selectedItem);
				if (parent.ChildrenCount == 0)
				{
					SelectedItem = parent;
				}
				else
				{
					if (oldIndex == 0)
					{
						SelectedItem = (ViewModelT)parent.Children.ToArray()[oldIndex];
					}
					else SelectedItem = (ViewModelT)parent.Children.ToArray()[oldIndex - 1];
				}
				parent.IsExpanded = true;
			}
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
			isClipboardCut = true;
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
			isClipboardCut = false;
		}
		bool CanCopy()
		{
			return SelectedItem != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var copy = Utils.Clone<HierarchicalItem<T>>(clipboard);
			if (!isClipboardCut)
				copy.Item.UID = Guid.NewGuid();
			isClipboardCut = false;

			ItemsCollection.AddChild(SelectedItem.HierarchicalItem, copy);
			var itemViewModel = AddBaseItemViewModel(copy, SelectedItem);
			itemViewModel.ExpandToThis();
			OnChanging();
		}
		bool CanPaste()
		{
			return SelectedItem != null && clipboard != null;
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
				var index = RootItems.IndexOf(SelectedItem);
				if (index > RootItems.Count - 2)
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
				var index = RootItems.IndexOf(SelectedItem);
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
			ItemsCollection.Move(SelectedItem.Item, delta);

			if (SelectedItem.Parent == null)
			{
				var itemViewModel = SelectedItem;
				var index = RootItems.IndexOf(SelectedItem);
				RootItems.Remove(SelectedItem);
				RootItems.Insert(index + delta, itemViewModel);
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
							RootItems.Insert(parentIndex + delta, itemViewModel);
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
							RootItems.Insert(parentIndex + delta + 1, itemViewModel);
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
				SelectedItem = RootItems.FirstOrDefault();
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
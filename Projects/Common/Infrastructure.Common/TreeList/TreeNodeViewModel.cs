using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Infrastructure.Common.Windows.ViewModels;
using Common;

namespace Infrastructure.Common.TreeList
{
	public class TreeNodeViewModel : BaseViewModel
	{
		private ITreeList Tree { get; set; }
		public void AssignToTree(ITreeList tree)
		{
			Tree = tree;
			foreach (var child in Children)
				child.AssignToTree(Tree);
			if (Parent == null)
			{
				Tree.Rows.Add(this);
				CreateChildrenRows();
			}
		}

		public class TreeItemCollection : ObservableCollectionAdv<TreeNodeViewModel>
		{
			private TreeNodeViewModel _owner;

			public TreeItemCollection(TreeNodeViewModel owner)
			{
				_owner = owner;
			}

			protected override void ClearItems()
			{
				while (Count != 0)
					RemoveAt(Count - 1);
			}
			protected override void InsertItem(int index, TreeNodeViewModel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");

				if (item.Parent != _owner)
				{
					if (item.Parent != null)
						item.Parent.Children.Remove(item);
					item.Parent = _owner;
					item.Index = index;
					item.Tree = _owner.Tree;
					for (int i = index; i < Count; i++)
						this[i].Index++;
					base.InsertItem(index, item);
				}
			}
			protected override void RemoveItem(int index)
			{
				TreeNodeViewModel item = this[index];
				item.Parent = null;
				item.Index = -1;
				for (int i = index + 1; i < Count; i++)
					this[i].Index--;
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, TreeNodeViewModel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				RemoveAt(index);
				InsertItem(index, item);
			}
		}

		public int Level
		{
			get
			{
				if (Parent == null)
					return 0;
				else
					return Parent.Level + 1;
			}
		}
		public int Index { get; private set; }
		public TreeNodeViewModel Parent { get; private set; }
		public TreeItemCollection Children { get; private set; }
		public bool IsExpandable
		{
			get { return Children.Count > 0; }
		}

		internal TreeNodeViewModel()
		{
			Tree = null;
			Index = -1;
			Children = new TreeItemCollection(this);
			Children.CollectionChanged += ChildrenChanged;
		}

		private bool IsVisible
		{
			get
			{
				TreeNodeViewModel node = Parent;
				while (node != null)
				{
					if (!node.IsExpanded)
						return false;
					node = node.Parent;
				}
				return true;
			}
		}
		private TreeNodeViewModel BottomNode
		{
			get
			{
				if (Parent != null)
				{
					if (Parent.NextNode != null)
						return Parent.NextNode;
					else
						return Parent.BottomNode;
				}
				return null;
			}
		}
		private TreeNodeViewModel NextVisibleNode
		{
			get
			{
				if (IsExpanded && Children.Count > 0)
					return Children[0];
				else
					return NextNode ?? BottomNode;
			}
		}
		private TreeNodeViewModel NextNode
		{
			get
			{
				if (Parent != null)
				{
					if (Index < Parent.Children.Count - 1)
						return Parent.Children[Index + 1];
				}
				return null;
			}
		}
		private int VisibleChildrenCount
		{
			get { return AllVisibleChildren.Count(); }
		}
		private IEnumerable<TreeNodeViewModel> AllVisibleChildren
		{
			get
			{
				int level = Level;
				TreeNodeViewModel node = this;
				while (true)
				{
					node = node.NextVisibleNode;
					if (node != null && node.Level > level)
						yield return node;
					else
						break;
				}
			}
		}

		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (IsExpanded && Tree != null)
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						if (e.NewItems != null)
						{
							int index = e.NewStartingIndex;
							int rowIndex = Tree.Rows.IndexOf(this);
							foreach (TreeNodeViewModel node in e.NewItems)
							{
								Tree.Rows.Insert(rowIndex + index + 1, node);
								index++;
							}
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (TreeNodeViewModel node in e.OldItems)
							node.DropChildrenRows(true);
						break;
					case NotifyCollectionChangedAction.Move:
					case NotifyCollectionChangedAction.Replace:
					case NotifyCollectionChangedAction.Reset:
						DropChildrenRows(false);
						CreateChildrenRows();
						break;
				}
			OnPropertyChanged("IsExpandable");
		}
		private void DropChildrenRows(bool removeParent)
		{
			int start = Tree.Rows.IndexOf(this);
			if (start >= 0)
			{
				int count = VisibleChildrenCount;
				if (removeParent)
					count++;
				else
					start++;
				Tree.Rows.RemoveRange(start, count);
			}
		}
		private void CreateChildrenRows()
		{
			int index = Tree.Rows.IndexOf(this);
			if (index >= 0)
			{
				var nodes = AllVisibleChildren.ToArray();
				Tree.Rows.InsertRange(index + 1, nodes);
				//if (nodes.Contains(Tree.SelectedTreeItem))
				//    Tree.ResumeSelection();
			}
		}

		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (value != IsExpanded)
				{
					if (value)
					{
						_isExpanded = true;
						if (Tree != null)
						{
							CreateChildrenRows();
							Tree.ResumeSelection();
						}
					}
					else
					{
						if (Tree != null)
						{
							Tree.SuspendSelection();
							DropChildrenRows(false);
						}
						_isExpanded = false;
					}
					OnPropertyChanged(() => IsExpandable);
					OnPropertyChanged(() => IsExpanded);
				}
			}
		}

		public bool IsSelected
		{
			get { return Tree == null ? false : Tree.SelectedTreeNode == this; }
			set
			{
				if (Tree != null)
				{
					ExpandToThis();
					Tree.SelectedTreeNode = this;
				}
				OnPropertyChanged(() => IsSelected);
			}
		}
		public bool HasChildren
		{
			get { return Children.Count > 0; }
		}

		public void ExpandToThis()
		{
			var parent = this;
			while (parent != null)
			{
				parent.IsExpanded = true;
				parent = parent.Parent;
			}
		}
		public void CollapseChildren(bool withSelf = true)
		{
			ProcessAllChildren(this, withSelf, item => item.IsExpanded = false);
		}
		public void ExpandChildren(bool withSelf = true)
		{
			ProcessAllChildren(this, withSelf, item => item.IsExpanded = true);
		}
		private void ProcessAllChildren(TreeNodeViewModel parent, bool withSelf, Action<TreeNodeViewModel> action)
		{
			if (withSelf)
				action(parent);
			foreach (TreeNodeViewModel t in parent.Children)
				ProcessAllChildren(t, true, action);
		}
	}

	public class TreeNodeViewModel<T> : TreeNodeViewModel
		where T : TreeNodeViewModel<T>
	{
		public TreeNodeViewModel()
		{
		}
		public TreeNodeViewModel(IEnumerable<T> children)
			: this()
		{
			Children.InsertRange(0, children);
		}

		public T ParentItem
		{
			get { return Parent as T; }
		}
		public T this[int index]
		{
			get { return Children[index] as T; }
		}
		public void Add(T item)
		{
			Children.Add(item);
		}

		public List<T> GetAllParents()
		{
			if (Parent == null)
				return new List<T>();
			else
			{
				List<T> allParents = ParentItem.GetAllParents();
				allParents.Add(ParentItem);
				return allParents;
			}
		}
		public IEnumerable<T> GetChildren()
		{
			foreach(T child in Children)
				yield return child;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using Infrastructure.Common.TreeList;

namespace Controls.TreeList
{
	public sealed class TreeNode : INotifyPropertyChanged
	{
		private class NodeCollection : Collection<TreeNode>
		{
			private TreeNode _owner;

			public NodeCollection(TreeNode owner)
			{
				_owner = owner;
			}

			protected override void ClearItems()
			{
				while (Count != 0)
					RemoveAt(Count - 1);
			}
			protected override void InsertItem(int index, TreeNode item)
			{
				if (item == null)
					throw new ArgumentNullException("item");

				if (item.Parent != _owner)
				{
					if (item.Parent != null)
						item.Parent.Children.Remove(item);
					item.Parent = _owner;
					item.Index = index;
					for (int i = index; i < Count; i++)
						this[i].Index++;
					base.InsertItem(index, item);
				}
			}
			protected override void RemoveItem(int index)
			{
				TreeNode item = this[index];
				item.Parent = null;
				item.Index = -1;
				for (int i = index + 1; i < Count; i++)
					this[i].Index--;
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, TreeNode item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				RemoveAt(index);
				InsertItem(index, item);
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion

		private INotifyCollectionChanged _childrenSource;
		internal INotifyCollectionChanged ChildrenSource
		{
			get { return _childrenSource; }
			set
			{
				if (_childrenSource != null)
					_childrenSource.CollectionChanged -= ChildrenChanged;
				_childrenSource = value;
				if (_childrenSource != null)
					_childrenSource.CollectionChanged += ChildrenChanged;
			}
		}
		internal TreeList Tree { get; private set; }
		internal bool IsVisible
		{
			get
			{
				TreeNode node = Parent;
				while (node != null)
				{
					if (!node.IsExpanded)
						return false;
					node = node.Parent;
				}
				return true;
			}
		}
		internal TreeNode BottomNode
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
		internal TreeNode NextVisibleNode
		{
			get
			{
				if (IsExpanded && Nodes.Count > 0)
					return Nodes[0];
				else
					return NextNode ?? BottomNode;
			}
		}
		internal Collection<TreeNode> Children { get; private set; }

		public bool IsSelected
		{
			get { return Tag.IsSelected; }
			set
			{
				if (value != Tag.IsSelected)
				{
					Tag.IsSelected = value;
					OnPropertyChanged("IsSelected");
				}
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
					Tree.SetIsExpanded(this, value);
					OnPropertyChanged("IsExpanded");
					OnPropertyChanged("IsExpandable");
				}
			}
		}
		public bool IsExpandable
		{
			get { return (HasChildren && !IsExpandedOnce) || Nodes.Count > 0; }
		}
		public TreeNode Parent { get; private set; }
		public int Level
		{
			get
			{
				if (Parent == null)
					return -1;
				else
					return Parent.Level + 1;
			}
		}
		public TreeNode PreviousNode
		{
			get
			{
				if (Parent != null)
				{
					if (Index > 0)
						return Parent.Nodes[Index - 1];
				}
				return null;
			}
		}
		public TreeNode NextNode
		{
			get
			{
				if (Parent != null)
				{
					if (Index < Parent.Nodes.Count - 1)
						return Parent.Nodes[Index + 1];
				}
				return null;
			}
		}
		public int VisibleChildrenCount
		{
			get { return AllVisibleChildren.Count(); }
		}
		public IEnumerable<TreeNode> AllVisibleChildren
		{
			get
			{
				int level = this.Level;
				TreeNode node = this;
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
		public ITreeNodeModel Tag { get; private set; }
		public ReadOnlyCollection<TreeNode> Nodes { get; private set; }
		public int Index { get; private set; }
		public bool IsExpandedOnce { get; internal set; }
		public bool HasChildren { get; internal set; }

		internal TreeNode(TreeList tree, ITreeNodeModel tag)
		{
			if (tree == null)
				throw new ArgumentNullException("tree");

			Index = -1;
			Tree = tree;
			Children = new NodeCollection(this);
			Nodes = new ReadOnlyCollection<TreeNode>(Children);
			Tag = tag;
			if (Tag != null)
			{
				Tag.PropertyChanged += new PropertyChangedEventHandler(Tag_PropertyChanged);
				IsExpanded = Tag.IsExpanded;
			}
		}

		//public override string ToString()
		//{
		//    return Tag != null ? Tag.ToString() : base.ToString();
		//}

		internal void AssignIsExpanded(bool value)
		{
			_isExpanded = value;
			if (Tag != null && Tag.IsExpanded != value)
				Tag.IsExpanded = value;
		}

		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems != null)
					{
						int index = e.NewStartingIndex;
						int rowIndex = Tree.Rows.IndexOf(this);
						foreach (ITreeNodeModel obj in e.NewItems)
						{
							Tree.InsertNewNode(this, obj, rowIndex, index);
							index++;
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					if (Children.Count > e.OldStartingIndex)
						RemoveChildAt(e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					while (Children.Count > 0)
						RemoveChildAt(0);
					Tree.CreateChildrenNodes(this);
					break;
			}
			HasChildren = Children.Count > 0;
			OnPropertyChanged("IsExpandable");
		}
		private void RemoveChildAt(int index)
		{
			var child = Children[index];
			Tree.DropChildrenRows(child, true);
			ClearChildrenSource(child);
			Children.RemoveAt(index);
		}
		private void ClearChildrenSource(TreeNode node)
		{
			node.ChildrenSource = null;
			foreach (var n in node.Children)
				ClearChildrenSource(n);
		}

		private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsExpanded")
				IsExpanded = Tag.IsExpanded;
			else if (e.PropertyName == "IsSelected")
				OnPropertyChanged(e.PropertyName);
		}
	}
}

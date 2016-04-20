using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.TreeList
{
	public class TreeItemViewModel : BaseViewModel
	{
		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;
				OnPropertyChanged("IsExpanded");
			}
		}

		private bool _isSelected;
		public virtual bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (IsSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged("IsSelected");
				}
			}
		}

		public TreeItemViewModel TreeParent { get; protected set; }
	}
	public class TreeItemViewModel<T> : TreeItemViewModel, ITreeNodeModel
		where T : TreeItemViewModel<T>
	{
		public TreeItemViewModel()
		{
			Children = new ObservableCollection<T>();
			Children.CollectionChanged += new NotifyCollectionChangedEventHandler(Children_CollectionChanged);
		}
		public TreeItemViewModel(IEnumerable<T> children)
			: this()
		{
			foreach (var item in children)
				Children.Add(item);
		}

		private ObservableCollection<T> _children;
		public ObservableCollection<T> Children
		{
			get { return _children; }
			private set
			{
				_children = value;
				OnPropertyChanged("Children");
			}
		}

		private T _parent;
		public T Parent
		{
			get { return _parent; }
			private set
			{
				_parent = value;
				TreeParent = value;
				OnPropertyChanged("Parent");
				OnPropertyChanged("TreeParent");
			}
		}

		public bool HasChildren
		{
			get { return Children.Count > 0; }
		}
		public int Level
		{
			get { return GetAllParents().Count(); }
		}

		public override bool IsSelected
		{
			get { return base.IsSelected; }
			set
			{
				ExpandToThis();
				base.IsSelected = value;
			}
		}

		public void ExpandToThis()
		{
			GetAllParents().ForEach(x => x.IsExpanded = true);
		}
		public void CollapseChildren(bool withSelf = true)
		{
			ProcessAllChildren((T)this, withSelf, item => item.IsExpanded = false);
		}
		public void ExpandChildren(bool withSelf = true)
		{
			ProcessAllChildren((T)this, withSelf, item => item.IsExpanded = true);
		}

		public void ResetParent()
		{
			Parent = null;
		}

		public List<T> GetAllParents()
		{
			if (Parent == null)
				return new List<T>();
			else
			{
				List<T> allParents = Parent.GetAllParents();
				allParents.Add(Parent);
				return allParents;
			}
		}
		private void ProcessAllChildren(T parent, bool withSelf, Action<T> action)
		{
			if (withSelf)
				action(parent);
			foreach (T t in parent.Children)
				ProcessAllChildren(t, true, action);
		}
		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (T item in e.NewItems)
						item.Parent = (T)this;
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (T item in e.OldItems)
						item.Parent = null;
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (T item in e.OldItems)
						item.Parent = null;
					foreach (T item in e.NewItems)
						item.Parent = (T)this;
					break;
			}
			OnPropertyChanged(() => HasChildren);
		}

		#region ITreeNodeModel Members

		public IEnumerable GetChildren()
		{
			return Children;
		}

		#endregion
	}
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common
{
	public class TreeBaseViewModel<T> : BaseViewModel
		where T : TreeBaseViewModel<T>
	{
		public TreeBaseViewModel()
		{
			Children = new ObservableCollection<T>();
		}

		public ObservableCollection<T> Source { get; set; }
		ObservableCollection<T> updatingSource { get; set; }

		bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;

				updatingSource = Source;
				Source = null;

				if (_isExpanded)
					ExpandChildren(this as T);
				else
					HideChildren(this as T);

				Source = updatingSource;

				OnPropertyChanged("IsExpanded");
			}
		}

		void HideChildren(T parent)
		{
			foreach (T t in parent.Children)
			{
				if (updatingSource.Contains(t))
					updatingSource.Remove(t);
				HideChildren(t);
			}
		}

		void ExpandChildren(T parent)
		{
			if (parent.IsExpanded)
			{
				int indexOf = updatingSource.IndexOf(parent);
				for (int i = 0; i < parent.Children.Count; i++)
				{
					if (updatingSource.Contains(parent.Children[i]) == false)
						updatingSource.Insert(indexOf + 1 + i, parent.Children[i]);
				}

				foreach (T t in parent.Children)
				{
					ExpandChildren(t);
				}
			}
		}

		public bool HasChildren
		{
			get { return (Children.Count > 0); }
		}

		public int Level
		{
			get { return GetAllParents().Count(); }
		}

		public void ExpantToThis()
		{
			GetAllParents().ForEach(x => x.IsExpanded = true);
		}

		List<T> GetAllParents()
		{
			if (Parent == null)
			{
				return new List<T>();
			}
			else
			{
				List<T> allParents = Parent.GetAllParents();
				allParents.Add(Parent);
				return allParents;
			}
		}

		public T Parent { get; set; }

		ObservableCollection<T> _children;
		public ObservableCollection<T> Children
		{
			get { return _children; }
			set
			{
				_children = value;
				OnPropertyChanged("Children");
			}
		}

		public bool IsBold { get; set; }
	}
}
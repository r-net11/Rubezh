using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Infrastructure
{
    public class TreeBaseViewModel<T> : BaseViewModel
        where T : TreeBaseViewModel<T>
    {
        public TreeBaseViewModel()
        {
            Children = new ObservableCollection<T>();
        }

        public ObservableCollection<T> Source { get; set; }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;

                if (isExpanded)
                {
                    ExpandChildren(this as T);
                }
                else
                {
                    HideChildren(this as T);
                }

                OnPropertyChanged("IsExpanded");
            }
        }

        void HideChildren(T parent)
        {
            foreach (T t in parent.Children)
            {
                if (Source.Contains(t))
                    Source.Remove(t);
                HideChildren(t);
            }
        }

        void ExpandChildren(T parent)
        {
            if (parent.IsExpanded)
            {
                int indexOf = Source.IndexOf(parent);
                for (int i = 0; i < parent.Children.Count; i++)
                {
                    if (Source.Contains(parent.Children[i]) == false)
                    {
                        Source.Insert(indexOf + 1 + i, parent.Children[i]);
                    }
                }

                foreach (T t in parent.Children)
                {
                    ExpandChildren(t);
                }
            }
        }


        public bool HasChildren
        {
            get
            {
                return (Children.Count > 0);
            }
        }

        public int Level
        {
            get
            {
                return GetAllParents().Count();
            }
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

        ObservableCollection<T> children;
        public ObservableCollection<T> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged("Children");
            }
        }
    }
}

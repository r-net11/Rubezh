using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Common
{
    public class TreeBase : INotifyPropertyChanged
    {
        public int? OrderNo { get; set; }

        string address;
        public string Address
        {
            get {return address;}
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        string parentAddress;
        public string ParentAddress
        {
            get { return parentAddress; }
            set
            {
                parentAddress = value;
                OnPropertyChanged("ParentAddress");
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        TreeBase parent;
        public TreeBase Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (parent != null)
                {
                    ParentAddress = parent.Address;
                    OrderNo = parent.Children.Count;
                    parent.Children.Add(this);
                }
                else
                {
                    ParentAddress = "";
                    OrderNo = null;
                }
                OnPropertyChanged("Parent");
            }
        }

        ObservableCollection<TreeBase> children;
        public ObservableCollection<TreeBase> Children
        {
            get
            {
                if (children == null)
                    children = new ObservableCollection<TreeBase>();
                return children;
            }
            set
            {
                children = value;
                OnPropertyChanged("Children");
            }
        }

        public string PlaceInTree
        {
            get
            {
                if (OrderNo.HasValue)
                {
                    if (Parent.PlaceInTree == "")
                        return OrderNo.ToString();
                    else
                        return Parent.PlaceInTree + @"\" + OrderNo.ToString();
                }
                else
                    return "";
            }
        }

        List<TreeBase> allChildren;
        public List<TreeBase> FindAllChildren()
        {
            allChildren = new List<TreeBase>();
            allChildren.Add(this);
            FindChildren(this);
            return allChildren;
        }

        void FindChildren(TreeBase parent)
        {
            if (parent.children != null)
                foreach (TreeBase child in parent.children)
                {
                    allChildren.Add(child);
                    FindChildren(child);
                }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
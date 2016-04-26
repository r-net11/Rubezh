using Common;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RubezhAPI.Hierarchy
{
	public class HierarchicalCollection<T> where T : ModelBase
	{
		public HierarchicalCollection()
		{
			RootItems = new List<HierarchicalItem<T>>();
		}

		public List<HierarchicalItem<T>> RootItems { get; set; }

		[XmlIgnore]
		public List<HierarchicalItem<T>> AllItems { get; private set; }

		public void BuildTree()
		{
			AllItems = new List<HierarchicalItem<T>>();
			RootItems.ForEach(x => AddToCache(x));
		}

		void AddToCache(HierarchicalItem<T> item)
		{
			AllItems.Add(item);
			foreach (var child in item.Children)
			{
				child.Parent = item;
				AllItems.Add(child);
			}
		}

		public bool AddChild(HierarchicalItem<T> parent, HierarchicalItem<T> item)
		{
			if (parent != null)
			{
				parent.Children.Add(item);
				item.Parent = parent;
			}
			else
			{
				RootItems.Add(item);
				item.Parent = null;
			}
			AddToCache(item);
			return true;
		}

		public bool Add(HierarchicalItem<T> parent, HierarchicalItem<T> item)
		{
			if (parent != null)
			{
				if (parent.Parent != null)
				{
					var index = parent.Parent.Children.IndexOf(parent);
					parent.Parent.Children.Insert(index + 1, parent);
				}
				else
				{
					var index = RootItems.IndexOf(parent);
					RootItems.Insert(index + 1, item);
				}
			}
			else
			{
				RootItems.Add(item);
				item.Parent = null;
			}
			AddToCache(item);
			return true;
		}

		public void Remove(T item)
		{
			var hierarchicalItem = AllItems.FirstOrDefault(x => x.Item.UID == item.UID);
			if (hierarchicalItem != null)
			{
				var allItems = new List<HierarchicalItem<T>>();
				GetAllChildren(hierarchicalItem, allItems);
				AllItems.Remove(hierarchicalItem);
				allItems.ForEach(x => AllItems.Remove(x));

				if (hierarchicalItem.Parent != null)
				{
					hierarchicalItem.Parent.Children.Remove(hierarchicalItem);
				}
			}
		}

		public HierarchicalItem<T> Clone(T item)
		{
			var hierarchicalItem = AllItems.FirstOrDefault(x => x.Item.UID == item.UID);
			if (hierarchicalItem != null)
			{
				return Utils.Clone<HierarchicalItem<T>>(hierarchicalItem);
			}
			return null;
		}

		void GetAllChildren(HierarchicalItem<T> item, List<HierarchicalItem<T>> allItems)
		{
			foreach (var child in item.Children)
			{
				allItems.Add(child);
				GetAllChildren(child, allItems);
			}
		}

		public void Move(T t, int delta)
		{
			var item = AllItems.FirstOrDefault(x => x.Item.UID == t.UID);
			if (item == null)
			{
				return;
			}

			if (item.Parent == null)
			{
				var index = RootItems.IndexOf(item);
				RootItems.Remove(item);
				RootItems.Insert(index + delta, item);
			}
			else
			{
				var itemViewModel = item;
				var parentItem = item.Parent;
				var index = parentItem.Children.IndexOf(item);
				var parentIndex = 0;
				if (parentItem.Parent == null)
				{
					parentIndex = RootItems.IndexOf(parentItem);
				}
				else
				{
					parentIndex = parentItem.Parent.Children.IndexOf(parentItem);
				}

				parentItem.Children.Remove(item);

				if (delta == 1)
				{
					if (parentItem.Children.Count <= (index + delta - 1))
					{
						if (parentItem.Parent == null)
						{
							RootItems.Insert(parentIndex + delta, item);
						}
						else
						{
							parentItem.Parent.Children.Insert(parentIndex + delta - 1, item);
						}
					}
					else
					{
						parentItem.Children.Insert(index + delta - 1, item);
					}
				}
				else
				{
					if (index == 0)
					{
						if (parentItem.Parent == null)
						{
							RootItems.Insert(parentIndex + delta + 1, itemViewModel);
						}
						else
						{
							parentItem.Children.Insert(parentIndex + delta + 1, itemViewModel);
						}
					}
					else
					{
						parentItem.Children.Insert(index + delta, itemViewModel);
					}
				}
			}
		}
	}
}
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
			var allItems = new List<HierarchicalItem<T>>();
			Build(RootItems, ref allItems);
			AllItems = allItems.ToList();
		}

		void Build(List<HierarchicalItem<T>> items, ref List<HierarchicalItem<T>> allItems)
		{
			allItems.AddRange(items);
			foreach (var item in items)
			{
				foreach (var child in item.Children)
				{
					child.Parent = item;
				}
				Build(item.Children, ref allItems);
			}
		}

		public bool Add(T parent, T item)
		{
			var hierarchicalItem = new HierarchicalItem<T>() { Item = item };
			if (parent != null)
			{
				var parentItem = AllItems.FirstOrDefault(x => x.Item.UID == parent.UID);
				if (parentItem == null)
					return false;
				parentItem.Children.Add(hierarchicalItem);
				hierarchicalItem.Parent = parentItem;
				AllItems.Add(hierarchicalItem);
			}
			else
			{
				RootItems.Add(hierarchicalItem);
				hierarchicalItem.Parent = null;
				AllItems.Add(hierarchicalItem);
			}
			return false;
		}

		public bool AddWithChild(HierarchicalItem<T> hierarchicalItem, T parent)
		{
			if (parent != null)
			{
				var parentItem = AllItems.FirstOrDefault(x => x.Item.UID == parent.UID);
				if (parentItem == null)
					return false;
				parentItem.Children.Add(hierarchicalItem);
				hierarchicalItem.Parent = parentItem;
				AllItems.Add(hierarchicalItem);
			}
			else
			{
				RootItems.Add(hierarchicalItem);
				hierarchicalItem.Parent = null;
				AllItems.Add(hierarchicalItem);
			}
			BuildTree();
			return true;
		}

		public void Remove(T item)
		{
			var hierarchicalItem = AllItems.FirstOrDefault(x => x.Item.UID == item.UID);
			if(hierarchicalItem != null)
			{
				var allItems = new List<HierarchicalItem<T>>();
				GetAllChildren(hierarchicalItem, allItems);
				AllItems.Remove(hierarchicalItem);
				allItems.ForEach(x => AllItems.Remove(x));

				if(hierarchicalItem.Parent != null)
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
			foreach(var child in item.Children)
			{
				allItems.Add(child);
				GetAllChildren(child, allItems);
			}
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Common;
using Infrastructure.Common.TreeList;

namespace Controls.TreeList
{
	public class TreeList : ListView
	{
		public static DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(IEnumerable), typeof(TreeList), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));
		public IEnumerable Source
		{
			get { return (IEnumerable)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var treeList = (TreeList)d;
			treeList.Root.Children.Clear();
			treeList.Rows.Clear();
			treeList.CreateChildrenNodes(treeList.Root);
		}

		public static DependencyProperty SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(ITreeNodeModel), typeof(TreeList), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedNodeChanged)) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
		public ITreeNodeModel SelectedNode
		{
			get { return (ITreeNodeModel)GetValue(SelectedNodeProperty); }
			set { SetValue(SelectedNodeProperty, value); }
		}
		private static void OnSelectedNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var treeList = (TreeList)d;
			var model = (ITreeNodeModel)e.NewValue;
			var oldModel = (ITreeNodeModel)e.OldValue;
			if (oldModel != null)
				oldModel.IsSelected = false;
			if (model == null)
				treeList.SetValue(Selector.SelectedItemProperty, null);
			else
				model.IsSelected = true;
		}

		internal TreeNode Root { get; private set; }
		internal ObservableCollectionAdv<TreeNode> Rows { get; private set; }
		internal TreeNode PendingFocusNode { get; set; }

		public ReadOnlyCollection<TreeNode> Nodes
		{
			get { return Root.Nodes; }
		}
		public ICollection<TreeNode> SelectedTreeNodes
		{
			get { return SelectedItems.Cast<TreeNode>().ToArray(); }
		}
		public TreeNode SelectedTreeNode
		{
			get { return SelectedItems.Count > 0 ? SelectedItems[0] as TreeNode : null; }
		}

		public TreeList()
		{
			Rows = new ObservableCollectionAdv<TreeNode>();
			Root = new TreeNode(this, null);
			SetIsExpanded(Root, true);
			ItemsSource = Rows;
			ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
			var descriptor = DependencyPropertyDescriptor.FromProperty(Selector.SelectedItemProperty, typeof(Selector));
			descriptor.AddValueChanged(this, (s, e) =>
			{
				SelectedNode = SelectedItem == null ? null : ((TreeNode)SelectedItem).Tag;
				ScrollIntoView(SelectedItem);
			});
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeListItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeListItem;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			var ti = element as TreeListItem;
			var node = item as TreeNode;
			if (ti != null && node != null)
			{
				ti.Node = node;
				base.PrepareContainerForItemOverride(element, node.Tag);
			}
		}

		private void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
		{
			if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && PendingFocusNode != null)
			{
				var item = ItemContainerGenerator.ContainerFromItem(PendingFocusNode) as TreeListItem;
				if (item != null)
					item.Focus();
				PendingFocusNode = null;
			}
		}

		internal void InsertNewNode(TreeNode parent, ITreeNodeModel tag, int rowIndex, int index)
		{
			TreeNode node = new TreeNode(this, tag);
			if (index >= 0 && index < parent.Children.Count)
				parent.Children.Insert(index, node);
			else
			{
				index = parent.Children.Count;
				parent.Children.Add(node);
			}
			Rows.Insert(rowIndex + index + 1, node);
		}
		internal void SetIsExpanded(TreeNode node, bool value)
		{
			if (value)
			{
				if (!node.IsExpandedOnce)
				{
					node.IsExpandedOnce = true;
					node.AssignIsExpanded(value);
					CreateChildrenNodes(node);
				}
				else
				{
					node.AssignIsExpanded(value);
					CreateChildrenRows(node);
				}
			}
			else
			{
				DropChildrenRows(node, false);
				node.AssignIsExpanded(value);
			}
		}
		private void CreateChildrenRows(TreeNode node)
		{
			int index = Rows.IndexOf(node);
			if (index >= 0 || node == Root)
			{
				var nodes = node.AllVisibleChildren.ToArray();
				Rows.InsertRange(index + 1, nodes);
			}
		}
		internal void CreateChildrenNodes(TreeNode node)
		{
			var children = node == Root ? Source : node.Tag.GetChildren();
			if (children != null)
			{
				int rowIndex = Rows.IndexOf(node);
				node.ChildrenSource = children as INotifyCollectionChanged;
				foreach (ITreeNodeModel obj in children)
				{
					TreeNode child = new TreeNode(this, obj);
					child.HasChildren = obj.HasChildren;
					node.Children.Add(child);
				}
				Rows.InsertRange(rowIndex + 1, node.Children.ToArray());
			}
		}
		internal void DropChildrenRows(TreeNode node, bool removeParent)
		{
			int start = Rows.IndexOf(node);
			if (start >= 0 || node == Root)
			{
				int count = node.VisibleChildrenCount;
				if (removeParent)
					count++;
				else
					start++;
				Rows.RemoveRange(start, count);
			}
		}
	}
}

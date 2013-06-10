using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Common;
using Infrastructure.Common.TreeList;
using System.ComponentModel;
using System.Windows.Data;

namespace Controls.TreeList
{
	public class TreeList : ListView, ITreeList
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
			treeList.Rows.Clear();
			if (e.NewValue != null)
			{
				foreach (TreeNodeViewModel item in (IEnumerable)e.NewValue)
					item.AssignToTree(treeList);
			}
		}

		public static DependencyProperty SelectedTreeNodeProperty = DependencyProperty.Register("SelectedTreeNode", typeof(TreeNodeViewModel), typeof(TreeList), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTreeItemChanged)) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
		public TreeNodeViewModel SelectedTreeNode
		{
			get { return (TreeNodeViewModel)GetValue(SelectedTreeNodeProperty); }
			set { SetValue(SelectedTreeNodeProperty, value); }
		}
		private static void OnSelectedTreeItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var treeList = (TreeList)d;
			treeList.SelectedItem = e.NewValue;
			treeList.ScrollIntoView(treeList.SelectedTreeNode);
		}

		internal ObservableCollectionAdv<TreeNodeViewModel> Rows { get; private set; }
		internal TreeNodeViewModel PendingFocusNode { get; set; }

		public TreeList()
		{
			Rows = new ObservableCollectionAdv<TreeNodeViewModel>();
			SelectionChanged += OnSelectionChanged;
			ItemsSource = Rows;
			ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
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
			var node = item as TreeNodeViewModel;
			if (ti != null && node != null)
			{
				ti.Node = node;
				ti.Tree = this;
				base.PrepareContainerForItemOverride(element, node);
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

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 || PendingFocusNode == null)
			{
				SelectedTreeNode = e.AddedItems.Count > 0 ? e.AddedItems[0] as TreeNodeViewModel : null;
				PendingFocusNode = null;
			}
		}

		#region ITreeList Members

		ObservableCollectionAdv<TreeNodeViewModel> ITreeList.Rows
		{
			get { return Rows; }
		}

		public void SuspendSelection()
		{
			PendingFocusNode = SelectedTreeNode;
		}
		public void ResumeSelection()
		{
			SelectedItem = SelectedTreeNode;
		}

		#endregion
	}
}

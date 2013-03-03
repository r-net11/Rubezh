using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Common.Services.DragDrop
{
	public class TreeListDragDropDecorator : DragDropDecorator
	{
		private TreeView TreeView
		{
			get { return Child as TreeView; }
		}

		public TreeListDragDropDecorator()
		{
		}
		
		protected override void IsTargetChanged()
		{
			base.IsTargetChanged();
			if (Child == null)
				return;
			if (IsTarget)
				Child.PreviewDragOver += new DragEventHandler(OnPreviewDragOver);
			else
				Child.PreviewDragOver += new DragEventHandler(OnPreviewDragOver);
		}
		protected override void StartDrag()
		{
			var data = TreeView.SelectedItem as TreeItemViewModel;
			if (data != null && ValidateStartPoint())
			{
				TreeViewItem draggedItemContainer = GetItemContainer(data);
				var dataObject = new DataObject(data);
				ServiceFactoryBase.DragDropService.DoDragDrop(dataObject, draggedItemContainer, ShowDragVisual, true);
			}
		}

		private void OnPreviewDragOver(object sender, DragEventArgs e)
		{
			//ItemsControl itemsControl = (ItemsControl)sender;
			//if (e.Data.GetDataPresent(ItemType))
			//{
			//    UpdateDragAdorner(e.GetPosition(itemsControl));
			//    UpdateInsertAdorner(itemsControl, e);
			//    HandleDragScrolling(itemsControl, e);
			//}
			//e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ? DragDropEffects.Copy : DragDropEffects.Move;
			e.Effects = DropCommand != null && DropCommand.CanExecute(e.Data) ? DragEffect : DragDropEffects.None;
			e.Handled = true;
		}

		private TreeViewItem GetItemContainer(TreeItemViewModel item)
		{
			if (item == null)
				return null;
			ItemsControl parentContainer = (ItemsControl)GetItemContainer(item.TreeParent) ?? TreeView;
			return parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
		}
		private bool ValidateStartPoint()
		{
			UIElement element = TreeView.InputHitTest(DragStartPoint) as UIElement;
			while (element != null)
			{
				if (element == TreeView)
					return false;
				object data = TreeView.ItemContainerGenerator.ItemFromContainer(element);
				if (data != DependencyProperty.UnsetValue)
					return true;
				else
					element = VisualTreeHelper.GetParent(element) as UIElement;
			}
			return false;
		}
	}
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrastructure.Common.TreeList;

namespace Infrastructure.Common.Services.DragDrop
{
	public class TreeListDragDropDecorator : DragDropDecorator
	{
		public static readonly DependencyProperty TreeItemDropCommandProperty = DependencyProperty.Register("TreeItemDropCommand", typeof(RelayCommand<TreeItemDropObject>), typeof(TreeListDragDropDecorator), new UIPropertyMetadata(null));
		public RelayCommand<TreeItemDropObject> TreeItemDropCommand
		{
			get { return (RelayCommand<TreeItemDropObject>)GetValue(TreeItemDropCommandProperty); }
			set { SetValue(TreeItemDropCommandProperty, value); }
		}

		private const int DRAG_WAIT_COUNTER_LIMIT = 10;
		private const int DRAG_SCROLL_AREA_WIDTH = 25;
		private int _dragScrollWaitCounter;
		private int _dragExpandWaitCounter;
		private ScrollViewer _scrollViewer;
		private TreeItemViewModel _targetElement;
		private TreeViewItem _target;
		private TreeItemDropObject _dropObject;

		private TreeView TreeView
		{
			get { return Child as TreeView; }
		}

		public TreeListDragDropDecorator()
		{
			DragEffect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
		}

		protected override void IsTargetChanged()
		{
			base.IsTargetChanged();
			if (Child == null)
				return;
			if (IsTarget)
			{
				Child.PreviewDragOver += new DragEventHandler(OnPreviewDragOver);
				Child.PreviewDragEnter += new DragEventHandler(OnPreviewDragOver);
				Child.PreviewDragLeave += new DragEventHandler(OnPreviewDragOver);
			}
			else
			{
				Child.PreviewDragLeave += new DragEventHandler(OnPreviewDragOver);
				Child.PreviewDragEnter += new DragEventHandler(OnPreviewDragOver);
				Child.PreviewDragOver += new DragEventHandler(OnPreviewDragOver);
			}
		}

		protected override void StartDrag()
		{
			var data = TreeView.SelectedItem as TreeItemViewModel;
			if (data != null && ValidateStartPoint())
			{
				_dropObject = new TreeItemDropObject();
				TreeViewItem draggedItemContainer = GetItemContainer(data);
				var dataObject = new DataObject(data);
				_dragScrollWaitCounter = 0;
				_dragExpandWaitCounter = 0;
				_scrollViewer = FindScrollViewer(TreeView);
				ServiceFactoryBase.DragDropService.DoDragDrop(dataObject, draggedItemContainer, ShowDragVisual, true, DragEffect);
			}
		}
		protected override void OnDrop(DragEventArgs e)
		{
			if (TreeItemDropCommand != null)
				TreeItemDropCommand.Execute(_dropObject);
		}

		private void OnPreviewDragOver(object sender, DragEventArgs e)
		{
			if (_dropObject != null)
			{
				FindTarget(e);
				HandleDragScrolling(e);
				HandleDragExpanding(e);
				UpdateInsertAdorner(e);
				bool _shiftPressed = (e.KeyStates & DragDropKeyStates.ShiftKey) != 0;
				e.Effects = TreeItemDropCommand != null && TreeItemDropCommand.CanExecute(_dropObject) ? (_shiftPressed ? DragDropEffects.Copy : DragDropEffects.Move) : DragDropEffects.None;
				_targetElement = null;
				e.Handled = true;
			}
		}

		private void FindTarget(DragEventArgs e)
		{
			var point = e.GetPosition(TreeView);
			var obj = TreeView.InputHitTest(point) as DependencyObject;
			while (obj != null && !(obj is TreeViewItem) && !(obj is TreeView))
				obj = VisualTreeHelper.GetParent(obj);
			var container = obj as TreeViewItem;
			_targetElement = container == null ? null : ((ItemsControl)ItemsControl.ItemsControlFromItemContainer(container)).ItemContainerGenerator.ItemFromContainer(container) as TreeItemViewModel;
			if (container != _target)
			{
				_dragExpandWaitCounter = 0;
				_target = container;
			}
			_dropObject.DataObject = e.Data;
			_dropObject.Target = _targetElement;
		}
		private void HandleDragScrolling(DragEventArgs e)
		{
			if (_scrollViewer != null && _scrollViewer.CanContentScroll && _scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible && _scrollViewer.ExtentHeight > _scrollViewer.ViewportHeight)
			{
				bool? isMouseAtTop = IsMousePointerAtTop(e);
				if (isMouseAtTop.HasValue)
				{
					if (_dragScrollWaitCounter == DRAG_WAIT_COUNTER_LIMIT)
					{
						_dragScrollWaitCounter = 8;
						if (isMouseAtTop.Value)
							_scrollViewer.LineUp();
						else
							_scrollViewer.LineDown();
					}
					else
						_dragScrollWaitCounter++;
				}
				else
					_dragScrollWaitCounter = 0;
			}
		}
		private void HandleDragExpanding(DragEventArgs e)
		{
			if (_target != null)
			{
				if (_dragExpandWaitCounter == 3 * DRAG_WAIT_COUNTER_LIMIT)
				{
					_dragExpandWaitCounter = 0;
					_target.IsExpanded = true;
				}
				else
					_dragExpandWaitCounter++;
			}
		}
		private void UpdateInsertAdorner(DragEventArgs e)
		{
			//TODO:
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
		private bool? IsMousePointerAtTop(DragEventArgs e)
		{
			var content = _scrollViewer.Content as IInputElement ?? TreeView;
			if (content != null)
			{
				var point = e.GetPosition(content);
				if (point.Y > 0.0 && point.Y < DRAG_SCROLL_AREA_WIDTH)
					return true;
				else if (point.Y > _scrollViewer.ViewportHeight - DRAG_SCROLL_AREA_WIDTH && point.Y < _scrollViewer.ViewportHeight)
					return false;
			}
			return null;
		}
		private ScrollViewer FindScrollViewer(UIElement element)
		{
			if (element != null)
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
				{
					var child = VisualTreeHelper.GetChild(element, i) as UIElement;
					if (child is ScrollViewer)
						return (ScrollViewer)child;
					child = FindScrollViewer(child);
					if (child != null)
						return child as ScrollViewer;
				}
			return null;
		}
	}
}

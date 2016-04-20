using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrastructure.Common.Windows.TreeList;

namespace Infrastructure.Common.Windows.Services.DragDrop
{
	public class TreeListDragDropDecorator : DragDropDecorator
	{
		public static readonly DependencyProperty TreeNodeDropCommandProperty = DependencyProperty.Register("TreeNodeDropCommand", typeof(RelayCommand<TreeNodeDropObject>), typeof(TreeListDragDropDecorator), new UIPropertyMetadata(null));
		public RelayCommand<TreeNodeDropObject> TreeNodeDropCommand
		{
			get { return (RelayCommand<TreeNodeDropObject>)GetValue(TreeNodeDropCommandProperty); }
			set { SetValue(TreeNodeDropCommandProperty, value); }
		}

		private const int DRAG_WAIT_COUNTER_LIMIT = 10;
		private const double DRAG_SCROLL_AREA_WIDTH = 1.2;
		private int _dragScrollWaitCounter;
		private int _dragExpandWaitCounter;
		private ScrollViewer _scrollViewer;
		private TreeNodeViewModel _targetElement;
		private ListViewItem _target;
		private TreeNodeDropObject _dropObject;

		private ListView TreeList
		{
			get { return Child as ListView; }
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
				Child.PreviewDragLeave -= new DragEventHandler(OnPreviewDragOver);
				Child.PreviewDragEnter -= new DragEventHandler(OnPreviewDragOver);
				Child.PreviewDragOver -= new DragEventHandler(OnPreviewDragOver);
			}
		}

		protected override void StartDrag()
		{
			var data = TreeList.SelectedItem as TreeNodeViewModel;
			if (data != null && ValidateStartPoint())
			{
				_dropObject = new TreeNodeDropObject();
				ListViewItem draggedItemContainer = GetItemContainer(data);
				var dataObject = new DataObject(data);
				_dragScrollWaitCounter = 0;
				_dragExpandWaitCounter = 0;
				_scrollViewer = FindScrollViewer(TreeList);
				ServiceFactoryBase.DragDropService.DoDragDrop(dataObject, draggedItemContainer, ShowDragVisual, true, DragEffect);
			}
		}
		protected override void OnDrop(DragEventArgs e)
		{
			if (TreeNodeDropCommand != null)
				TreeNodeDropCommand.Execute(_dropObject);
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
				e.Effects = TreeNodeDropCommand != null && TreeNodeDropCommand.CanExecute(_dropObject) ? (_shiftPressed ? DragDropEffects.Copy : DragDropEffects.Move) : DragDropEffects.None;
				_targetElement = null;
				e.Handled = true;
			}
		}

		private void FindTarget(DragEventArgs e)
		{
			var point = e.GetPosition(TreeList);
			var obj = TreeList.InputHitTest(point) as DependencyObject;
			while (obj != null && !(obj is ListViewItem) && !(obj is ListView))
				obj = VisualTreeHelper.GetParent(obj);
			var container = obj as ListViewItem;
			_targetElement = container == null ? null : ((ItemsControl)ItemsControl.ItemsControlFromItemContainer(container)).ItemContainerGenerator.ItemFromContainer(container) as TreeNodeViewModel;
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
			if (_scrollViewer != null && _scrollViewer.CanContentScroll && _scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible && _scrollViewer.ExtentHeight > _scrollViewer.ViewportHeight && _target != null)
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
				if (_dragExpandWaitCounter == DRAG_WAIT_COUNTER_LIMIT)
				{
					_dragExpandWaitCounter = 0;
					_targetElement.IsExpanded = true;
				}
				else
					_dragExpandWaitCounter++;
			}
		}
		private void UpdateInsertAdorner(DragEventArgs e)
		{
			//TODO:
		}

		private ListViewItem GetItemContainer(TreeNodeViewModel item)
		{
			if (item == null)
				return null;
			return TreeList.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
		}
		private bool ValidateStartPoint()
		{
			UIElement element = TreeList.InputHitTest(DragStartPoint) as UIElement;
			while (element != null)
			{
				if (element == TreeList)
					return false;
				object data = TreeList.ItemContainerGenerator.ItemFromContainer(element);
				if (data != DependencyProperty.UnsetValue)
					return true;
				else
					element = VisualTreeHelper.GetParent(element) as UIElement;
			}
			return false;
		}
		private bool? IsMousePointerAtTop(DragEventArgs e)
		{
			var content = _scrollViewer.Content as IInputElement ?? TreeList;
			if (content != null)
			{
				var point = e.GetPosition(content);
				var coordingate = point.Y / _target.ActualHeight;
				if (coordingate > 0.0 && coordingate < DRAG_SCROLL_AREA_WIDTH)
					return true;
				else if (coordingate > _scrollViewer.ViewportHeight - DRAG_SCROLL_AREA_WIDTH && coordingate < _scrollViewer.ViewportHeight)
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

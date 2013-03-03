using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace Infrastructure.Common.Services.DragDrop
{
	public class DragDropService : IDragDropService
	{
		public bool IsDragging { get; private set; }
		private bool _dragHasLeftScope;
		private bool _useDefaultCursor;
		private DragAdorner _dragAdorner;
		private FrameworkElement _dragScope;

		public DragDropService()
		{
		}

		public void DoDragDrop(IDataObject dataObject, UIElement dragSource)
		{
			DoDragDrop(dataObject, dragSource, DragDropEffects.None | DragDropEffects.Move | DragDropEffects.Copy);
		}
		public void DoDragDrop(IDataObject dataObject, UIElement dragSource, DragDropEffects effects)
		{
			DoDragDrop(dataObject, dragSource, false, true, effects);
		}
		public void DoDragDrop(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush)
		{
			DoDragDrop(dataObject, dragSource, showDragVisual, useVisualBrush, DragDropEffects.None | DragDropEffects.Move);
		}
		public void DoDragDrop(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, DragDropEffects effects)
		{
			if (!IsDragging)
			{
				_useDefaultCursor = true;
				_dragScope = Application.Current.MainWindow.Content as FrameworkElement;
				bool previousDrop = _dragScope.AllowDrop;
				_dragScope.AllowDrop = true;

				_dragScope.PreviewDragOver += DragScope_PreviewDragOver;
				_dragScope.DragOver += DragScope_DragOver;
				_dragScope.DragLeave += DragScope_DragLeave;
				_dragScope.DragEnter += DragScope_DragEnter;
				_dragScope.GiveFeedback += DragSource_GiveFeedback;
				_dragScope.QueryContinueDrag += DragScope_QueryContinueDrag;

				var _layer = AdornerLayer.GetAdornerLayer(_dragScope);
				if (showDragVisual)
				{
					_dragAdorner = new DragAdorner(_dragScope, dragSource, useVisualBrush);
					_layer.Add(_dragAdorner);
				}

				IsDragging = true;
				_dragHasLeftScope = false;
				dragSource.CaptureMouse();
				DragDropEffects de = System.Windows.DragDrop.DoDragDrop(dragSource, dataObject, effects);
				dragSource.ReleaseMouseCapture();

				if (_dragAdorner != null)
				{
					_layer.Remove(_dragAdorner);
					_dragAdorner = null;
				}
				_dragScope.AllowDrop = previousDrop;

				_dragScope.GiveFeedback -= DragSource_GiveFeedback;
				_dragScope.DragLeave -= DragScope_DragLeave;
				_dragScope.DragLeave -= DragScope_DragEnter;
				_dragScope.QueryContinueDrag -= DragScope_QueryContinueDrag;
				_dragScope.DragOver -= DragScope_DragOver;
				_dragScope.PreviewDragOver -= DragScope_PreviewDragOver;

				IsDragging = false;
			}
		}

		private void DragScope_DragLeave(object sender, DragEventArgs e)
		{
			//if (e.Source == DragScope)
			//{
			//    _dragHasLeftScope = true;
			//    e.Handled = true;
			//}
			if (!e.Handled)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}
		private void DragScope_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Handled)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}
		private void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (e.EscapePressed || _dragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}
		private void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (!_useDefaultCursor && !e.Handled)
			{
				e.UseDefaultCursors = false;
				e.Handled = true;
			}
		}
		private void DragScope_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (_dragAdorner != null)
				_dragAdorner.UpdatePosition(e.GetPosition(_dragScope));
		}
		private void DragScope_DragOver(object sender, DragEventArgs e)
		{
			if (!e.Handled)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}
	}
}

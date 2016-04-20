using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Infrastructure.Common.Windows.Services.DragDrop
{
	public class DragDropService : IDragDropService
	{
		public event DragServiceEventHandler DragOver;
		public event DragServiceEventHandler Drop;
		public event DragCorrectionEventHandler DragCorrection;

		public bool IsDragging { get; private set; }
		private bool _dragHasLeftScope;
		private bool _useDefaultCursor;
		private DragAdorner _dragAdorner;
		private FrameworkElement _dragScope;
		private bool _previousDrop;
		private AdornerLayer _layer;
		private UIElement _dragSource;
		private Action _callback;
		private IDataObject _dataObject;

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

		public void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, Action callback)
		{
			DoDragDropSimulate(dataObject, dragSource, DragDropEffects.None | DragDropEffects.Move | DragDropEffects.Copy, callback);
		}
		public void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, DragDropEffects effects, Action callback)
		{
			DoDragDropSimulate(dataObject, dragSource, false, true, effects, callback);
		}
		public void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, Action callback)
		{
			DoDragDropSimulate(dataObject, dragSource, showDragVisual, useVisualBrush, DragDropEffects.None | DragDropEffects.Move, callback);
		}
		public void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, DragDropEffects effects, Action callback)
		{
			if (!IsDragging)
			{
				_dataObject = dataObject;
				_callback = callback;
				_useDefaultCursor = true;
				_dragScope = Application.Current.MainWindow.Content as FrameworkElement;
				_previousDrop = _dragScope.AllowDrop;
				_dragScope.AllowDrop = true;

				_dragScope.PreviewMouseLeftButtonUp += DragScope_PreviewMouseLeftButtonUp;
				_dragScope.MouseMove += DragScope_MouseMove;
				_dragScope.PreviewKeyDown += DragScope_PreviewKeyDown;

				_layer = AdornerLayer.GetAdornerLayer(_dragScope);
				if (showDragVisual)
				{
					_dragAdorner = new DragAdorner(_dragScope, dragSource, useVisualBrush);
					_layer.Add(_dragAdorner);
					_dragAdorner.UpdatePosition(Mouse.GetPosition(_dragScope), true);
				}

				_dragSource = dragSource;
				IsDragging = true;
				_dragHasLeftScope = false;
				Mouse.OverrideCursor = Cursors.No;
				dragSource.CaptureMouse();
			}
		}
		public void StopDragSimulate(bool cancel)
		{
			if (IsDragging)
			{
				if (_callback != null)
					_callback();
				_dragSource.ReleaseMouseCapture();
				if (_dragAdorner != null)
				{
					_layer.Remove(_dragAdorner);
					_dragAdorner = null;
				}

				_dragScope.PreviewMouseLeftButtonUp -= DragScope_PreviewMouseLeftButtonUp;
				_dragScope.MouseMove -= DragScope_MouseMove;
				_dragScope.PreviewKeyDown -= DragScope_PreviewKeyDown;

				_dragScope.AllowDrop = _previousDrop;
				Mouse.OverrideCursor = null;
				IsDragging = false;
			}
		}

		private void DragScope_DragLeave(object sender, DragEventArgs e)
		{
			//if (e.Source == DragScope)
			//{
			//	_dragHasLeftScope = true;
			//	e.Handled = true;
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
			UpdateAdornerPosition(new DragCorrectionEventArgs(e));
		}
		private void DragScope_DragOver(object sender, DragEventArgs e)
		{
			if (!e.Handled)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}

		private void DragScope_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (IsDragging && Drop != null)
			{
				var ee = new DragServiceEventArgs(_dataObject);
				Drop(this, ee);
				if (ee.Handled)
				{
					Mouse.OverrideCursor = null;
					StopDragSimulate(false);
					e.Handled = true;
				}
			}
		}
		private void DragScope_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsDragging)
			{
				UpdateAdornerPosition(new DragCorrectionEventArgs(_dataObject, e));
				Mouse.OverrideCursor = Cursors.No;
				if (DragOver != null)
				{
					var ee = new DragServiceEventArgs(_dataObject);
					DragOver(this, ee);
					if (ee.Handled)
					{
						Mouse.OverrideCursor = ee.Effects == DragDropEffects.Move ? null : Cursors.No;
						e.Handled = true;
					}
				}
				if (!_useDefaultCursor && !e.Handled)
					e.Handled = true;
			}
		}
		private void DragScope_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				StopDragSimulate(true);
				e.Handled = true;
			}
		}

		private void UpdateAdornerPosition(DragCorrectionEventArgs e)
		{
			if (_dragAdorner != null)
			{
				if (DragCorrection != null)
					DragCorrection(this, e);
				_dragAdorner.UpdatePosition(e.GetPosition(_dragScope) + e.Correction);
			}
		}
	}
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Infrastructure.Common.Services.DragDrop
{
	public class DragDropDecorator : Decorator
	{
		public static readonly DependencyProperty IsSourceProperty = DependencyProperty.Register("IsSource", typeof(bool), typeof(DragDropDecorator), new UIPropertyMetadata(false, IsSourceChanged));
		public static readonly DependencyProperty IsTargetProperty = DependencyProperty.Register("IsTarget", typeof(bool), typeof(DragDropDecorator), new UIPropertyMetadata(false, IsTargetChanged));
		public static readonly DependencyProperty DragCommandProperty = DependencyProperty.Register("DragCommand", typeof(RelayCommand<DataObject>), typeof(DragDropDecorator), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register("DropCommand", typeof(RelayCommand<IDataObject>), typeof(DragDropDecorator), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DragEffectProperty = DependencyProperty.Register("DragEffect", typeof(DragDropEffects), typeof(DragDropDecorator), new UIPropertyMetadata(DragDropEffects.Move));
		public static readonly DependencyProperty ShowDragVisualProperty = DependencyProperty.Register("ShowDragVisual", typeof(bool), typeof(DragDropDecorator), new UIPropertyMetadata(true));
		public static readonly DependencyProperty DragObjectProperty = DependencyProperty.Register("DragObject", typeof(object), typeof(DragDropDecorator), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DragVisualProperty = DependencyProperty.Register("DragVisual", typeof(UIElement), typeof(DragDropDecorator), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DragVisualProviderProperty = DependencyProperty.Register("DragVisualProvider", typeof(Converter<IDataObject, UIElement>), typeof(DragDropDecorator), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DynamicCursorProperty = DependencyProperty.Register("DynamicCursor", typeof(bool), typeof(DragDropDecorator), new UIPropertyMetadata(false));
		public static readonly DependencyProperty AllowSimulateDragProperty = DependencyProperty.Register("AllowSimulateDrag", typeof(bool), typeof(DragDropDecorator), new UIPropertyMetadata(false));

		private static void IsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var decorator = d as DragDropDecorator;
			if (decorator != null && (bool)e.NewValue != (bool)e.OldValue)
				decorator.IsSourceChanged();
		}
		private static void IsTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var decorator = d as DragDropDecorator;
			if (decorator != null && (bool)e.NewValue != (bool)e.OldValue)
				decorator.IsTargetChanged();
		}

		public bool IsSource
		{
			get { return (bool)GetValue(IsSourceProperty); }
			set { SetValue(IsSourceProperty, value); }
		}
		public bool IsTarget
		{
			get { return (bool)GetValue(IsTargetProperty); }
			set { SetValue(IsTargetProperty, value); }
		}
		public RelayCommand<DataObject> DragCommand
		{
			get { return (RelayCommand<DataObject>)GetValue(DragCommandProperty); }
			set { SetValue(DragCommandProperty, value); }
		}
		public RelayCommand<IDataObject> DropCommand
		{
			get { return (RelayCommand<IDataObject>)GetValue(DropCommandProperty); }
			set { SetValue(DropCommandProperty, value); }
		}
		public DragDropEffects DragEffect
		{
			get { return (DragDropEffects)GetValue(DragEffectProperty); }
			set { SetValue(DragEffectProperty, value); }
		}
		public bool ShowDragVisual
		{
			get { return (bool)GetValue(ShowDragVisualProperty); }
			set { SetValue(ShowDragVisualProperty, value); }
		}
		public object DragObject
		{
			get { return (object)GetValue(DragObjectProperty); }
			set { SetValue(DragObjectProperty, value); }
		}
		public UIElement DragVisual
		{
			get { return (UIElement)GetValue(DragVisualProperty); }
			set { SetValue(DragVisualProperty, value); }
		}
		public Converter<IDataObject, UIElement> DragVisualProvider
		{
			get { return (Converter<IDataObject, UIElement>)GetValue(DragVisualProviderProperty); }
			set { SetValue(DragVisualProviderProperty, value); }
		}
		public bool DynamicCursor
		{
			get { return (bool)GetValue(DynamicCursorProperty); }
			set { SetValue(DynamicCursorProperty, value); }
		}
		public bool AllowSimulateDrag
		{
			get { return (bool)GetValue(AllowSimulateDragProperty); }
			set { SetValue(AllowSimulateDragProperty, value); }
		}

		protected Point DragStartPoint { get; private set; }
		public bool IsDragging { get; private set; }

		public DragDropDecorator()
		{
			Loaded += new RoutedEventHandler(DragDropDecorator_Loaded);
		}
		private void DragDropDecorator_Loaded(object sender, RoutedEventArgs e)
		{
			IsTargetChanged();
			IsSourceChanged();
		}

		protected virtual void IsSourceChanged()
		{
			if (Child == null)
				return;
			if (IsSource)
			{
				Child.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);
				Child.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp);
				Child.PreviewMouseMove += new MouseEventHandler(OnPreviewMouseMove);
			}
			else
			{
				Child.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);
				Child.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp);
				Child.PreviewMouseMove -= new MouseEventHandler(OnPreviewMouseMove);
			}
		}
		protected virtual void IsTargetChanged()
		{
			if (Child == null)
				return;
			if (IsTarget)
			{
				Child.AllowDrop = true;
				Child.Drop += new DragEventHandler(OnDrop);
				//Child.QueryContinueDrag += new QueryContinueDragEventHandler(QueryContinueDrag);
				//Child.GiveFeedback += new GiveFeedbackEventHandler(GiveFeedback);
				Child.DragEnter += new DragEventHandler(OnDragEnter);
				Child.DragOver += new DragEventHandler(OnDragOver);
				Child.DragLeave += new DragEventHandler(OnDragLeave);
			}
			else
			{
				Child.ClearValue(FrameworkElement.AllowDropProperty);
				Child.Drop -= new DragEventHandler(OnDrop);
				//Child.QueryContinueDrag -= new QueryContinueDragEventHandler(QueryContinueDrag);
				//Child.GiveFeedback -= new GiveFeedbackEventHandler(GiveFeedback);
				Child.DragEnter -= new DragEventHandler(OnDragEnter);
				Child.DragOver -= new DragEventHandler(OnDragOver);
				Child.DragLeave -= new DragEventHandler(OnDragLeave);
			}
		}

		bool IsPressed { get; set; }
		private void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			UpdateCursor();
			if (IsPressed && e.LeftButton == MouseButtonState.Pressed)
			{
				if (!IsDragging)
				{
					Point position = e.GetPosition(Child);
					if (Math.Abs(position.X - DragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
					{
						IsDragging = true;
						StartDrag();
						IsDragging = false;
						IsPressed = false;
					}
				}
			}
			else
			{
				IsPressed = e.LeftButton == MouseButtonState.Pressed;
			}
		}
		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPoint = e.GetPosition(Child);
		}
		private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (AllowSimulateDrag && !IsDragging && e.LeftButton == MouseButtonState.Released)
			{
				if (ServiceFactoryBase.DragDropService.IsDragging)
					ServiceFactoryBase.DragDropService.StopDragSimulate();
				var position = e.GetPosition(Child);
				if (Math.Abs(position.X - DragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
				{
					IsDragging = true;
					StartSimulateDrag();
				}
			}
		}

		protected virtual void OnDrop(object sender, DragEventArgs e)
		{
			if (DropCommand != null)
				DropCommand.Execute(e.Data);
		}
		private void OnDragOver(object sender, DragEventArgs e)
		{
			e.Effects = DropCommand != null && DropCommand.CanExecute(e.Data) ? DragEffect : DragDropEffects.None;
			e.Handled = true;
		}
		private void OnDragLeave(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}
		private void OnDragEnter(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}

		protected virtual void StartDrag()
		{
			StartDragInternal(true);
		}
		protected virtual void StartSimulateDrag()
		{
			StartDragInternal(false);
		}
		private void StartDragInternal(bool isRealDrag)
		{
			var cancel = false;
			IDataObject data = GetDataObject(out cancel, true);
			if (!cancel)
			{
				var visual = DragVisualProvider == null ? null : DragVisualProvider(data);
				var dataObject = data ?? new DataObject(Child);
				var dragSource = visual ?? DragVisual ?? Child;
				if (isRealDrag)
					ServiceFactoryBase.DragDropService.DoDragDrop(dataObject, dragSource, ShowDragVisual, visual == null, DragEffect);
				else
					ServiceFactoryBase.DragDropService.DoDragDropSimulate(dataObject, dragSource, ShowDragVisual, visual == null, DragEffect, () => IsDragging = false);
			}
			else
				IsDragging = false;
		}
		private IDataObject GetDataObject(out bool cancel, bool drag)
		{
			cancel = false;
			IDataObject data = DragObject as IDataObject;
			if (data == null && DragObject != null)
				data = new DataObject(DragObject);
			if (data == null && DragCommand != null)
			{
				var dataObject = new DataObject();
				if (DragCommand.CanExecute(dataObject))
				{
					if (drag)
						DragCommand.Execute(dataObject);
				}
				else
					cancel = true;
				data = dataObject;
			}
			return data;
		}
		protected virtual void UpdateCursor()
		{
			if (DynamicCursor)
			{
				bool cancel;
				GetDataObject(out cancel, false);
				Cursor = cancel ? Cursors.Arrow : Cursors.Hand;
			}
		}
	}
}

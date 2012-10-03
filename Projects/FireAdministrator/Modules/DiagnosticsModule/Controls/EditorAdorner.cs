using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DiagnosticsModule.Controls
{
	public class EditorAdorner : Adorner
	{
		private EditorPresenter _editorPresenter;
		private VisualCollection _visualChildren;

		public EditorAdorner(EditorPresenter editorPresenter, UIElement adornedElement)
			: base(adornedElement)
		{
			_editorPresenter = editorPresenter;
			_visualChildren = new VisualCollection(this);

			Root = new Control();
			Root.DataContext = editorPresenter.DataContext;
			Root.LayoutUpdated += new EventHandler(OnContentLayoutUpdated);
			var contentTemplateBinding = new Binding();
			contentTemplateBinding.Path = new PropertyPath(EditorPresenter.EditTemplateProperty);
			contentTemplateBinding.Source = editorPresenter;
			Root.SetBinding(Control.TemplateProperty, contentTemplateBinding);

			var contentBinding = new Binding();
			contentBinding.Path = new PropertyPath(EditorPresenter.DataContextProperty);
			contentBinding.Source = AdornedElement;
			Root.SetBinding(ContentControl.ContentProperty, contentBinding);

			_visualChildren.Add(Root);
			AddLogicalChild(Root);
		}

		public FrameworkElement Root { get; private set; }

		protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
		protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }

		protected override Size MeasureOverride(Size constraint)
		{
			Root.IsEnabled = _isVisible;
			if (_isVisible)
			{
				AdornedElement.Measure(constraint);
				Root.Measure(constraint);
				return new Size(AdornedElement.RenderSize.Width, Root.RenderSize.Height);
			}
			else
				return new Size(0, 0);
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			Point location = new Point(0, 0);
			if (_isVisible)
			{
				Rect rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
				Root.Arrange(rect);
			}
			else
				Root.Arrange(new Rect(0, 0, 0, 0));
			return Root.RenderSize;
		}

		private void OnContentLayoutUpdated(object sender, EventArgs e)
		{
			if (_isVisible && !Root.IsKeyboardFocusWithin)
				Root.Focus();
		}

		private bool _isVisible;
		public void UpdateVisibilty(bool isVisislbe)
		{
			_isVisible = isVisislbe;
			InvalidateMeasure();
		}
	}
}

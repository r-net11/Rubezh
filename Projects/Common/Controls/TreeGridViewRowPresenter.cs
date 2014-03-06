using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Controls
{
	public class TreeGridViewRowPresenter : GridViewRowPresenter
	{
		public static DependencyProperty FirstColumnIndentProperty = DependencyProperty.Register("FirstColumnIndent", typeof(Double), typeof(TreeGridViewRowPresenter), new PropertyMetadata(0d));
		public static DependencyProperty ExpanderProperty = DependencyProperty.Register("Expander", typeof(UIElement), typeof(TreeGridViewRowPresenter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnExpanderChanged)));

		private UIElementCollection childs;

		private static PropertyInfo ActualIndexProperty = typeof(GridViewColumn).GetProperty("ActualIndex", BindingFlags.NonPublic | BindingFlags.Instance);
		private static PropertyInfo DesiredWidthProperty = typeof(GridViewColumn).GetProperty("DesiredWidth", BindingFlags.NonPublic | BindingFlags.Instance);

		public TreeGridViewRowPresenter()
		{
			childs = new UIElementCollection(this, this);
			DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(ColumnsProperty, typeof(TreeGridViewRowPresenter));
			if (dpd != null)
				dpd.AddValueChanged(this, (s, e) => EnsureLines());
		}

		public Double FirstColumnIndent
		{
			get { return (Double)GetValue(FirstColumnIndentProperty); }
			set { SetValue(FirstColumnIndentProperty, value); }
		}

		public UIElement Expander
		{
			get { return (UIElement)GetValue(ExpanderProperty); }
			set { SetValue(ExpanderProperty, value); }
		}

		private static void OnExpanderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// Use a second UIElementCollection so base methods work as original
			TreeGridViewRowPresenter p = (TreeGridViewRowPresenter)d;

			if (e.OldValue != null)
				p.childs.Remove(e.OldValue as UIElement);
			if (e.NewValue != null)
				p.childs.Add((UIElement)e.NewValue);
		}

		private static readonly Style DefaultSeparatorStyle;
		public static readonly DependencyProperty SeparatorStyleProperty;
		private readonly List<FrameworkElement> _lines = new List<FrameworkElement>();
		static TreeGridViewRowPresenter()
		{
			DefaultSeparatorStyle = new Style(typeof(Rectangle));
			DefaultSeparatorStyle.Setters.Add(new Setter(Shape.FillProperty, SystemColors.ControlLightBrush));
			DefaultSeparatorStyle.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
			SeparatorStyleProperty = DependencyProperty.Register("SeparatorStyle", typeof(Style), typeof(TreeGridViewRowPresenter), new UIPropertyMetadata(DefaultSeparatorStyle, SeparatorStyleChanged));
		}

		public Style SeparatorStyle
		{
			get { return (Style)GetValue(SeparatorStyleProperty); }
			set { SetValue(SeparatorStyleProperty, value); }
		}
		private IEnumerable<FrameworkElement> Children
		{
			get { return LogicalTreeHelper.GetChildren(this).OfType<FrameworkElement>(); }
		}
		private static void SeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var presenter = (TreeGridViewRowPresenter)d;
			var style = (Style)e.NewValue;
			foreach (FrameworkElement line in presenter._lines)
				line.Style = style;
		}
		private void EnsureLines()
		{
			int count = Columns == null ? 0 : Columns.Count;
			count = count - _lines.Count;
			for (var i = 0; i < count; i++)
			{
				var line = (FrameworkElement)Activator.CreateInstance(SeparatorStyle.TargetType);
				//line = new Rectangle{Fill=Brushes.LightGray};
				line.Style = SeparatorStyle;
				AddVisualChild(line);
				_lines.Add(line);
			}
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			Size s = base.ArrangeOverride(arrangeSize);
			if (Columns == null || Columns.Count == 0)
				return s;
			UIElement expander = Expander;

			//if (Columns != null)
			//	EnsureLines(Columns.Count);

			double current = 0;
			double max = arrangeSize.Width;
			for (int x = 0; x < Columns.Count; x++)
			{
				GridViewColumn column = Columns[x];
				// Actual index needed for column reorder
				UIElement uiColumn = (UIElement)base.GetVisualChild((int)ActualIndexProperty.GetValue(column, null));

				// Compute column width
				double w = Math.Min(max, (Double.IsNaN(column.Width)) ? (double)DesiredWidthProperty.GetValue(column, null) : column.Width);

				// First column indent
				Rect rect;
				if (x == 0 && expander != null)
				{
					double indent = FirstColumnIndent + expander.DesiredSize.Width;
					rect = new Rect(current + indent, 0, w >= indent ? w - indent : 0, arrangeSize.Height);
				}
				else
					rect = new Rect(current, 0, w, arrangeSize.Height);

				uiColumn.Arrange(rect);
				if (x != 0)
				{
					var line = _lines[x];
					Rect lineRect = new Rect(rect.X - 1.5, rect.Y, 1, rect.Height);
					line.Measure(lineRect.Size);
					line.Arrange(lineRect);
				}
				else if (expander != null)
				{
					double ew = FirstColumnIndent + expander.DesiredSize.Width <= w ? expander.DesiredSize.Width : w - FirstColumnIndent;
					if (ew < 0)
						ew = 0;
					expander.Arrange(new Rect(FirstColumnIndent, 0, ew, expander.DesiredSize.Height));
				}
				max -= w;
				current += w;
			}
			return s;
		}
		protected override Size MeasureOverride(Size constraint)
		{
			Size s = base.MeasureOverride(constraint);

			// Measure expander
			UIElement expander = Expander;
			if (expander != null)
			{
				// Compute max measure
				expander.Measure(constraint);
				s.Width = Math.Max(s.Width, expander.DesiredSize.Width);
				s.Height = Math.Max(s.Height, expander.DesiredSize.Height);
			}

			return s;
		}
		protected override Visual GetVisualChild(int index)
		{
			var count = base.VisualChildrenCount;
			if (index < count)
				return base.GetVisualChild(index);
			else if (index - count < _lines.Count)
				return _lines[index - count];
			else
				return Expander;
		}
		protected override int VisualChildrenCount
		{
			get
			{
				if (Expander != null)
					return base.VisualChildrenCount + 1 + _lines.Count;
				else
					return base.VisualChildrenCount + _lines.Count;
			}
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			if (visualAdded != null)
			{
				var textBlock = visualAdded as TextBlock;
				if (textBlock != null)
					textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			}
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}
	}
}

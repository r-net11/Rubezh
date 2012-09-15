using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
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
		}

		public Double FirstColumnIndent
		{
			get { return (Double)this.GetValue(FirstColumnIndentProperty); }
			set { this.SetValue(FirstColumnIndentProperty, value); }
		}

		public UIElement Expander
		{
			get { return (UIElement)this.GetValue(ExpanderProperty); }
			set { this.SetValue(ExpanderProperty, value); }
		}

		private static void OnExpanderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// Use a second UIElementCollection so base methods work as original
			TreeGridViewRowPresenter p = (TreeGridViewRowPresenter)d;

			p.childs.Remove(e.OldValue as UIElement);
			p.childs.Add((UIElement)e.NewValue);
		}

		private static readonly Style DefaultSeparatorStyle;
		public static readonly DependencyProperty SeparatorStyleProperty;
		private readonly List<FrameworkElement> _lines = new List<FrameworkElement>();
		static TreeGridViewRowPresenter()
		{
			DefaultSeparatorStyle = new Style(typeof(Rectangle));
			DefaultSeparatorStyle.Setters.Add(new Setter(Shape.FillProperty, SystemColors.ControlLightBrush));
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
			{
				line.Style = style;
			}
		}
		private void EnsureLines(int count)
		{
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
			if (this.Columns == null || this.Columns.Count == 0)
				return s;
			UIElement expander = this.Expander;

			if (Columns != null)
				EnsureLines(Columns.Count);

			double current = 0;
			double max = arrangeSize.Width;
			for (int x = 0; x < this.Columns.Count; x++)
			{
				GridViewColumn column = this.Columns[x];
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
					Rect lineRect = new Rect(rect.X, rect.Y, 1, rect.Height);
					line.Measure(lineRect.Size);
					line.Arrange(lineRect);
				}
				else if (expander != null)
				{
					double ew = FirstColumnIndent + expander.DesiredSize.Width <= w ? expander.DesiredSize.Width : w - FirstColumnIndent;
					if (ew < 0)
						ew = 0;
					expander.Arrange(new Rect(this.FirstColumnIndent, 0, ew, expander.DesiredSize.Height) );
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
			UIElement expander = this.Expander;
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
			// Last element is always the expander
			// called by render engine
			if (index < count)
				return base.GetVisualChild(index);
			else if (index - count < _lines.Count)
				return _lines[index - count];
			else
				return this.Expander;
		}
		protected override int VisualChildrenCount
		{
			get
			{
				// Last element is always the expander
				if (this.Expander != null)
					return base.VisualChildrenCount + 1 + _lines.Count;
				else
					return base.VisualChildrenCount + _lines.Count;
			}
		}
	}
}

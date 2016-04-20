using Infrastructure.Common.Services;
using Infrastructure.Designer.DesignerItems;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.InstrumentAdorners;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class PointsAdorner : InstrumentAdorner
	{
		private Dictionary<RemoveButton, DesignerItemShape> _pointMap;
		private Dictionary<Shape, DesignerItemShape> _shapeMap;
		public PointsAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override void Show()
		{
			_pointMap = new Dictionary<RemoveButton, DesignerItemShape>();
			_shapeMap = new Dictionary<Shape, DesignerItemShape>();
			AdornerCanvas.Cursor = Cursors.No;
			foreach (var designerItem in DesignerCanvas.Items.OfType<DesignerItemShape>())
			{
				var element = designerItem.Element as ElementBaseShape;
				Shape shape = null;
				switch (element.Type)
				{
					case ElementType.Polygon:
						shape = new Polygon()
						{
							Points = element.Points.ToWindowsPointCollection()
						};
						break;
					case ElementType.Polyline:
						shape = new Polyline()
						{
							Points = element.Points.ToWindowsPointCollection()
						};
						break;
				}
				if (shape != null)
				{
					shape.Stroke = Brushes.Transparent;
					shape.Margin = new Thickness(element.BorderThickness / 2);
					shape.StrokeThickness = element.BorderThickness;
					shape.Cursor = Cursors.Pen;
					shape.DataContext = element;
					AdornerCanvas.Children.Add(shape);
					shape.MouseDown += new MouseButtonEventHandler(CreatePoint);
					_shapeMap.Add(shape, designerItem);
				}
				foreach (var point in element.Points)
					CreatePointMark(designerItem, point);
			}
		}

		private void CreatePoint(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var shape = sender as Shape;
				if (shape != null)
				{
					var designerItem = _shapeMap[shape];
					var element = (ElementBaseShape)designerItem.Element;
					var points = element.Points;

					int index = CalculateIndex(element.Points.ToWindowsPointCollection(), e.GetPosition(AdornerCanvas), element.Type == ElementType.Polygon);
					if (index > -1)
					{
						designerItem.IsSelected = true;
						DesignerCanvas.BeginChange();
						Point point = CalculatePoint(element.Points[index], element.Points[(index + 1) % element.Points.Count], e.GetPosition(AdornerCanvas));
						if (index == points.Count - 1)
							element.Points.Add(point);
						else
							element.Points.Insert(index + 1, point);
						CreatePointMark(designerItem, point);
						designerItem.RefreshPainter();
						DesignerCanvas.EndChange();
						designerItem.IsSelected = false;
						DesignerCanvas.DesignerChanged();
					}
				}
				e.Handled = true;
			}
		}
		private void DeletePoint(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var item = sender as RemoveButton;
				if (item != null)
				{
					var designerItem = _pointMap[item];
					var element = (ElementBaseShape)designerItem.Element;
					var points = element.Points;
					designerItem.IsSelected = true;
					DesignerCanvas.BeginChange();
					AdornerCanvas.Children.Remove(item);
					_pointMap.Remove(item);
					if (points.Count > 2)
					{
						Point point = new Point(Canvas.GetLeft(item), Canvas.GetTop(item));
						points.Remove(point);
						designerItem.RefreshPainter();
					}
					else
					{
						var pair = FindShape(_pointMap, designerItem);
						AdornerCanvas.Children.Remove(pair);
						_pointMap.Remove(pair);
						var shape = FindShape(_shapeMap, designerItem);
						AdornerCanvas.Children.Remove(shape);
						_shapeMap.Remove(shape);
						((DesignerCanvas)DesignerCanvas).RemoveDesignerItem(designerItem);
						ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Publish(new List<ElementBase>() { designerItem.Element });
					}
					DesignerCanvas.EndChange();
					designerItem.IsSelected = false;
					DesignerCanvas.DesignerChanged();
				}
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right && e.ButtonState == MouseButtonState.Released)
				((DesignerCanvas)DesignerCanvas).Toolbox.SetDefault();
		}

		private T FindShape<T>(Dictionary<T, DesignerItemShape> map, DesignerItemShape designerItem)
			where T : class
		{
			foreach (var pair in map)
				if (pair.Value == designerItem)
					return pair.Key;
			return null;
		}
		private int CalculateIndex(PointCollection points, Point point, bool enclose)
		{
			int index = -1;
			double distance = double.MaxValue;
			Point point0 = CalculatePoint(points[0], points[points.Count - 1], point);
			if (enclose && (points[0].X - point0.X) * (points[points.Count - 1].X - point0.X) <= 0)
			{
				distance = Distance(points[0], points[points.Count - 1], point);
				index = points.Count - 1;
			}
			for (int i = 0; i < points.Count - 1; i++)
			{
				double temp = Distance(points[i], points[i + 1], point);
				if (temp < distance)
				{
					point0 = CalculatePoint(points[i], points[i + 1], point);
					if ((points[i].X - point0.X) * (points[i + 1].X - point0.X) <= 0)
					{
						distance = temp;
						index = i;
					}
				}
			}
			return index;
		}
		private double Distance(Point point1, Point point2)
		{
			return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
		}
		private double Distance(Point point1, Point point2, Point point)
		{
			return Math.Abs((point1.Y - point2.Y) * point.X + (point2.X - point1.X) * point.Y + (point1.X * point2.Y - point2.X * point1.Y)) / Distance(point1, point2);
		}
		private Point CalculatePoint(Point pointa, Point pointb, Point pointp)
		{
			if (pointa.X == pointb.X)
				return new Point(pointa.X, pointp.Y);
			if (pointa.Y == pointb.Y)
				return new Point(pointp.X, pointa.Y);
			double x0 = (pointa.X * Math.Pow(pointb.Y - pointa.Y, 2) + pointp.X * Math.Pow(pointb.X - pointa.X, 2) + (pointb.X - pointa.X) * (pointb.Y - pointa.Y) * (pointp.Y - pointa.Y)) / Math.Pow(Distance(pointa, pointb), 2);
			double y0 = (pointb.Y - pointa.Y) * (x0 - pointa.X) / (pointb.X - pointa.X) + pointa.Y;
			return new Point(x0, y0);
		}
		private void CreatePointMark(DesignerItemShape designerItem, Point point)
		{
			var item = new RemoveButton() { DataContext = designerItem.Element, };
			SetSize(item);
			_pointMap.Add(item, designerItem);
			item.MouseDown += new MouseButtonEventHandler(DeletePoint);
			Canvas.SetLeft(item, point.X);
			Canvas.SetTop(item, point.Y);
			AdornerCanvas.Children.Add(item);
		}
		private void SetSize(RemoveButton removeButton)
		{
			removeButton.LayoutTransform = new ScaleTransform(1 / DesignerCanvas.Zoom, 1 / DesignerCanvas.Zoom);
		}
		public override void UpdateZoom()
		{
			base.UpdateZoom();
			foreach (var removeButton in _pointMap.Keys)
				SetSize(removeButton);
		}
	}
}
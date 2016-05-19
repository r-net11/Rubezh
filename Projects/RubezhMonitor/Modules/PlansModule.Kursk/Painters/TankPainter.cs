using Infrastructure.Plans.Painters;
using Infrastructure.Plans.Presenter;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Colors = System.Windows.Media.Colors;

namespace PlansModule.Kursk.Painters
{
	class TankPainter : GeometryPainter<GeometryGroup>
	{
		private PresenterItem _presenterItem;
		private GKDevice _device;
		private static Brush _brush;

		public TankPainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			var elementRectangleTank = presenterItem.Element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				_device = PlanPresenter.Cache.Get<GKDevice>(elementRectangleTank.DeviceUID);
				if (_device != null && _device.State != null)
					_device.State.StateChanged += OnPropertyChanged;
			}
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.Title = GetTooltip();
			UpdateBrush();
		}

		private void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				UpdateBrush();
				_presenterItem.Title = GetTooltip();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		private string GetTooltip()
		{
			if (_device == null)
				return null;
			return _device.PresentationName + "\n" + _device.State.StateClass.ToDescription();
		}
		private void UpdateBrush()
		{
			var state = GetState();
			switch (state)
			{
				case TankState.Full:
					_brush = CreateBrush(0.0);
					break;
				case TankState.Empty:
					_brush = CreateBrush(1.0);
					break;
				case TankState.Little:
					_brush = CreateBrush(2.0 / 3);
					break;
				case TankState.Half:
					_brush = CreateBrush(1.0 / 3);
					break;
				case TankState.Error:
				default:
					_brush = PainterCache.GetTransparentBrush(RubezhAPI.Colors.Black);
					break;
			}
		}
		private Brush CreateBrush(double offset)
		{
			return new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(0, 1),
				GradientStops = new GradientStopCollection()
				{
					new GradientStop()
					{
						Color = Colors.Transparent,
						Offset = 0,
					},
					new GradientStop()
					{
						Color = Colors.Transparent,
						Offset = offset,
					},
					new GradientStop()
					{
						Color = Colors.Blue,
						Offset = offset,
					},
					new GradientStop()
					{
						Color = Colors.Blue,
						Offset = 1,
					},
				}
			};
		}
		private TankState GetState()
		{
			if (_device == null)
				return TankState.Empty;

			switch (_device.State.StateClass)
			{
				case XStateClass.ConnectionLost:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.Failure:
				case XStateClass.Unknown:
					return TankState.Error;
			}
			if (_device.State.AdditionalStates.Count == 0)
				return TankState.Empty;

			if (_device.State.AdditionalStates.Any(x => x.Name == "Аварийный уровень"))
				return TankState.Full;
			if (_device.State.AdditionalStates.Any(x => x.Name == "Высокий уровень"))
				return TankState.Half;
			if (_device.State.AdditionalStates.Any(x => x.Name == "Низкий уровень"))
				return TankState.Little;

			return TankState.Empty;
		}

		protected override GeometryGroup CreateGeometry()
		{
			var geometry = new GeometryGroup();
			geometry.Children.Add(new RectangleGeometry());
			geometry.Children.Add(new RectangleGeometry());
			geometry.Children.Add(new RectangleGeometry());
			return geometry;
		}
		protected override Brush GetBrush()
		{
			return _brush;
		}
		protected override Pen GetPen()
		{
			return DesignerCanvas.PainterCache.ZonePen;
		}
		public override void Transform()
		{
			CalculateRectangle();
			var size = new Size(Rect.Width, Rect.Height / 3);
			((RectangleGeometry)Geometry.Children[0]).Rect = new Rect(Rect.Location, size);
			((RectangleGeometry)Geometry.Children[1]).Rect = new Rect(new Point(Rect.Left, Rect.Top + size.Height), size);
			((RectangleGeometry)Geometry.Children[2]).Rect = new Rect(new Point(Rect.Left, Rect.Top + size.Height * 2), size);
		}
		public override Rect Bounds
		{
			get { return Pen == null ? Rect : new Rect(Rect.Left - Pen.Thickness / 2, Rect.Top - Pen.Thickness / 2, Rect.Width + Pen.Thickness, Rect.Height + Pen.Thickness); }
		}
	}
}
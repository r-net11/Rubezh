using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Controls.Converters;
using System;

namespace PlansModule.Kursk.Designer
{
	class TankPainter : GeometryPainter<GeometryGroup>
	{
		private PresenterItem _presenterItem;
		private XDevice _device;
		private ContextMenu _contextMenu;
		private static Brush _brush;

		public TankPainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			var elementRectangleTank = presenterItem.Element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				_device = Helper.GetXDevice(elementRectangleTank);
				if (_device != null && _device.State != null)
					_device.State.StateChanged += OnPropertyChanged;
			}
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			//_presenterItem.Cursor = Cursors.Hand;
			//_presenterItem.ClickEvent += (s, e) => OnShowProperties();
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
					_brush = PainterCache.GetTransparentBrush(Colors.Black);
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
			return PainterCache.ZonePen;
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

		private ContextMenu CreateContextMenu()
		{
			return _contextMenu;
		}
	}
}
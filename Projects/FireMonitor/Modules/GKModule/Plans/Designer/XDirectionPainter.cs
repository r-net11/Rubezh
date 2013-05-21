using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using Controls.Converters;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;
using System.Windows.Input;
using System.Globalization;

namespace GKModule.Plans.Designer
{
	class XDirectionPainter : RectangleZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private XDirection XDirection;
		private ContextMenu _contextMenu;
		private GeometryDrawing _textDrawing;
		private ScaleTransform _scaleTransform;

		public XDirectionPainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_textDrawing = null;
			_scaleTransform = new ScaleTransform();
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			XDirection = Helper.GetXDirection((ElementXDirection)_presenterItem.Element);
			if (XDirection != null)
				XDirection.DirectionState.StateChanged += OnPropertyChanged;
			_presenterItem.Title = GetDirectionTooltip();
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
		}

		private void OnPropertyChanged()
		{
			_presenterItem.Title = GetDirectionTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		private string GetDirectionTooltip()
		{
			if (XDirection == null)
				return null;
			var sb = new StringBuilder();
			sb.AppendLine(XDirection.PresentationName);
			sb.AppendLine("Состояние: " + XDirection.DirectionState.GetStateType().ToDescription());
			return sb.ToString().TrimEnd();
		}

		#region IPainter Members

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}
		public override void Transform()
		{
			base.Transform();
			var text = XDirection.DirectionState.StateClass.ToDescription();
			if (XDirection.DirectionState.States.Contains(XStateType.TurningOn) && XDirection.DirectionState.OnDelay > 0)
			{
				text += "\n" + string.Format("Задержка: {0} сек", XDirection.DirectionState.OnDelay);
			}
			else if (XDirection.DirectionState.States.Contains(XStateType.On) && XDirection.DirectionState.HoldDelay > 0)
			{
				text += "\n" + string.Format("Удержание: {0} сек", XDirection.DirectionState.HoldDelay);
			}
			if (string.IsNullOrEmpty(text))
				_textDrawing = null;
			else
			{
				var typeface = new Typeface("Arial");
				var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 10, PainterCache.BlackBrush);
				Point point = Geometry.Rect.TopLeft;
				_scaleTransform.CenterX = point.X;
				_scaleTransform.CenterY = point.Y;
				_scaleTransform.ScaleX = Geometry.Rect.Width / formattedText.Width;
				_scaleTransform.ScaleY = Geometry.Rect.Height / formattedText.Height;
				_textDrawing = new GeometryDrawing(PainterCache.BlackBrush, null, null);
				_textDrawing.Geometry = formattedText.BuildGeometry(point);
			}
		}
		protected override void InnerDraw(DrawingContext drawingContext)
		{
			base.InnerDraw(drawingContext);
			if (_textDrawing != null)
			{
				drawingContext.PushTransform(_scaleTransform);
				drawingContext.DrawDrawing(_textDrawing);
				drawingContext.Pop();
			}
		}

		#endregion

		public Color GetStateColor()
		{
			switch(XDirection.DirectionState.StateClass)
			{
				case XStateClass.Unknown:
					return Colors.DarkGray;

				case XStateClass.Norm:
					return Colors.Green;

				case XStateClass.AutoOff:
					return Colors.Gray;

				case XStateClass.Ignore:
					return Colors.Yellow;

				case XStateClass.TurningOn:
					return Colors.Pink;

				case XStateClass.On:
					return Colors.Red;

				default:
					return Colors.White;
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(XDirection.UID);
		}
		bool CanShowInTree()
		{
			return XDirection != null;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(XDirection);
			DialogService.ShowWindow(directionDetailsViewModel);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);
				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в списке",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Свойства",
					Command = ShowPropertiesCommand
				});
			}
			return _contextMenu;
		}
	}
}
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
	class XDirectionPainter : PolygonZonePainter, IPainter
	{
		PresenterItem _presenterItem;
		XDirection Direction;
		ContextMenu _contextMenu;
		GeometryDrawing _textDrawing;
		ScaleTransform _scaleTransform;
		bool _showText = false;

		public XDirectionPainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_textDrawing = null;
			_scaleTransform = new ScaleTransform();
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			Direction = Helper.GetXDirection((IElementDirection)_presenterItem.Element);
			_showText = _presenterItem.Element is ElementRectangleXDirection;
			if (Direction != null)
				Direction.DirectionState.StateChanged += OnPropertyChanged;
			_presenterItem.Title = GetDirectionTooltip();
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
		}

		void OnPropertyChanged()
		{
			_presenterItem.Title = GetDirectionTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		string GetDirectionTooltip()
		{
			if (Direction == null)
				return null;
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Direction.PresentationName);
			stringBuilder.AppendLine("Состояние: " + Direction.DirectionState.StateClass.ToDescription());
			return stringBuilder.ToString().TrimEnd();
		}

		#region IPainter Members

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}
		public override void Transform()
		{
			base.Transform();
			if (_showText)
			{
				var text = Direction.DirectionState.StateClass.ToDescription();
				if (Direction.DirectionState.StateBits.Contains(XStateBit.TurningOn) && Direction.DirectionState.OnDelay > 0)
					text += "\n" + string.Format("Задержка: {0} сек", Direction.DirectionState.OnDelay);
				else if (Direction.DirectionState.StateBits.Contains(XStateBit.On) && Direction.DirectionState.HoldDelay > 0)
					text += "\n" + string.Format("Удержание: {0} сек", Direction.DirectionState.HoldDelay);
				if (string.IsNullOrEmpty(text))
					_textDrawing = null;
				else
				{
					var typeface = new Typeface("Arial");
					var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 10, PainterCache.BlackBrush);
					Point point = Geometry.Bounds.TopLeft;
					_scaleTransform.CenterX = point.X;
					_scaleTransform.CenterY = point.Y;
					_scaleTransform.ScaleX = Geometry.Bounds.Width / formattedText.Width;
					_scaleTransform.ScaleY = Geometry.Bounds.Height / formattedText.Height;
					_textDrawing = new GeometryDrawing(PainterCache.BlackBrush, null, null);
					_textDrawing.Geometry = formattedText.BuildGeometry(point);
				}
			}
			else
				_textDrawing = null;
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
			switch(Direction.DirectionState.StateClass)
			{
				case XStateClass.ConnectionLost:
				case XStateClass.TechnologicalRegime:
				case XStateClass.Unknown:
					return Colors.DarkGray;

				case XStateClass.On:
					return Colors.Red;

				case XStateClass.TurningOn:
					return Colors.Pink;

				case XStateClass.AutoOff:
					return Colors.Gray;
				
				case XStateClass.Ignore:
					return Colors.Yellow;

				case XStateClass.Norm:
				case XStateClass.Off:
					return Colors.Green;

				default:
					return Colors.White;
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(Direction.UID);
		}
		bool CanShowInTree()
		{
			return Direction != null;
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Direction = Direction
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(Direction);
			DialogService.ShowWindow(directionDetailsViewModel);
		}

		ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в списке",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать связанные события",
					Command = ShowJournalCommand
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
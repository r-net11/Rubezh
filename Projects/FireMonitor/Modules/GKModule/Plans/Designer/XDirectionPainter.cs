using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;

namespace GKModule.Plans.Designer
{
	class XDirectionPainter : PolygonZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private XDirection _direction;
		private ContextMenu _contextMenu;
		private DirectionTooltipViewModel _tooltip;
		private GeometryDrawing _textDrawing;
		private ScaleTransform _scaleTransform;
		private bool _showText = false;

		public XDirectionPainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_textDrawing = null;
			_scaleTransform = new ScaleTransform();
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			_direction = Helper.GetXDirection((IElementDirection)_presenterItem.Element);
			_showText = _direction != null && _presenterItem.Element is ElementRectangleXDirection;
			if (_direction != null)
				_direction.State.StateChanged += OnPropertyChanged;
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
			UpdateTooltip();
		}

		private void OnPropertyChanged()
		{
			UpdateTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		private void UpdateTooltip()
		{
			if (_direction == null)
				return;

			if (_tooltip == null)
			{
				_tooltip = new DirectionTooltipViewModel(_direction);
			}
		}

		#region IPainter Members

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}
		public override void Transform()
		{
			if (_direction == null)
				return;

			base.Transform();
			if (_showText)
			{
				var text = _direction.State.StateClass.ToDescription();
				if (_direction.State.StateClasses.Contains(XStateClass.TurningOn) && _direction.State.OnDelay > 0)
					text += "\n" + string.Format("Задержка: {0} сек", _direction.State.OnDelay);
				else if (_direction.State.StateClasses.Contains(XStateClass.On) && _direction.State.HoldDelay > 0)
					text += "\n" + string.Format("Удержание: {0} сек", _direction.State.HoldDelay);
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
			if (_direction == null)
				return Colors.Transparent;

				switch (_direction.State.StateClass)
				{
					case XStateClass.Unknown:
					case XStateClass.DBMissmatch:
					case XStateClass.TechnologicalRegime:
					case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
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
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(_direction.BaseUID);
		}
		private bool CanShowInTree()
		{
			return _direction != null;
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Direction = _direction
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new DirectionDetailsViewModel(_direction));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать в дереве", 
					"pack://application:,,,/Controls;component/Images/BTree.png", 
					ShowInTreeCommand
				));
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать связанные события", 
					"pack://application:,,,/Controls;component/Images/BJournal.png", 
					ShowJournalCommand
				));
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Свойства", 
					"pack://application:,,,/Controls;component/Images/BSettings.png", 
					ShowPropertiesCommand
				));
			}
			return _contextMenu;
		}
	}
}
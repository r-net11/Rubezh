using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Controls;
using FiresecAPI;
using FiresecAPI.Models;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
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
		private ImageTextStateTooltipViewModel _tooltip;
		private GeometryDrawing _textDrawing;
		private ScaleTransform _scaleTransform;
		private bool _showText = false;

		public XDirectionPainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
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
				_direction.DirectionState.StateChanged += OnPropertyChanged;
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
				_tooltip = new ImageTextStateTooltipViewModel();
				_tooltip.TitleViewModel.Title = _direction.PresentationName.TrimEnd();
				_tooltip.TitleViewModel.ImageSource = @"/Controls;component/Images/Blue_Direction.png";
			}
			_tooltip.StateViewModel.Title = _direction.DirectionState.StateClass.ToDescription();
			_tooltip.StateViewModel.ImageSource = _direction.DirectionState.StateClass.ToIconSource();
			_tooltip.Update();
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
				var text = _direction.DirectionState.StateClass.ToDescription();
				if (_direction.DirectionState.StateClasses.Contains(XStateClass.TurningOn) && _direction.DirectionState.OnDelay > 0)
					text += "\n" + string.Format("Задержка: {0} сек", _direction.DirectionState.OnDelay);
				else if (_direction.DirectionState.StateClasses.Contains(XStateClass.On) && _direction.DirectionState.HoldDelay > 0)
					text += "\n" + string.Format("Удержание: {0} сек", _direction.DirectionState.HoldDelay);
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

				switch (_direction.DirectionState.StateClass)
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
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(_direction.UID);
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
			var directionDetailsViewModel = new DirectionDetailsViewModel(_direction);
			DialogService.ShowWindow(directionDetailsViewModel);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = Helper.SetHeader("Показать в дереве", "pack://application:,,,/Controls;component/Images/BTree.png"),
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = Helper.SetHeader("Показать связанные события", "pack://application:,,,/Controls;component/Images/BJournal.png"),
					Command = ShowJournalCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = Helper.SetHeader("Свойства", "pack://application:,,,/Controls;component/Images/BSettings.png"),
					Command = ShowPropertiesCommand
				});
			}
			return _contextMenu;
		}
	}
}
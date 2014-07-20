using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using Infrastructure.Events;

namespace GKModule.Plans.Designer
{
	class XDirectionPainter : BaseZonePainter<XDirection, ShowXDirectionEvent>
	{
		private GeometryDrawing _textDrawing;
		private ScaleTransform _scaleTransform;
		private bool _showText = false;

		public XDirectionPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			_textDrawing = null;
			_scaleTransform = new ScaleTransform();
			_showText = Item != null && presenterItem.Element is ElementRectangleXDirection;
		}

		protected override XDirection CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementDirection;
			return element == null ? null : PlanPresenter.Cache.Get<XDirection>(element.DirectionUID);
		}
		protected override StateTooltipViewModel<XDirection> CreateToolTip()
		{
			return new DirectionTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					ShowJournalCommand
				));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new DirectionDetailsViewModel(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Direction = Item
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		#region IPainter Members

		public override void Transform()
		{
			if (Item == null)
				return;

			base.Transform();
			if (_showText)
			{
				var text = Item.State.StateClass.ToDescription();
				if (Item.State.StateClasses.Contains(XStateClass.TurningOn) && Item.State.OnDelay > 0)
					text += "\n" + string.Format("Задержка: {0} сек", Item.State.OnDelay);
				else if (Item.State.StateClasses.Contains(XStateClass.On) && Item.State.HoldDelay > 0)
					text += "\n" + string.Format("Удержание: {0} сек", Item.State.HoldDelay);
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

		protected override Color GetStateColor()
		{
			if (Item == null)
				return Colors.Transparent;

			switch (Item.State.StateClass)
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
	}
}
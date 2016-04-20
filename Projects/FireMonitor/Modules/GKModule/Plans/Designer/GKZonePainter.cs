using GKModule.ViewModels;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GKModule.Plans.Designer
{
	class GKZonePainter : BaseZonePainter<GKZone, ShowGKZoneEvent>
	{
		ZoneViewModel _zoneViewModel;
		GeometryDrawing _textDrawing;
		ScaleTransform _scaleTransform;
		bool _showState = false;

		public GKZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_zoneViewModel = new ZoneViewModel(Item);
			_textDrawing = null;
			_scaleTransform = new ScaleTransform();

			if (presenterItem.Element is ElementRectangleGKZone)
			{
				var element = presenterItem.Element as ElementRectangleGKZone;
				_showState = element.ShowState;
			}
			else if (presenterItem.Element is ElementPolygonGKZone)
			{
				var element = presenterItem.Element as ElementPolygonGKZone;
				this._showState = element.ShowState;
			}
		}

		protected override GKZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<GKZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<GKZone> CreateToolTip()
		{
			return new ZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Отключить все устройства",
					"pack://application:,,,/Controls;component/Images/BTurnOff.png",
					_zoneViewModel.SetIgnoreAllCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Снять отключения всех устройств",
					"pack://application:,,,/Controls;component/Images/BResetIgnore.png",
					_zoneViewModel.ResetIgnoreAllCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					_zoneViewModel.ShowJournalCommand
				));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new ZoneDetailsViewModel(Item);
		}

		public override void Transform()
		{
			if (Item == null)
				return;

			base.Transform();

			var text = "";
			if (_showState)
			{
				text = Item.State.StateClass.ToDescription();
			}

			if (!string.IsNullOrEmpty(text))
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
			else
			{
				_textDrawing = null;
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
	}
}
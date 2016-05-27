using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using StrazhAPI.GK;
using StrazhAPI.Models;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using VideoModule.ViewModels;
using Color = StrazhAPI.Color;
using Colors = StrazhAPI.Colors;

namespace VideoModule.Plans.Designer
{
	class CameraPainter : BasePointPainter<Camera, ShowCameraEvent>
	{
		public CameraPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
		}

		protected override Camera CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementCamera;
			return element == null ? null : PlanPresenter.Cache.Get<Camera>(element.CameraUID);
		}
		protected override StateTooltipViewModel<Camera> CreateToolTip()
		{
			return new CameraTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new CameraDetailsViewModel(Item);
		}

		protected override Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.CameraPicture.GetBrush(background);
		}

		private Color GetStateColor()
		{
			switch (Item.CameraState.StateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;
				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Colors.Red;
				case XStateClass.Attention:
					return Colors.Yellow;
				case XStateClass.Ignore:
					return Colors.Yellow;
				case XStateClass.Norm:
					return Colors.Green;
				default:
					return Colors.White;
			}
		}
	}
}
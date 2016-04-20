using Common;
using DeviceControls;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using RubezhAPI.Models;
using System.Windows.Controls;
using VideoModule.ViewModels;

namespace VideoModule.Plans.Designer
{
	class CameraPainter : BasePointPainter<Camera, ShowCameraEvent>
	{
		private int rotation = 0;

		public CameraPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{

		}


		protected override Camera CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementCamera;
			this.rotation = element == null ? 0 : element.Rotation;
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
		public override void Transform()
		{
			_rotateTransform.Angle = rotation;
			base.Transform();
		}
		protected override System.Windows.Media.Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.CameraPicture.GetBrush(background);
		}

		private Color GetStateColor()
		{
			if (Item.Status == RviStatus.Error || Item.Status == RviStatus.ConnectionLost || Item.Status == RviStatus.Connecting)
			{
				switch (Item.Status)
				{
					case RviStatus.Error:
						return Colors.DarkGray;
					case RviStatus.ConnectionLost:
						return Colors.Gray;
					case RviStatus.Connecting:
						return Colors.LightGray;
					default:
						return Colors.White;
				}
			}
			else if (Item.IsRecordOnline)
			{
				return Colors.Red;
			}

			else if (Item.IsOnGuard)
			{
				return Colors.DarkBlue;
			}

			else
			{
				return Colors.Green;
			}

		}
	}
}
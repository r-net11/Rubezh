using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using VideoModule.ViewModels;
using XFiresecAPI;
using CameraDetailsViewModel = VideoModule.ViewModels.CameraDetailsViewModel;

namespace VideoModule.Plans.Designer
{
	class CameraPainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private Camera _camera;
		private ContextMenu _contextMenu;
		private CameraTooltipViewModel _tooltip;

		public CameraPainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_contextMenu = null;
			var elementCamera = presenterItem.Element as ElementCamera;
			if (elementCamera != null)
			{
				_camera = Helper.GetCamera(elementCamera);
				//if (_camera != null && _camera.StateClass != null)
				//	_camera.StateChanged += OnPropertyChanged;
			}
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
			UpdateTooltip();
		}

		private void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				UpdateTooltip();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		private void UpdateTooltip()
		{
			if (_camera == null)
				return;
			if (_tooltip == null)
				_tooltip = new CameraTooltipViewModel(_camera);
			_tooltip.OnStateChanged();
		}

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.CameraPicture.GetBrush(background);
		}

		private Color GetStateColor()
		{
			switch (_camera.StateClass)
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

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowCameraEvent>().Publish(_camera.UID);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new CameraDetailsViewModel(_camera));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать в дереве",
					"pack://application:,,,/Controls;component/Images/BTree.png",
					ShowInTreeCommand
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
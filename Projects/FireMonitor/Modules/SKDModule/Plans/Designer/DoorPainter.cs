using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using SKDModule.Events;
using SKDModule.ViewModels;
using Infrastructure.Client.Plans;
using Common;
using FiresecAPI.GK;

namespace SKDModule.Plans.Designer
{
	class DoorPainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private Door _door;
		private ContextMenu _contextMenu;
		private DoorTooltipViewModel _tooltip;

		public DoorPainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_contextMenu = null;
			var elementDoor = presenterItem.Element as ElementDoor;
			if (elementDoor != null)
			{
				_door = PlanPresenter.Cache.Get<Door>(elementDoor.DoorUID);
				var stateClass = _door as IDeviceState<XStateClass>;
				if (stateClass != null)
					stateClass.StateChanged += OnPropertyChanged;
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
			if (_door == null)
				return;

			if (_tooltip == null)
				_tooltip = new DoorTooltipViewModel(_door);
			_tooltip.OnStateChanged();
		}

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return PictureCacheSource.DoorPicture.GetBrush(GetStateBrush());
		}

		public Brush GetStateBrush()
		{
			return Brushes.Green;
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			//ServiceFactory.Events.GetEvent<ShowDoorEvent>().Publish(_door.UID);
		}
		private bool CanShowInTree()
		{
			return _door != null;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			//DialogService.ShowWindow(new DoorDetailsViewModel(_door));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать в дереве",
					"pack://application:,,,/Controls;component/Images/BTree.png",
					ShowInTreeCommand
				));
				_contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Свойства",
					"pack://application:,,,/Controls;component/Images/BSettings.png",
					ShowPropertiesCommand
				));
			}
			return _contextMenu;
		}
	}
}
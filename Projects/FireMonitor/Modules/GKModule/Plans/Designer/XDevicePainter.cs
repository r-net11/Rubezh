using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using System.Windows.Input;

namespace GKModule.Plans.Designer
{
	class XDevicePainter : PointPainter
	{
		PresenterItem _presenterItem;
		XDevice _device;
		ContextMenu _contextMenu;

		public XDevicePainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			var elementXDevice = presenterItem.Element as ElementXDevice;
			if (elementXDevice != null)
			{
				_device = Helper.GetXDevice(elementXDevice);
				if (_device != null && _device.DeviceState != null)
					_device.DeviceState.StateChanged += OnPropertyChanged;
			}
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			_presenterItem.Title = GetDeviceTooltip();
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
		}

		private void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				_presenterItem.Title = GetDeviceTooltip();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		private string GetDeviceTooltip()
		{
			if (_device == null)
				return null;
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(_device.PresentationAddressAndDriver);
			stringBuilder.Append(" - ");
			stringBuilder.AppendLine(_device.Driver.ShortName);
			stringBuilder.AppendLine(_device.DeviceState.StateClass.ToDescription());

			return stringBuilder.ToString().TrimEnd();
		}

		protected override Brush GetBrush()
		{
			return DevicePictureCache.GetDynamicXBrush(_device);
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(_device.UID);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(_device.UID);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в дереве",
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
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

namespace GKModule.Plans.Designer
{
    class XDevicePainter : PointPainter
    {
        private PresenterItem _presenterItem;
        private XDeviceControl _xdeviceControl;
        private XDevice _xdevice;
        private ContextMenu _contextMenu;

        public XDevicePainter(ElementXDevice elementXDevice)
            : base(elementXDevice)
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public void Bind(PresenterItem presenterItem)
        {
            _presenterItem = presenterItem;
            _presenterItem.IsPoint = true;
            _presenterItem.SetBorder(new PresenterBorder(_presenterItem));
            _presenterItem.ContextMenuProvider = CreateContextMenu;
            var elementDevice = presenterItem.Element as ElementXDevice;
            if (elementDevice != null)
            {
                _xdevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.XDeviceUID);
                if (_xdevice != null)
                {
                    _xdeviceControl = new XDeviceControl();
                    _xdeviceControl.XDriverId = _xdevice.DriverUID;
                    _xdeviceControl.XStateClass = _xdevice.DeviceState.StateClass;
                    _xdevice.DeviceState.StateChanged += OnPropertyChanged;
                }
            }
            _presenterItem.Title = GetDeviceTooltip();
        }

        private void OnPropertyChanged()
        {
            _xdeviceControl.XStateClass = _xdevice.DeviceState.StateClass;
            _presenterItem.Title = GetDeviceTooltip();
			//_presenterItem.Redraw();
			_presenterItem.DesignerCanvas.Refresh();
		}
        private string GetDeviceTooltip()
        {
            if (_xdevice == null)
                return null;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(_xdevice.PresentationAddressAndDriver);
            stringBuilder.Append(" - ");
            stringBuilder.AppendLine(_xdevice.Driver.ShortName);

            foreach (var state in _xdevice.DeviceState.States)
                stringBuilder.AppendLine(state.ToDescription());

            return stringBuilder.ToString().TrimEnd();
        }

        protected override Brush GetBrush()
        {
            return DevicePictureCache.GetBrush(_xdevice);
        }

        public RelayCommand ShowInTreeCommand { get; private set; }
        void OnShowInTree()
        {
            ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(_xdevice.UID);
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(_xdevice.UID);
        }

        private ContextMenu CreateContextMenu()
        {
            if (_contextMenu == null)
            {
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
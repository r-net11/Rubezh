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

namespace GKModule.Plans.Designer
{
	class XDevicePainter : IPainter
	{
		private PresenterItem _presenterItem;
		private XDeviceControl _xdeviceControl;
		private XDevice _xdevice;

		public XDevicePainter()
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		public void Bind(PresenterItem presenterItem)
		{
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.Border = BorderHelper.CreateBorderRectangle();
			//_presenterItem.ContextMenu = (ContextMenu)_presenterItem.FindResource("XDeviceMenuView");
			//_presenterItem.ContextMenu.DataContext = this;
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
			_presenterItem.Redraw();
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

		#region IPainter Members

		public bool RedrawOnZoom
		{
			get { return false; }
		}
		public void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
		}
		public UIElement Draw(ElementBase element)
		{
			if (_xdevice == null)
				return null;
			_xdeviceControl.Update();
			return _xdeviceControl;
		}

		#endregion

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
	}
}

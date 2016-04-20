using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Models;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;

namespace DevicesModule.Plans.Designer
{
	class DevicePainter : PointPainter
	{
		PresenterItem _presenterItem;
		Device _device;
		ElementDevice _elementDevice;
		ContextMenu _contextMenu;

		public DevicePainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_contextMenu = null;
			_elementDevice = presenterItem.Element as ElementDevice;
			if (_elementDevice != null)
			{
				_device = Helper.GetDevice(_elementDevice);
				if (_device != null)
				{
					_device.DeviceState.StateChanged += OnPropertyChanged;
					_device.DeviceState.ParametersChanged += OnParametersChanged;
				}
			}
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			_presenterItem.Title = GetDeviceTooltip();
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
		}

		void OnParametersChanged()
		{
			if (_presenterItem != null)
				_presenterItem.Title = GetDeviceTooltip();
		}
		void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				OnParametersChanged();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		string GetDeviceTooltip()
		{
			if (_device == null)
				return null;
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(_device.PresentationAddressAndName);
			stringBuilder.AppendLine("Состояние: " + _device.DeviceState.StateType.ToDescription());

			if (_device.DeviceState.ParentStringStates != null)
				foreach (var parentState in _device.DeviceState.ParentStringStates)
					stringBuilder.AppendLine(parentState);

			foreach (var state in _device.DeviceState.ThreadSafeStates)
				stringBuilder.AppendLine(state.DriverState.Name);

			if (_device.DeviceState.ThreadSafeParameters != null)
				foreach (var parameter in _device.DeviceState.ThreadSafeParameters)
				{
					if (!parameter.IsIgnore && parameter.Visible && parameter.Value != "NAN")
					{
						stringBuilder.Append(parameter.Caption);
						stringBuilder.Append(" - ");
						stringBuilder.AppendLine(parameter.Value);
					}
				}
			return stringBuilder.ToString().TrimEnd();
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.DevicePicture.GetDynamicBrush(_device, _elementDevice.AlternativeLibraryDeviceUID);
		}

		public DeviceState DeviceState
		{
			get { return _device == null ? null : _device.DeviceState; }
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.UID);
		}
		public bool CanDisable()
		{
			return _device == null ? false : FiresecManager.CanDisable(_device.DeviceState);
		}
		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.ChangeDisabled(_device.DeviceState);
		}
		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(new ElementDeviceReference() { DeviceUID = _elementDevice.DeviceUID, AlternativeUID = _elementDevice.AlternativeLibraryDeviceUID });
		}

		ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				DisableCommand = new RelayCommand(OnDisable, CanDisable);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в дереве",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "",
					Command = DisableCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Свойства",
					Command = ShowPropertiesCommand
				});
			}
			((MenuItem)_contextMenu.Items[1]).Header = DeviceState.IsDisabled ? "Снять отключение" : "Отключить";
			return _contextMenu;
		}
	}
}
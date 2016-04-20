using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;

namespace DevicesModule.Plans.Designer
{
	class ZonePainter : PolygonZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private Zone _zone;
		private List<Device> _devices;
		private List<DeviceState> _deviceStates;
		private ContextMenu _contextMenu;

		public ZonePainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			_zone = Helper.GetZone((IElementZone)_presenterItem.Element);
			if (_zone != null)
				_zone.ZoneState.StateChanged += OnPropertyChanged;
			_presenterItem.Title = GetZoneTooltip();
			//_presenterItem.Cursor = Cursors.Hand;
			//_presenterItem.ClickEvent += (s, e) => OnShowProperties();
			InitializeDevices();
		}

		private void OnPropertyChanged()
		{
			_presenterItem.Title = GetZoneTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		private string GetZoneTooltip()
		{
			if (_zone == null)
				return null;
			var sb = new StringBuilder();
			sb.AppendLine(_zone.PresentationName);
			sb.AppendLine("Состояние: " + _zone.ZoneState.StateType.ToDescription());
			if (_zone.ZoneType == ZoneType.Fire)
				sb.AppendLine("Количество датчиков для сработки: " + _zone.DetectorCount.ToString());
			if (_zone.ZoneType == ZoneType.Guard)
			{
				if (FiresecManager.IsZoneOnGuardAlarm(_zone.ZoneState))
					sb.AppendLine("Охранная тревога");
				else
					sb.AppendLine(FiresecManager.IsZoneOnGuard(_zone.ZoneState) ? "На охране" : "Не на охране");
			}
			return sb.ToString().TrimEnd();
		}

		#region IPainter Members

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}

		#endregion

		public Color GetStateColor()
		{
			if (_zone != null && _zone.ZoneState.Zone.ZoneType == ZoneType.Guard)
			{
				if (_zone.ZoneState.StateType == StateType.Norm)
					return Colors.Blue;

				if (FiresecManager.IsZoneOnGuardAlarm(_zone.ZoneState))
					return Colors.Red;

				if (FiresecManager.IsZoneOnGuard(_zone.ZoneState))
					return Colors.DarkGreen;
			}

			StateType stateType = _zone == null ? StateType.Unknown : _zone.ZoneState.StateType;
			switch (stateType)
			{
				case StateType.Fire:
					return Colors.Red;
				case StateType.Service:
				case StateType.Off:
				case StateType.Attention:
					return Colors.Yellow;
				case StateType.Failure:
					return Colors.Pink;
				case StateType.Unknown:
					return Colors.Gray;
				case StateType.Info:
					return Colors.LightBlue;
				case StateType.Norm:
					return Colors.LightGreen;
				case StateType.No:
					return Colors.White;
				default:
					return Colors.Black;
			}
		}
		private void InitializeDevices()
		{
			_devices = new List<Device>();
			_deviceStates = new List<DeviceState>();
			if (_zone != null && FiresecManager.Devices != null)
				Helper.GetDevices(_zone.UID).ForEach(device =>
					{
						if (device != null && device.Driver != null && device.Driver.CanDisable)
						{
							_devices.Add(device);
							_deviceStates.Add(device.DeviceState);
						}
					});
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_zone.UID);
		}
		bool CanShowInTree()
		{
			return _zone != null;
		}

		public RelayCommand DisableAllCommand { get; private set; }
		void OnDisableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.AddToIgnoreList(_devices);
		}
		bool CanDisableAll()
		{
			if (_zone == null || _zone.ZoneType == ZoneType.Guard)
				return false;
			//return (FiresecManager.CheckPermission(PermissionType.Oper_RemoveFromIgnoreList) && _deviceStates.Any(x => !x.IsDisabled));
			return (_deviceStates.Any(x => !x.IsDisabled));
		}

		public RelayCommand EnableAllCommand { get; private set; }
		void OnEnableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.RemoveFromIgnoreList(_devices);
		}
		bool CanEnableAll()
		{
			if (_zone == null || _zone.ZoneType == ZoneType.Guard)
				return false;
			//return (FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList) && _deviceStates.Any(x => x.IsDisabled));
			return (_deviceStates.Any(x => x.IsDisabled));
		}

		public RelayCommand SetGuardCommand { get; private set; }
		void OnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.SetZoneGuard(_zone);
		}
		bool CanSetGuard()
		{
			return _zone != null && _zone.ZoneType == ZoneType.Guard && _zone.SecPanelUID != null && !FiresecManager.IsZoneOnGuard(_zone.ZoneState);
		}

		public RelayCommand UnSetGuardCommand { get; private set; }
		void OnUnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.UnSetZoneGuard(_zone);
		}
		bool CanUnSetGuard()
		{
			return _zone != null && _zone.ZoneType == ZoneType.Guard && _zone.SecPanelUID != null && FiresecManager.IsZoneOnGuard(_zone.ZoneState);
		}

		//public RelayCommand ShowPropertiesCommand { get; private set; }
		//void OnShowProperties()
		//{
		//	var zoneDetailsViewModel = new ZoneDetailsViewModel(_zone);
		//	DialogService.ShowWindow(zoneDetailsViewModel);
		//}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				DisableAllCommand = new RelayCommand(OnDisableAll, CanDisableAll);
				EnableAllCommand = new RelayCommand(OnEnableAll, CanEnableAll);
				SetGuardCommand = new RelayCommand(OnSetGuard, CanSetGuard);
				UnSetGuardCommand = new RelayCommand(OnUnSetGuard, CanUnSetGuard);
				
				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в списке",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Отключить все устройства в зоне",
					Command = DisableAllCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Снять отключение всех устройств в зоне",
					Command = EnableAllCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Поставить на охрану",
					Command = SetGuardCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Снять с охраны",
					Command = UnSetGuardCommand
				});
				//_contextMenu.Items.Add(new MenuItem()
				//{
				//	Header = "Свойства",
				//	Command = ShowPropertiesCommand
				//});
			}
			return _contextMenu;
		}
	}
}

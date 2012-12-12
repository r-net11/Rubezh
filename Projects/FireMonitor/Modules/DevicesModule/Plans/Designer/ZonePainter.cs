using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using System.Windows;
using Infrustructure.Plans.Elements;
using FiresecClient;
using FiresecAPI.Models;
using DeviceControls;
using FiresecAPI;
using Infrustructure.Plans.Presenter;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.Plans.Designer
{
	class ZonePainter : BaseViewModel, IPainter
	{
		private PresenterItem _presenterItem;
		private IPainter _painter;
		private Zone _zone;
		private List<Device> _devices;
		private List<DeviceState> _deviceStates;

		public ZonePainter(PresenterItem presenterItem)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
			DisableAllCommand = new RelayCommand(OnDisableAll, CanDisableAll);
			EnableAllCommand = new RelayCommand(OnEnableAll, CanEnableAll);
			SetGuardCommand = new RelayCommand(OnSetGuard, CanSetGuard);
			UnSetGuardCommand = new RelayCommand(OnUnSetGuard, CanUnSetGuard);
			_presenterItem = presenterItem;
			_painter = presenterItem.Painter;
			Bind();
		}

		private void Bind()
		{
			_presenterItem.Border = BorderHelper.CreateBorderPolyline(_presenterItem.Element);
			_presenterItem.ContextMenu = (ContextMenu)_presenterItem.FindResource("ZoneMenuView");
			_presenterItem.ContextMenu.DataContext = this;
			_zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == ((IElementZone)_presenterItem.Element).ZoneUID);
			if (_zone != null)
				_zone.ZoneState.StateChanged += OnPropertyChanged;
			_presenterItem.Title = GetZoneTooltip();
			InitializeDevices();
		}

		private void OnPropertyChanged()
		{
			_presenterItem.Title = GetZoneTooltip();
			_presenterItem.Redraw();
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

		public Brush GetStateBrush()
		{
			if (_zone != null && _zone.ZoneState.Zone.ZoneType == ZoneType.Guard)
			{
				if (_zone.ZoneState.StateType == StateType.Norm)
					return Brushes.Blue;

				if (FiresecManager.IsZoneOnGuardAlarm(_zone.ZoneState))
					return Brushes.Red;

				if (FiresecManager.IsZoneOnGuard(_zone.ZoneState))
					return Brushes.DarkGreen;
			}

			StateType stateType = _zone == null ? StateType.Unknown : _zone.ZoneState.StateType;
			switch (stateType)
			{
				case StateType.Fire:
					return Brushes.Red;

				case StateType.Attention:
					return Brushes.Yellow;

				case StateType.Failure:
					return Brushes.Pink;

				case StateType.Service:
					return Brushes.Yellow;

				case StateType.Off:
					return Brushes.Yellow;

				case StateType.Unknown:
					return Brushes.Gray;

				case StateType.Info:
					return Brushes.LightBlue;

				case StateType.Norm:
					return Brushes.LightGreen;

				case StateType.No:
					return Brushes.White;

				default:
					return Brushes.Black;
			}
		}

		#region IPainter Members

		public UIElement Draw(ElementBase element)
		{
			if (_zone == null)
				return null;
			var shape = (Shape)_painter.Draw(element);
			shape.Fill = GetStateBrush();
			shape.Opacity = 1;
			return shape;
		}

		#endregion

		private void InitializeDevices()
		{
			_devices = new List<Device>();
			_deviceStates = new List<DeviceState>();
			if (_zone != null && FiresecManager.Devices != null)
				foreach (var device in FiresecManager.Devices)
					if (device != null && device.Driver != null && device.ZoneUID == _zone.UID && device.Driver.CanDisable)
					{
						_devices.Add(device);
						_deviceStates.Add(device.DeviceState);
					}
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
				FiresecManager.FiresecDriver.AddToIgnoreList(_devices);
		}
		bool CanDisableAll()
		{
			if (_zone == null || _zone.ZoneType == ZoneType.Guard)
				return false;
			return (FiresecManager.CheckPermission(PermissionType.Oper_RemoveFromIgnoreList) && _deviceStates.Any(x => !x.IsDisabled));
		}

		public RelayCommand EnableAllCommand { get; private set; }
		void OnEnableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.FiresecDriver.RemoveFromIgnoreList(_devices);
		}
		bool CanEnableAll()
		{
			if (_zone == null || _zone.ZoneType == ZoneType.Guard)
				return false;
			return (FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList) && _deviceStates.Any(x => x.IsDisabled));
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
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using XFiresecAPI;

namespace PlansModule.ViewModels
{
	public class ElementXZoneViewModel : BaseViewModel
	{
		public ElementXZoneView ElementXZoneView { get; private set; }
		public Guid ZoneUID { get; private set; }
		public XZone XZone { get; private set; }
		public XZoneState XZoneState { get; private set; }
		List<Device> devices;
		List<DeviceState> deviceStates;

		public ElementXZoneViewModel(ElementPolygonXZone elementPolygonXZone)
		{
			//ShowInTreeCommand = new RelayCommand(OnShowInTree);
			//DisableAllCommand = new RelayCommand(OnDisableAll, CanDisableAll);
			//EnableAllCommand = new RelayCommand(OnEnableAll, CanEnableAll);
			//SetGuardCommand = new RelayCommand(OnSetGuard, CanSetGuard);
			//UnSetGuardCommand = new RelayCommand(OnUnSetGuard, CanUnSetGuard);

			ZoneUID = elementPolygonXZone.ZoneUID;
			XZone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == ZoneUID);
			if (XZone != null)
			{
				XZoneState = XZone.ZoneState;
				XZoneState.StateChanged += new Action(XZoneState_StateChanged);
			}

			ElementXZoneView = new ElementXZoneView();
			if (elementPolygonXZone.Points == null)
				elementPolygonXZone.Points = new System.Windows.Media.PointCollection();
			ElementXZoneView._polygon.Points = PainterHelper.GetPoints(elementPolygonXZone);
			InitializeDevices();
		}

		void XZoneState_StateChanged()
		{
			OnPropertyChanged("XZoneState");
			OnPropertyChanged("Tooltip");
		}

		public string PresentationName
		{
			get { return "Зона " + XZone.No + "." + XZone.Name; }
		}

		public string Tooltip
		{
			get
			{
				var toolTip = XZone.PresentationName;
				toolTip += "\n" + "Состояние: " + XZoneState.GetStateType().ToDescription();
				//if (XZone.ZoneType == ZoneType.Fire)
				//{
				//    toolTip += "\n" + "Количество датчиков для сработки: " + XZone.DetectorCount.ToString();
				//}
				//if (XZone.ZoneType == ZoneType.Guard)
				//{
				//    if (FiresecManager.IsZoneOnGuardAlarm(XZoneState))
				//        toolTip += "\n" + "Охранная тревога";
				//    else
				//    {
				//        if (FiresecManager.IsZoneOnGuard(XZoneState))
				//            toolTip += "\n" + "На охране";
				//        else
				//            toolTip += "\n" + "Не на охране";
				//    }
				//}
				return toolTip;
			}
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				ElementXZoneView._polygon.StrokeThickness = value ? 1 : 0;
				OnPropertyChanged("IsSelected");

				if (value)
					ElementXZoneView.Flush();
			}
		}

		void InitializeDevices()
		{
			devices = new List<Device>();
			deviceStates = new List<DeviceState>();
			//foreach (var device in FiresecManager.Devices)
			//{
			//    if ((device != null) && (device.Driver != null))
			//    {
			//        if ((device.ZoneUID == ZoneUID) && (device.Driver.CanDisable))
			//        {
			//            devices.Add(device);
			//            deviceStates.Add(device.DeviceState);
			//        }
			//    }
			//}
		}

		//public RelayCommand ShowInTreeCommand { get; private set; }
		//void OnShowInTree()
		//{
		//    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(ZoneUID);
		//}

		//public RelayCommand DisableAllCommand { get; private set; }
		//void OnDisableAll()
		//{
		//    if (ServiceFactory.SecurityService.Validate())
		//        FiresecManager.FiresecDriver.AddToIgnoreList(devices);
		//}
		//bool CanDisableAll()
		//{
		//    if (XZone.ZoneType == ZoneType.Guard)
		//        return false;
		//    return (FiresecManager.CheckPermission(PermissionType.Oper_RemoveFromIgnoreList) && deviceStates.Any(x => !x.IsDisabled));
		//}

		//public RelayCommand EnableAllCommand { get; private set; }
		//void OnEnableAll()
		//{
		//    if (ServiceFactory.SecurityService.Validate())
		//        FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
		//}
		//bool CanEnableAll()
		//{
		//    if (XZone.ZoneType == ZoneType.Guard)
		//        return false;
		//    return (FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList) && deviceStates.Any(x => x.IsDisabled));
		//}

		//public RelayCommand SetGuardCommand { get; private set; }
		//void OnSetGuard()
		//{
		//    if (ServiceFactory.SecurityService.Validate())
		//        FiresecManager.SetZoneGuard(XZone);
		//}
		//bool CanSetGuard()
		//{
		//    return ((XZone.ZoneType == ZoneType.Guard) && (XZone.SecPanelUID != null) && (FiresecManager.IsZoneOnGuard(XZoneState) == false));
		//}

		//public RelayCommand UnSetGuardCommand { get; private set; }
		//void OnUnSetGuard()
		//{
		//    if (ServiceFactory.SecurityService.Validate())
		//        FiresecManager.UnSetZoneGuard(XZone);
		//}
		//bool CanUnSetGuard()
		//{
		//    return ((XZone.ZoneType == ZoneType.Guard) && (XZone.SecPanelUID != null) && (FiresecManager.IsZoneOnGuard(XZoneState) == true));
		//}
	}
}
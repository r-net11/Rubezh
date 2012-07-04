using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.Text;

namespace PlansModule.ViewModels
{
	public class ElementZoneViewModel : BaseViewModel
	{
		public ElementZoneView ElementZoneView { get; private set; }
		public ulong? ZoneNo { get; private set; }
		public Zone Zone { get; private set; }
		public ZoneState ZoneState { get; private set; }
		List<Guid> deviceUIDs;
		List<DeviceState> deviceStates;

		public ElementZoneViewModel(ElementPolygonZone elementPolygonZone)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableAllCommand = new RelayCommand(OnDisableAll, CanDisableAll);
			EnableAllCommand = new RelayCommand(OnEnableAll, CanEnableAll);

			ZoneNo = elementPolygonZone.ZoneNo;
			Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);
			if (Zone != null)
			{
				ZoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);
				if (ZoneState != null)
				{
					ZoneState.StateChanged += new Action(ZoneState_StateChanged);
				}
			}

			ElementZoneView = new ElementZoneView();
			if (elementPolygonZone.Points == null)
				elementPolygonZone.Points = new System.Windows.Media.PointCollection();
			foreach (var polygonPoint in elementPolygonZone.Points)
			{
				ElementZoneView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
			}

			InitializeDevices();
		}

		void ZoneState_StateChanged()
		{
			OnPropertyChanged("ZoneState");
		}

		public string PresentationName
		{
			get { return "Зона " + Zone.No + "." + Zone.Name; }
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				ElementZoneView._polygon.StrokeThickness = value ? 1 : 0;
				OnPropertyChanged("IsSelected");

				if (value)
					ElementZoneView.Flush();
			}
		}

		void InitializeDevices()
		{
			deviceUIDs = new List<Guid>();
			deviceStates = new List<DeviceState>();
			foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
			{
				if ((deviceState.Device != null) && (deviceState.Device.Driver != null))
				{
					if ((deviceState.Device.ZoneNo == ZoneNo) && (deviceState.Device.Driver.CanDisable))
					{
						deviceUIDs.Add(deviceState.UID);
						deviceStates.Add(deviceState);
					}
				}
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(ZoneNo);
		}

		bool CanDisableAll()
		{
			return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList) && deviceStates.Any(x => !x.IsDisabled)) ;
		}

		public RelayCommand DisableAllCommand { get; private set; }
		void OnDisableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.FiresecService.AddToIgnoreList(deviceUIDs);
		}

		bool CanEnableAll()
		{
			return (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_AddToIgnoreList) && deviceStates.Any(x => x.IsDisabled));
		}

		public RelayCommand EnableAllCommand { get; private set; }
		void OnEnableAll()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.FiresecService.RemoveFromIgnoreList(deviceUIDs);
		}
	}
}
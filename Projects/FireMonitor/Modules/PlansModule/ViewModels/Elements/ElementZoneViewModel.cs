using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace PlansModule.ViewModels
{
    public class ElementZoneViewModel : BaseViewModel
    {
        public ElementZoneView ElementZoneView { get; private set; }
        public ulong? ZoneNo { get; private set; }
        public Zone Zone { get; private set; }
        public ZoneState ZoneState { get; private set; }

        public ElementZoneViewModel(ElementPolygonZone elementPolygonZone)
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            DisableCommand = new RelayCommand(OnDisable);
            EnableCommand = new RelayCommand(OnEnable);
            FiresecEventSubscriber.ZoneStateChangedEvent += OnZoneStateChanged;

            ZoneNo = elementPolygonZone.ZoneNo;
            Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);
            ZoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);

            ElementZoneView = new ElementZoneView();
            foreach (var polygonPoint in elementPolygonZone.PolygonPoints)
            {
                ElementZoneView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }

            OnZoneStateChanged(ZoneNo);
        }

        void OnZoneStateChanged(ulong? zoneNo)
        {
            if (ZoneNo == zoneNo)
            {
                StateType = ZoneState.StateType;
            }
        }

        public string PresentationName
        {
            get { return "Зона " + Zone.No + "." + Zone.Name; }
        }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
            }
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
            }
        }

        List<Guid> DevicesToIgnore
        {
            get
            {
                return new List<Guid>(
                       from device in FiresecManager.DeviceConfiguration.Devices
                       where device.ZoneNo == ZoneNo
                       where device.Driver.CanDisable
                       select device.UID);
            }
        }

        public RelayCommand ShowInTreeCommand { get; private set; }
        void OnShowInTree()
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(ZoneNo);
        }

        public RelayCommand DisableCommand { get; private set; }
        void OnDisable()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
                FiresecManager.AddToIgnoreList(DevicesToIgnore);
            }
        }

        public RelayCommand EnableCommand { get; private set; }
        void OnEnable()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
                FiresecManager.RemoveFromIgnoreList(DevicesToIgnore);
            }
        }
    }
}
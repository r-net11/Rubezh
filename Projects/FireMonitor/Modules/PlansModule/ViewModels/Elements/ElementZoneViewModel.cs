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
        public ElementZoneViewModel()
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree);
            DisableCommand = new RelayCommand(OnDisable);
            EnableCommand = new RelayCommand(OnEnable);

            FiresecEventSubscriber.ZoneStateChangedEvent += OnZoneStateChanged;
        }

        public ulong? ZoneNo { get; private set; }
        Zone _zone;
        ElementZoneView _elementZoneView;

        public void Initialize(ElementPolygonZone elementPolygonZone, Canvas canvas)
        {
            ZoneNo = elementPolygonZone.ZoneNo;
            _zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);

            _elementZoneView = new ElementZoneView()
            {
                DataContext = this
            };
            Canvas.SetLeft(_elementZoneView, elementPolygonZone.Left);
            Canvas.SetTop(_elementZoneView, elementPolygonZone.Top);
            _elementZoneView._polygon.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(zonePolygon_PreviewMouseLeftButtonDown);
            foreach (var polygonPoint in elementPolygonZone.PolygonPoints)
            {
                _elementZoneView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            canvas.Children.Add(_elementZoneView);

            OnZoneStateChanged(ZoneNo);
        }

        public string PresentationName
        {
            get { return "Зона " + _zone.No + "." + _zone.Name; }
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
                OnPropertyChanged("IsSelected");
            }
        }

        public event Action Selected;

        void zonePolygon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected != null)
                Selected();
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
            if (ServiceFactory.Get<ISecurityService>().Validate())
            {
                FiresecManager.AddToIgnoreList(DevicesToIgnore);
            }
        }

        public RelayCommand EnableCommand { get; private set; }
        void OnEnable()
        {
            if (ServiceFactory.Get<ISecurityService>().Validate())
            {
                FiresecManager.RemoveFromIgnoreList(DevicesToIgnore);
            }
        }

        void OnZoneStateChanged(ulong? zoneNo)
        {
            if (ZoneNo == zoneNo)
            {
                var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
                if (zoneState != null)
                {
                    StateType = zoneState.StateType;
                }
            }
        }
    }
}
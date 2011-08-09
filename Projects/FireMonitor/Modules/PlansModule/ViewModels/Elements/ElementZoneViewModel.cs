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

            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
        }

        public string ZoneNo { get; private set; }
        Zone _zone;
        ElementZoneView _elementZoneView;

        public void Initialize(ElementZone elementZone, Canvas canvas)
        {
            ZoneNo = elementZone.ZoneNo;
            _zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);

            _elementZoneView = new ElementZoneView();
            _elementZoneView.DataContext = this;
            _elementZoneView._polygon.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(zonePolygon_PreviewMouseLeftButtonDown);
            foreach (var polygonPoint in elementZone.PolygonPoints)
            {
                _elementZoneView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            canvas.Children.Add(_elementZoneView);

            OnZoneStateChanged(ZoneNo);
        }

        public string PresentationName
        {
            get
            {
                return "Зона " + _zone.No + "." + _zone.Name;
            }
        }

        State _state;
        public State State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                //elementZoneView._polygon.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");
            }
        }

        public event Action Selected;

        void zonePolygon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected != null)
                Selected();
        }

        List<string> DevicesToIgnore
        {
            get
            {
                return new List<string>(
                       from device in FiresecManager.DeviceConfiguration.Devices
                       where device.ZoneNo == ZoneNo
                       where device.Driver.CanDisable
                       select device.Id);
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
            bool result = ServiceFactory.Get<ISecurityService>().Check();
            if (result)
            {
                FiresecManager.AddToIgnoreList(DevicesToIgnore);
            }
        }

        public RelayCommand EnableCommand { get; private set; }
        void OnEnable()
        {
            bool result = ServiceFactory.Get<ISecurityService>().Check();
            if (result)
            {
                FiresecManager.RemoveFromIgnoreList(DevicesToIgnore);
            }
        }

        void OnZoneStateChanged(string zoneNo)
        {
            if (ZoneNo == zoneNo)
            {
                var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
                State = zoneState.State;
            }
        }
    }
}
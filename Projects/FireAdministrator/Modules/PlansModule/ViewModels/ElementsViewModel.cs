using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class ElementsViewModel : BaseViewModel
    {
        DesignerCanvas DesignerCanvas;

        public ElementsViewModel(DesignerCanvas designerCanvas)
        {
            DesignerCanvas = designerCanvas;
            Devices = new ObservableCollection<ElementViewModel>();
            Zones = new ObservableCollection<ElementViewModel>();
            SubPlans = new ObservableCollection<ElementViewModel>();
            Elements = new ObservableCollection<ElementViewModel>();

            ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementAdded);
            ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
            //ServiceFactory.Events.GetEvent<PlanChangedEvent>().Subscribe(OnPlansChanged);
            ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
            Update();
        }

        void OnPlansChanged(Guid planUID)
        {
            Update();
        }

        public void Update()
        {
            Devices.Clear();
            Zones.Clear();
            SubPlans.Clear();
            Elements.Clear();

            foreach (var designerItem in DesignerCanvas.Items)
            {
                AddDesignerItem(designerItem);
            }

            IsDevicesVisible = true;
            IsZonesVisible = true;
            IsSubPlansVisible = true;
            IsElementsVisible = true;
            IsDevicesSelectable = true;
            IsZonesSelectable = true;
            IsSubPlansSelectable = true;
            IsElementsSelectable = true;
        }

        void AddDesignerItem(DesignerItem designerItem)
        {
            string name = "";
            var elementBase = designerItem.ElementBase;

            if (elementBase is ElementDevice)
            {
                ElementDevice elementDevice = elementBase as ElementDevice;
                name = elementDevice.Device.DottedAddress + " " + elementDevice.Device.Driver.ShortName;
                Devices.Add(new ElementViewModel(designerItem, name));
            }
            if (elementBase is IElementZone)
            {
                IElementZone elementZone = elementBase as IElementZone;
                if (elementZone.Zone != null)
                {
                    name = elementZone.Zone.PresentationName;
                }
                else
                {
                    name = "Несвязанная зона";
                }
                Zones.Add(new ElementViewModel(designerItem, name));
            }
            if (elementBase is ElementSubPlan)
            {
                ElementSubPlan elementSubPlan = elementBase as ElementSubPlan;
                if (elementSubPlan.Plan != null)
                {
                    name = elementSubPlan.Plan.Caption;
                }
                else
                {
                    name = "Несвязанный подплан";
                }
                SubPlans.Add(new ElementViewModel(designerItem, name));
            }
            if (elementBase is ElementEllipse)
            {
                Elements.Add(new ElementViewModel(designerItem, "Эллипс"));
            }
            if (elementBase is ElementPolygon)
            {
                Elements.Add(new ElementViewModel(designerItem, "Многоугольник"));
            }
            if (elementBase is ElementRectangle)
            {
                Elements.Add(new ElementViewModel(designerItem, "Прямоугольник"));
            }
            if (elementBase is ElementTextBlock)
            {
                Elements.Add(new ElementViewModel(designerItem, "Надпись"));
            }
        }

        public ObservableCollection<ElementViewModel> Devices { get; private set; }
        public ObservableCollection<ElementViewModel> Zones { get; private set; }
        public ObservableCollection<ElementViewModel> SubPlans { get; private set; }
        public ObservableCollection<ElementViewModel> Elements { get; private set; }

        public bool HasDevices
        {
            get { return Devices.Count > 0; }
        }

        public bool HasZones
        {
            get { return Zones.Count > 0; }
        }

        public bool HasSubPlans
        {
            get { return SubPlans.Count > 0; }
        }

        public bool HasElements
        {
            get { return Elements.Count > 0; }
        }

        void UpdateHasItems()
        {
            OnPropertyChanged("HasDevices");
            OnPropertyChanged("HasZones");
            OnPropertyChanged("HasSubPlans");
            OnPropertyChanged("HasElements");
        }

        bool _isDevicesVisible;
        public bool IsDevicesVisible
        {
            get { return _isDevicesVisible; }
            set
            {
                _isDevicesVisible = value;
                foreach (var device in Devices)
                    device.IsVisible = value;
                OnPropertyChanged("IsDevicesVisible");
            }
        }

        bool _isZonesVisible;
        public bool IsZonesVisible
        {
            get { return _isZonesVisible; }
            set
            {
                _isZonesVisible = value;
                foreach (var zone in Zones)
                    zone.IsVisible = value;
                OnPropertyChanged("IsZonesVisible");
            }
        }

        bool _isSubPlansVisible;
        public bool IsSubPlansVisible
        {
            get { return _isSubPlansVisible; }
            set
            {
                _isSubPlansVisible = value;
                foreach (var subPlan in SubPlans)
                    subPlan.IsVisible = value;
                OnPropertyChanged("IsSubPlansVisible");
            }
        }

        bool _isElementsVisible;
        public bool IsElementsVisible
        {
            get { return _isElementsVisible; }
            set
            {
                _isElementsVisible = value;
                foreach (var element in Elements)
                    element.IsVisible = value;
                OnPropertyChanged("IsElementsVisible");
            }
        }

        bool _isDevicesSelectable;
        public bool IsDevicesSelectable
        {
            get { return _isDevicesSelectable; }
            set
            {
                _isDevicesSelectable = value;
                foreach (var device in Devices)
                    device.IsSelectable = value;
                OnPropertyChanged("IsDevicesSelectable");
            }
        }

        bool _isZonesSelectable;
        public bool IsZonesSelectable
        {
            get { return _isZonesSelectable; }
            set
            {
                _isZonesSelectable = value;
                foreach (var zone in Zones)
                    zone.IsSelectable = value;
                OnPropertyChanged("IsZonesSelectable");
            }
        }

        bool _isSubPlansSelectable;
        public bool IsSubPlansSelectable
        {
            get { return _isSubPlansSelectable; }
            set
            {
                _isSubPlansSelectable = value;
                foreach (var subPlan in SubPlans)
                    subPlan.IsSelectable = value;
                OnPropertyChanged("IsSubPlansSelectable");
            }
        }

        bool _isElementsSelectable;
        public bool IsElementsSelectable
        {
            get { return _isElementsSelectable; }
            set
            {
                _isElementsSelectable = value;
                foreach (var element in Elements)
                    element.IsSelectable = value;
                OnPropertyChanged("IsElementsSelectable");
            }
        }

        void OnElementAdded(List<ElementBase> elements)
        {
            foreach (var elementBase in elements)
            {
                var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                if (designerItem != null)
                {
                    AddDesignerItem(designerItem);
                }
            }

            UpdateHasItems();
        }

        void OnElementRemoved(List<ElementBase> elements)
        {
            foreach(var elementBase in elements)
            {
                var device = Devices.FirstOrDefault(x => x.DesignerItem.ElementBase.UID == elementBase.UID);
                if (device != null)
                {
                    Devices.Remove(device);
                }

                var zone = Zones.FirstOrDefault(x => x.DesignerItem.ElementBase.UID == elementBase.UID);
                if (zone != null)
                {
                    Zones.Remove(zone);
                }

                var subPlan = SubPlans.FirstOrDefault(x => x.DesignerItem.ElementBase.UID == elementBase.UID);
                if (subPlan != null)
                {
                    SubPlans.Remove(subPlan);
                }

                var element = Elements.FirstOrDefault(x => x.DesignerItem.ElementBase.UID == elementBase.UID);
                if (element != null)
                {
                    Elements.Remove(element);
                }
            }

            UpdateHasItems();
        }

        void OnElementSelected(List<ElementBase> elements)
        {
        }

        void OnElementChanged(List<ElementBase> elements)
        {

        }
    }
}
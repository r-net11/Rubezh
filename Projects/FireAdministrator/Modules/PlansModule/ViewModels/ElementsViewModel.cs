using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;
using Infrastructure;
using System;

namespace PlansModule.ViewModels
{
    public class ElementsViewModel : DialogContent
    {
        DesignerCanvas DesignerCanvas;

        public ElementsViewModel(DesignerCanvas designerCanvas)
        {
            DesignerCanvas = designerCanvas;
            Title = "Элементы на плане";
            Devices = new ObservableCollection<ElementViewModel>();
            Zones = new ObservableCollection<ElementViewModel>();
            SubPlans = new ObservableCollection<ElementViewModel>();
            Elements = new ObservableCollection<ElementViewModel>();

            ServiceFactory.Events.GetEvent<PlanChangedEvent>().Subscribe(OnPlansChanged);
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

            IsDevicesVisible = true;
            IsZonesVisible = true;
            IsSubPlansVisible = true;
            IsElementsVisible = true;
            IsDevicesSelectable = true;
            IsZonesSelectable = true;
            IsSubPlansSelectable = true;
            IsElementsSelectable = true;
        }

        public ObservableCollection<ElementViewModel> Devices { get; private set; }
        public ObservableCollection<ElementViewModel> Zones { get; private set; }
        public ObservableCollection<ElementViewModel> SubPlans { get; private set; }
        public ObservableCollection<ElementViewModel> Elements { get; private set; }

        bool _isDevicesVisible;
        public bool IsDevicesVisible
        {
            get{return _isDevicesVisible;}
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
            get{return _isDevicesSelectable;}
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
    }
}
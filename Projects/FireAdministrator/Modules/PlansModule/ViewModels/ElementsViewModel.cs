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

            ElementGroupDevices = new ElementGroupViewModel() { Name = "Устройства" };
            ElementGroupZones = new ElementGroupViewModel() { Name = "Зоны" };
            ElementGroupSubPlans = new ElementGroupViewModel() { Name = "Подпланы" };
            ElementGroupElements = new ElementGroupViewModel() { Name = "Элементы" };
            ElementGroups = new List<ElementGroupViewModel>();
            ElementGroups.Add(ElementGroupDevices);
            ElementGroups.Add(ElementGroupZones);
            ElementGroups.Add(ElementGroupSubPlans);
            ElementGroups.Add(ElementGroupElements);

            ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementAdded);
            ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
            ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
            Update();
        }

        public List<ElementGroupViewModel> ElementGroups { get; private set; }
        ElementGroupViewModel ElementGroupDevices;
        ElementGroupViewModel ElementGroupZones;
        ElementGroupViewModel ElementGroupSubPlans;
        ElementGroupViewModel ElementGroupElements;

        void OnPlansChanged(Guid planUID)
        {
            Update();
        }

        public void Update()
        {
            foreach (var elementGroup in ElementGroups)
            {
                elementGroup.Elements.Clear();
            }

            foreach (var designerItem in DesignerCanvas.Items)
            {
                AddDesignerItem(designerItem);
            }
        }

        void AddDesignerItem(DesignerItem designerItem)
        {
            string name = "";
            var elementBase = designerItem.ElementBase;

            if (elementBase is ElementDevice)
            {
                ElementDevice elementDevice = elementBase as ElementDevice;
                name = elementDevice.Device.DottedAddress + " " + elementDevice.Device.Driver.ShortName;
                ElementGroupDevices.Elements.Add(new ElementViewModel(designerItem, name));
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
                ElementGroupZones.Elements.Add(new ElementViewModel(designerItem, name));
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
                ElementGroupSubPlans.Elements.Add(new ElementViewModel(designerItem, name));
            }
            if (elementBase is ElementEllipse)
            {
                ElementGroupElements.Elements.Add(new ElementViewModel(designerItem, "Эллипс"));
            }
            if (elementBase is ElementPolygon)
            {
                ElementGroupElements.Elements.Add(new ElementViewModel(designerItem, "Многоугольник"));
            }
            if (elementBase is ElementRectangle)
            {
                ElementGroupElements.Elements.Add(new ElementViewModel(designerItem, "Прямоугольник"));
            }
            if (elementBase is ElementTextBlock)
            {
                ElementGroupElements.Elements.Add(new ElementViewModel(designerItem, "Надпись"));
            }
        }

        void UpdateHasItems()
        {
            foreach (var elementGroup in ElementGroups)
            {
                elementGroup.UpdateHasElements();
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
                foreach (var elementGroup in ElementGroups)
                {
                    var element = elementGroup.Elements.FirstOrDefault(x => x.DesignerItem.ElementBase.UID == elementBase.UID);
                    if (element != null)
                    {
                        elementGroup.Elements.Remove(element);
                    }
                }
            }

            UpdateHasItems();
        }

        void OnElementChanged(List<ElementBase> elements)
        {
        }

        void OnElementSelected(Guid elementUID)
        {
            foreach (var elementGroup in ElementGroups)
            {
                foreach (var element in elementGroup.Elements)
                {
                    if (element.DesignerItem.ElementBase.UID == elementUID)
                    {
                        elementGroup.IsExpanded = true;
                        elementGroup.SelectedElement = element;
                        return;
                    }
                }
            }
        }
    }
}
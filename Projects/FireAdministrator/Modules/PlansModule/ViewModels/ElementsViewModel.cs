using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<ElementViewModel> Devices { get; private set; }
        public ObservableCollection<ElementViewModel> Zones { get; private set; }
        public ObservableCollection<ElementViewModel> SubPlans { get; private set; }
        public ObservableCollection<ElementViewModel> Elements { get; private set; }

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
                    var plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == elementSubPlan.PlanUID);
                    name = plan.Caption;
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
        }
    }
}

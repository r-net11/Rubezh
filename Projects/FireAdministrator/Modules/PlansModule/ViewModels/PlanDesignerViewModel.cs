using System.Linq;
using System.Windows;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using PlansModule.Designer;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PlansModule.ViewModels
{
    public class PlanDesignerViewModel : BaseViewModel
    {
        public DesignerCanvas DesignerCanvas;
        Plan Plan;

        public void Initialize(Plan plan)
        {
            Plan = plan;
            DesignerCanvas.Width = plan.Width;
            DesignerCanvas.Height = plan.Height;

            foreach (var elementRectangle in plan.ElementRectangles)
            {
                Create(elementRectangle);
            }

            foreach (var elementEllipse in plan.ElementEllipses)
            {
                Create(elementEllipse);
            }

            foreach (var elementTextBlock in plan.ElementTextBlocks)
            {
                Create(elementTextBlock);
            }

            foreach (var elementPolygon in plan.ElementPolygons)
            {
                Create(elementPolygon);
            }

            foreach (var elementZonePolygon in plan.ElementZones)
            {
                //elementZonePolygon.Normalize();
                Create(elementZonePolygon,isOpacity:true);
            }

            foreach (var elementDevice in plan.ElementDevices)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.Id);
                var deviceControl = new DeviceControl()
                {
                    DriverId = device.Driver.UID,
                    StateType = StateType.Norm
                };
                Create(elementDevice, frameworkElement: deviceControl);
            }

            DesignerCanvas.DeselectAll();
        }

        public void Save()
        {
            if (Plan == null)
                return;

            Plan.ElementRectangles = new List<ElementRectangle>();

            var designerItems = from item in DesignerCanvas.Children.OfType<DesignerItem>()
                select item;

            foreach (var designerItem in designerItems)
            {
                if (designerItem.ElementBase is ElementRectangle)
                {
                    ElementRectangle elementRectangle = designerItem.ElementBase as ElementRectangle;
                    elementRectangle.Left = Canvas.GetLeft(designerItem);
                    elementRectangle.Top = Canvas.GetTop(designerItem);
                    elementRectangle.Width = designerItem.Width;
                    elementRectangle.Height = designerItem.Height;
                    Plan.ElementRectangles.Add(elementRectangle);
                }
            }
        }

        void Create(ElementBase elementBase, bool isOpacity = false, FrameworkElement frameworkElement = null)
        {
            if (frameworkElement == null)
            {
                frameworkElement = elementBase.Draw();
            }
            frameworkElement.IsHitTestVisible = false;

            var designerItem = new DesignerItem()
            {
                MinWidth = 10,
                MinHeight = 10,
                Width = elementBase.Width,
                Height = elementBase.Height,
                Content = frameworkElement,
                ElementBase = elementBase,
                IsPolygon = elementBase is ElementPolygon
            };

            if (isOpacity)
                designerItem.Opacity = 0.5;

            DesignerCanvas.SetLeft(designerItem, elementBase.Left);
            DesignerCanvas.SetTop(designerItem, elementBase.Top);
            DesignerCanvas.Children.Add(designerItem);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public class PlanDesignerViewModel : BaseViewModel
    {
        public DesignerCanvas DesignerCanvas;
        Plan Plan;

        public void Initialize(Plan plan)
        {
            Plan = plan;
            DesignerCanvas.Plan = plan;
            DesignerCanvas.Update();
            DesignerCanvas.Children.Clear();
            DesignerCanvas.Width = plan.Width;
            DesignerCanvas.Height = plan.Height;

            foreach (var elementRectangle in plan.ElementRectangles)
            {
               DesignerCanvas.Create(elementRectangle);
            }

            foreach (var elementEllipse in plan.ElementEllipses)
            {
                DesignerCanvas.Create(elementEllipse);
            }

            foreach (var elementTextBlock in plan.ElementTextBlocks)
            {
                DesignerCanvas.Create(elementTextBlock);
            }

            foreach (var elementPolygon in plan.ElementPolygons)
            {
                DesignerCanvas.Create(elementPolygon);
            }

            foreach (var elementRectangleZone in plan.ElementRectangleZones)
            {
                DesignerCanvas.Create(elementRectangleZone, isOpacity: true);
            }

            foreach (var ElementPolygonZone in plan.ElementPolygonZones)
            {
                DesignerCanvas.Create(ElementPolygonZone, isOpacity: true);
            }

            foreach (var elementDevice in plan.ElementDevices)
            {
                //var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.Id);
                //var deviceControl = new DeviceControl()
                //{
                //    DriverId = device.Driver.UID,
                //    StateType = StateType.Norm
                //};
                var deviceControl = new Rectangle()
                {
                    Fill = new SolidColorBrush(Colors.White)
                };
                DesignerCanvas.Create(elementDevice, frameworkElement: deviceControl);
            }

            DesignerCanvas.DeselectAll();
        }

        public void Save()
        {
            if (Plan == null)
                return;

            Plan.ElementRectangles = new List<ElementRectangle>();
            Plan.ElementEllipses = new List<ElementEllipse>();
            Plan.ElementTextBlocks = new List<ElementTextBlock>();
            Plan.ElementPolygons = new List<ElementPolygon>();

            var designerItems = from item in DesignerCanvas.Children.OfType<DesignerItem>()
                select item;

            foreach (var designerItem in designerItems)
            {
                ElementBase elementBase = designerItem.ElementBase;
                elementBase.Left = Canvas.GetLeft(designerItem);
                elementBase.Top = Canvas.GetTop(designerItem);
                elementBase.Width = designerItem.Width;
                elementBase.Height = designerItem.Height;

                if (elementBase is ElementBasePolygon)
                {
                    ElementBasePolygon elementPolygon = elementBase as ElementBasePolygon;
                    elementPolygon.PolygonPoints = new System.Windows.Media.PointCollection((designerItem.Content as Polygon).Points);
                }

                if (elementBase is ElementRectangle)
                {
                    ElementRectangle elementRectangle = elementBase as ElementRectangle;
                    Plan.ElementRectangles.Add(elementRectangle);
                }
                if (elementBase is ElementEllipse)
                {
                    ElementEllipse elementEllipse = elementBase as ElementEllipse;
                    Plan.ElementEllipses.Add(elementEllipse);
                }
                if (elementBase is ElementTextBlock)
                {
                    ElementTextBlock elementTextBlock = elementBase as ElementTextBlock;
                    Plan.ElementTextBlocks.Add(elementTextBlock);
                }
                if (elementBase is ElementPolygon)
                {
                    ElementPolygon elementPolygon = elementBase as ElementPolygon;
                    Plan.ElementPolygons.Add(elementPolygon);
                }
            }
        }
    }
}

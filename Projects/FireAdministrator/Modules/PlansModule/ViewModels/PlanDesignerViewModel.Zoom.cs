using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        public double ZoomFactor = 1;

        public void ChangeZoom(double newZoomFactor)
        {
            double ondZoomFactor = ZoomFactor;
            ZoomFactor = newZoomFactor;
            Zoom(newZoomFactor / ondZoomFactor);
            ZoomFactor = newZoomFactor;
        }

        void Zoom(double zoomFactor)
        {
            DesignerCanvas.Width *= zoomFactor;
            DesignerCanvas.Height *= zoomFactor;
            foreach (var item in DesignerCanvas.Children)
            {
                DesignerItem designerItem = item as DesignerItem;
                Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) * zoomFactor);
                Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) * zoomFactor);
                designerItem.Width *= zoomFactor;
                designerItem.Height *= zoomFactor;
                designerItem.MinWidth *= zoomFactor;
                designerItem.MinHeight *= zoomFactor;

                var pointCollection = new PointCollection();

                if (designerItem.Content is Polygon)
                {
                    Polygon polygon = designerItem.Content as Polygon;
                    pointCollection = new PointCollection();
                    foreach (var point in polygon.Points)
                    {
                        pointCollection.Add(new System.Windows.Point(point.X * zoomFactor, point.Y * zoomFactor));
                    }
                    polygon.Points = pointCollection;
                    designerItem.UpdatePolygonAdorner();
                }

                if (designerItem.ElementBase is ElementPolygon)
                {
                    ElementPolygon elementPolygon = designerItem.ElementBase as ElementPolygon;
                    elementPolygon.PolygonPoints = pointCollection.Clone();
                }

                if (designerItem.ElementBase is ElementPolygonZone)
                {
                    ElementPolygonZone elementPolygonZone = designerItem.ElementBase as ElementPolygonZone;
                    elementPolygonZone.PolygonPoints = pointCollection.Clone();
                }

                if (designerItem.ElementBase is ElementSubPlan)
                {
                    ElementSubPlan elementSubPlan = designerItem.ElementBase as ElementSubPlan;
                    elementSubPlan.PolygonPoints = pointCollection.Clone();
                }

                if ((designerItem.ElementBase is ElementRectangle) ||
                    (designerItem.ElementBase is ElementEllipse) ||
                    (designerItem.ElementBase is ElementTextBlock))
                {
                    var scaleTransform = new ScaleTransform(ZoomFactor, ZoomFactor);
                    (designerItem.Content as System.Windows.FrameworkElement).LayoutTransform = scaleTransform;
                }
            }
        }
    }
}

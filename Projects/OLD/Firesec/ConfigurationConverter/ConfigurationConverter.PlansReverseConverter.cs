using System.Collections.Generic;
using System.Linq;
using Firesec.Models.Plans;
using FiresecAPI.Models;
using Infrastructure.Plans.Elements;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		surfaces ConvertPlansBack(PlansConfiguration plansConfiguration, DeviceConfiguration deviceConfiguration)
		{
			var innerPlansConfiguration = new surfaces();

			var innerPlans = new List<surfacesSurface>();
			foreach (var plan in plansConfiguration.Plans.OfType<Plan>())
			{
				surfacesSurface innerPlan = new surfacesSurface()
				{
					caption = plan.Caption,
					height = (plan.Height / 10).ToString(),
					width = (plan.Width / 10).ToString()
				};
				innerPlans.Add(innerPlan);

				var mainLayer = new surfacesSurfaceLayer()
				{
					name = "План"
				};

				var mainLayerElements = new List<surfacesSurfaceLayerElementsElement>();
				foreach (var elementRectangle in plan.ElementRectangles)
				{
					var innerRectangle = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TSCDeRectangle",
						rect = new surfacesSurfaceLayerElementsElementRect[] { GetRect(elementRectangle) }
					};
					mainLayerElements.Add(innerRectangle);
				}
				foreach (var elementEllipse in plan.ElementEllipses)
				{
					var innerEllipse = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TSCDeEllipse",
						rect = new surfacesSurfaceLayerElementsElementRect[] { GetRect(elementEllipse) }
					};
					mainLayerElements.Add(innerEllipse);
				}
				foreach (var elementTextBlock in plan.ElementTextBlocks)
				{
					var innerLable = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TSCDeLabel",
						rect = new surfacesSurfaceLayerElementsElementRect[] { GetRect(elementTextBlock) },
						caption = elementTextBlock.Text
					};
					innerLable.pen = new surfacesSurfaceLayerElementsElementPen[1];
					innerLable.pen[0] = new surfacesSurfaceLayerElementsElementPen()
					{
						color = "cl" + elementTextBlock.ForegroundColor.ToString()
					};
					innerLable.brush = new surfacesSurfaceLayerElementsElementBrush[1];
					innerLable.brush[0] = new surfacesSurfaceLayerElementsElementBrush()
					{
						color = "cl" + elementTextBlock.BackgroundColor.ToString()
					};
					mainLayerElements.Add(innerLable);
				}
				foreach (var elementPolyline in plan.ElementPolylines)
				{
					var innerPolyline = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TSCDePolyLine",
					};
					var innerPointsCollection = new List<surfacesSurfaceLayerElementsElementPointsPoint>();
					foreach (var point in elementPolyline.Points)
					{
						innerPointsCollection.Add(new surfacesSurfaceLayerElementsElementPointsPoint() { x = point.X.ToString(), y = point.Y.ToString() });
					}
					innerPolyline.points = innerPointsCollection.ToArray();
					mainLayerElements.Add(innerPolyline);
				}
				foreach (var elementPolygon in plan.ElementPolygons)
				{
					var innerPolyline = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TSCDePolygon",
					};
					var innerPointsCollection = new List<surfacesSurfaceLayerElementsElementPointsPoint>();
					foreach (var point in elementPolygon.Points)
					{
						innerPointsCollection.Add(new surfacesSurfaceLayerElementsElementPointsPoint() { x = point.X.ToString(), y = point.Y.ToString() });
					}
					innerPolyline.points = innerPointsCollection.ToArray();
					mainLayerElements.Add(innerPolyline);
				}
				mainLayer.elements = mainLayerElements.ToArray();

				var devicesLayer = new surfacesSurfaceLayer()
				{
					name = "Устройства"
				};
				var innerDeviceElements = new List<surfacesSurfaceLayerElementsElement>();
				foreach (var elementDevice in plan.ElementDevices)
				{
					var innerDeviceElement = new surfacesSurfaceLayerElementsElement();
					var innerRect = new surfacesSurfaceLayerElementsElementRect()
					{
						left = elementDevice.Left.ToString(),
						top = elementDevice.Top.ToString(),
						bottom = (elementDevice.Top + 10).ToString(),
						right = (elementDevice.Left + 10).ToString()
					};
					innerDeviceElement.rect = new surfacesSurfaceLayerElementsElementRect[] { innerRect };

					var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.UID);
					if (device != null)
					{
						innerDeviceElement.id = device.ShapeIds.FirstOrDefault();
					}
					innerDeviceElements.Add(innerDeviceElement);
				}
				devicesLayer.elements = innerDeviceElements.ToArray();

				var zonesLayer = new surfacesSurfaceLayer()
				{
					name = "Зоны"
				};
				var zoneLayerElements = new List<surfacesSurfaceLayerElementsElement>();
				foreach (var elementRectangleZone in plan.ElementRectangleZones)
				{
					var innerRectangleZone = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TFS_ZoneShape",
						rect = new surfacesSurfaceLayerElementsElementRect[] { GetRect(elementRectangleZone) }
					};
					var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.UID == elementRectangleZone.UID);
					if (zone != null)
					{
						innerRectangleZone.id = zone.ShapeIds.FirstOrDefault();
					}
					zoneLayerElements.Add(innerRectangleZone);
				}

				foreach (var elementPolygonZone in plan.ElementPolygonZones)
				{
					var innerPolygonZone = new surfacesSurfaceLayerElementsElement()
					{
						@class = "TFS_PolyZoneShape",
					};
					var innerPointsCollection = new List<surfacesSurfaceLayerElementsElementPointsPoint>();
					foreach (var point in elementPolygonZone.Points)
					{
						innerPointsCollection.Add(new surfacesSurfaceLayerElementsElementPointsPoint() { x = point.X.ToString(), y = point.Y.ToString() });
					}
					innerPolygonZone.points = innerPointsCollection.ToArray();
					var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.UID == elementPolygonZone.UID);
					if (zone != null)
					{
						innerPolygonZone.id = zone.ShapeIds.FirstOrDefault();
					}
					zoneLayerElements.Add(innerPolygonZone);
				}
				zonesLayer.elements = innerDeviceElements.ToArray();

				var innerLayers = new List<surfacesSurfaceLayer>();
				innerLayers.Add(mainLayer);
				innerLayers.Add(devicesLayer);
				innerLayers.Add(zonesLayer);
				innerPlan.layer = innerLayers.ToArray();
			}
			innerPlansConfiguration.surface = innerPlans.ToArray();

			return innerPlansConfiguration;
		}

		surfacesSurfaceLayerElementsElementRect GetRect(ElementBaseRectangle elementBaseRectangle)
		{
			var innerRect = new surfacesSurfaceLayerElementsElementRect()
			{
				left = elementBaseRectangle.Left.ToString(),
				top = elementBaseRectangle.Top.ToString(),
				bottom = (elementBaseRectangle.Top + elementBaseRectangle.Height).ToString(),
				right = (elementBaseRectangle.Left + elementBaseRectangle.Width).ToString()
			};
			return innerRect;
		}
	}
}
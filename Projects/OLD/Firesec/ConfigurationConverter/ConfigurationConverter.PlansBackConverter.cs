using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using Common;
using Firesec.Models.Plans;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using System.Collections.Generic;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		surfaces ConvertPlansBack(PlansConfiguration plansConfiguration, DeviceConfiguration deviceConfiguration)
		{
			surfaces result = new surfaces();
			var innerPlans = new List<surfacesSurface>();

			foreach (var plan in plansConfiguration.AllPlans)
			{
				var innerPlan = new surfacesSurface();
				innerPlans.Add(innerPlan);
				innerPlan.caption = plan.Caption;
				innerPlan.height = (plan.Height / 10).ToString();
				innerPlan.width = (plan.Width / 10).ToString();

				var layers = new List<surfacesSurfaceLayer>();
				var layer = new surfacesSurfaceLayer();
				layer.name = "Зоны";
				layers.Add(layer);
				innerPlan.layer = layers.ToArray();

				var elements = new List<surfacesSurfaceLayerElementsElement>();

				foreach (var elementRectangle in plan.ElementRectangles)
				{
					var element = new surfacesSurfaceLayerElementsElement();
					elements.Add(element);
					element.@class = "TSCDeRectangle";

					element.rect = new surfacesSurfaceLayerElementsElementRect[1]{new surfacesSurfaceLayerElementsElementRect()};
					element.rect[0].left = elementRectangle.Left.ToString();
					element.rect[0].top = elementRectangle.Top.ToString();
					element.rect[0].bottom = (elementRectangle.Top + elementRectangle.Height).ToString();
					element.rect[0].right = (elementRectangle.Left + elementRectangle.Width).ToString();
					element.rect[0].left = elementRectangle.Top.ToString();
				}

				layer.elements = elements.ToArray();
			}

			result.surface = innerPlans.ToArray();
			return result;
		}
	}
}
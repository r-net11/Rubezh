﻿using DevicesModule.Plans.Designer;
using DevicesModule.Plans.ViewModels;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Elements;
using Infrastructure.Plans.InstrumentAdorners;

namespace DevicesModule.Plans.InstrumentAdorners
{
	public class ZoneRectangleAdorner : BaseRectangleAdorner
	{
		private ZonesViewModel _zonesViewModel;
		public ZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetZone(element);
			return element;
		}
	}
}
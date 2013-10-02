using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Services;
using Infrastructure.Common;

namespace Infrastructure.Designer
{
	public static class DesignerLoader
	{
		private static bool _isLoaded = false;

		public static void RegisterResource()
		{
			if (!_isLoaded)
			{
				_isLoaded = true;
				ServiceFactoryBase.ResourceService.AddResource(new ResourceDescription(typeof(DesignerLoader).Assembly, "DataTemplates/Dictionary.xaml"));
				ServiceFactoryBase.ResourceService.AddResource(new ResourceDescription(typeof(DesignerLoader).Assembly, "InstrumentAdorners/GridLineShape.xaml"));
			}
		}
	}
}

using Infrastructure.Common;
using Infrastructure.Common.Services;

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

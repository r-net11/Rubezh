using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;

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
				ServiceFactoryBase.ResourceService.AddResource(typeof(DesignerLoader).Assembly, "DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(DesignerLoader).Assembly, "InstrumentAdorners/GridLineShape.xaml");
			}
		}
	}
}
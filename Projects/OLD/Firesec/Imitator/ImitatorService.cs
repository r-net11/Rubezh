using Firesec.Imitator.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;

namespace Firesec.Imitator
{
	public static class ImitatorService
	{
		static ImitatorService()
		{
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
			//resourceService.AddResource(new ResourceDescription(typeof(Controls.Converters.StateTypeToColorConverter).Assembly, "Themes/Styles.xaml"));
			//resourceService.AddResource(new ResourceDescription(typeof(Controls.Converters.StateTypeToColorConverter).Assembly, "Themes/ScrollBarStyle.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ImitatorService).Assembly, "Imitator/DataTemplates/Dictionary.xaml"));
		}

		public static void Show()
		{
			var imitatorViewModel = new ImitatorViewModel();
			DialogService.ShowWindow(imitatorViewModel);
		}
	}
}
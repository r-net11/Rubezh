using Firesec.Imitator.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common;

namespace Firesec.Imitator
{
	public static class ImitatorService
	{
		static ImitatorService()
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(ImitatorService).Assembly, "Imitator/DataTemplates/Dictionary.xaml"));
		}

		public static void Show()
		{
			var imitatorViewModel = new ImitatorViewModel();
			DialogService.ShowWindow(imitatorViewModel);
		}
	}
}
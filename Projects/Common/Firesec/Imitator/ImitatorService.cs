using Firesec.Imitator.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

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
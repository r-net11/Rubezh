using Firesec.Imitator.ViewModels;
using Infrastructure.Common.Windows;

namespace Firesec.Imitator
{
	public static class ImitatorService
	{
		public static void Show()
		{
			var imitatorViewModel = new ImitatorViewModel();
			DialogService.ShowModalWindow(imitatorViewModel);
		}
	}
}
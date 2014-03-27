using Entities.DeviceOriented;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using VideoModule.Views;

namespace VideoModule.ViewModels
{
	public class PreviewViewModel : SaveCancelDialogViewModel
	{
		public PreviewViewModel(string title)
		{
			Title = title;
		}
	}
}

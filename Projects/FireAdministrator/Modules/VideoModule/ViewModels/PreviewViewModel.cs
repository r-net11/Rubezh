using Infrastructure.Common.Windows.ViewModels;

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
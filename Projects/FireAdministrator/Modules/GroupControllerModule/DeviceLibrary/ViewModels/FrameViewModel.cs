using Infrastructure;
using Infrastructure.Client.Library;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class FrameViewModel : BaseFrameViewModel<LibraryXFrame>
	{
		public FrameViewModel(LibraryXFrame libraryFrame)
			: base(libraryFrame)
		{
		}

		protected override void OnChanged()
		{
			ServiceFactory.SaveService.XLibraryChanged = true;
		}

		protected override void InvalidatePreview()
		{
			LibraryViewModel.Current.InvalidatePreview();
		}
	}
}
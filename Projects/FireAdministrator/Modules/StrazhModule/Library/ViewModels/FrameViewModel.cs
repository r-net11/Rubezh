using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Library;

namespace StrazhModule.ViewModels
{
	public class FrameViewModel : BaseFrameViewModel<SKDLibraryFrame>
	{
		public FrameViewModel(SKDLibraryFrame libraryFrame)
			: base(libraryFrame)
		{
		}

		protected override void OnChanged()
		{
			ServiceFactory.SaveService.SKDLibraryChanged = true;
		}

		protected override void InvalidatePreview()
		{
			LibraryViewModel.Current.InvalidatePreview();
		}
	}
}
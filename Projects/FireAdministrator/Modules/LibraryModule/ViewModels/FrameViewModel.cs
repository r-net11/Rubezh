using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Client.Library;

namespace LibraryModule.ViewModels
{
	public class FrameViewModel : BaseFrameViewModel<LibraryFrame>
	{
		public FrameViewModel(LibraryFrame libraryFrame)
			: base(libraryFrame)
		{
		}

		protected override void OnChanged()
		{
			ServiceFactory.SaveService.LibraryChanged = true;
		}

		protected override void InvalidatePreview()
		{
			LibraryViewModel.Current.InvalidatePreview();
		}
	}
}
using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Client.Library;

namespace GKModule.ViewModels
{
	public class FrameViewModel : BaseFrameViewModel<GKLibraryFrame>
	{
		public FrameViewModel(GKLibraryFrame libraryFrame)
			: base(libraryFrame)
		{
		}

		protected override void OnChanged()
		{
			ServiceFactory.SaveService.GKLibraryChanged = true;
		}

		protected override void InvalidatePreview()
		{
			LibraryViewModel.Current.InvalidatePreview();
		}
	}
}
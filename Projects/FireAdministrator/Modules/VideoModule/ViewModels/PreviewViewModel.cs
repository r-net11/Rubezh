using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class PreviewViewModel : SaveCancelDialogViewModel
	{
		public CellPlayerWrap CellPlayerWrap { get; private set; }

		public PreviewViewModel(string title, CellPlayerWrap cellPlayerWrap)
		{
			Title = title;
			CellPlayerWrap = cellPlayerWrap;
		}
	}
}

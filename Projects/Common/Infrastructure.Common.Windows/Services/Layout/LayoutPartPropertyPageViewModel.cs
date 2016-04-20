using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Services.Layout
{
	public abstract class LayoutPartPropertyPageViewModel : BaseViewModel
	{
		public abstract string Header { get; }

		public abstract void CopyProperties();
		public abstract bool CanSave();
		public abstract bool Save();
	}
}
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2.ConfigurationWriter;

namespace ConfigurationViewer.ViewModels
{
	public class NonPanelUnequalByteViewModel : BaseViewModel
	{
		public NonPanelDatabaseViewModel NonPanelDatabaseViewModel { get; set; }
		public ByteDescription ByteDescription { get; set; }

		public NonPanelUnequalByteViewModel(NonPanelDatabaseViewModel nonPanelDatabaseViewModel, ByteDescription byteDescription)
		{
			ShowUnEqualBytesCommand = new RelayCommand(OnShowUnEqualBytes);
			NonPanelDatabaseViewModel = nonPanelDatabaseViewModel;
			ByteDescription = byteDescription;
		}

		public RelayCommand ShowUnEqualBytesCommand { get; private set; }
		void OnShowUnEqualBytes()
		{
			NonPanelDatabaseViewModel.SelectedByteDescription = ByteDescription;
		}
	}
}
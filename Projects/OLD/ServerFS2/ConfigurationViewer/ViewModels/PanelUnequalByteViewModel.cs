using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ServerFS2.ConfigurationWriter;

namespace ConfigurationViewer.ViewModels
{
	public class PanelUnequalByteViewModel : BaseViewModel
	{
		public PanelDatabaseViewModel PanelDatabaseViewModel { get; set; }
		public bool IsFirstDatabase { get; set; }
		public ByteDescription ByteDescription { get; set; }

		public PanelUnequalByteViewModel(PanelDatabaseViewModel panelDatabaseViewModel, bool isFirstDatabase, ByteDescription byteDescription)
		{
			ShowUnEqualBytesCommand = new RelayCommand(OnShowUnEqualBytes);
			PanelDatabaseViewModel = panelDatabaseViewModel;
			IsFirstDatabase = isFirstDatabase;
			ByteDescription = byteDescription;
		}

		public RelayCommand ShowUnEqualBytesCommand { get; private set; }
		void OnShowUnEqualBytes()
		{
			if (IsFirstDatabase)
			{
				PanelDatabaseViewModel.RomSelectedByteDescription = ByteDescription;
			}
			else
			{
				PanelDatabaseViewModel.FlashSelectedByteDescription = ByteDescription;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace ClientFS2.ConfigurationWriter
{
	public class UnequalByteViewModel : BaseViewModel
	{
		public UnequalByteViewModel(PanelDatabase panelDatabase, ByteDescription byteDescription)
		{
			ShowUnEqualBytesCommand = new RelayCommand(OnShowUnEqualBytes);
			PanelDatabase = panelDatabase;
			ByteDescription = byteDescription;
		}

		public PanelDatabase PanelDatabase { get; set; }
		public ByteDescription ByteDescription { get; set; }

		public RelayCommand ShowUnEqualBytesCommand { get; private set; }
		void OnShowUnEqualBytes()
		{
			PanelDatabase.SelectedByteDescription1 = ByteDescription;
		}
	}
}
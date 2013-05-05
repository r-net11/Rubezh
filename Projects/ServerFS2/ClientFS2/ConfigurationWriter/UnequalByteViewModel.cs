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
		public UnequalByteViewModel(PanelDatabase panelDatabase, bool isFirstDatabase, ByteDescription byteDescription)
		{
			ShowUnEqualBytesCommand = new RelayCommand(OnShowUnEqualBytes);
			PanelDatabase = panelDatabase;
			IsFirstDatabase = isFirstDatabase;
			ByteDescription = byteDescription;
		}

		public PanelDatabase PanelDatabase { get; set; }
		public bool IsFirstDatabase { get; set; }
		public ByteDescription ByteDescription { get; set; }

		public RelayCommand ShowUnEqualBytesCommand { get; private set; }
		void OnShowUnEqualBytes()
		{
			if (IsFirstDatabase)
			{
				PanelDatabase.SelectedByteDescription1 = ByteDescription;
			}
			else
			{
				PanelDatabase.SelectedByteDescription2 = ByteDescription;
			}
		}
	}

	public class BIUnequalByteViewModel : BaseViewModel
	{
		public BIUnequalByteViewModel(BIDatabase biDatabase, ByteDescription byteDescription)
		{
			ShowUnEqualBytesCommand = new RelayCommand(OnShowUnEqualBytes);
			BIDatabase = biDatabase;
			ByteDescription = byteDescription;
		}

		public BIDatabase BIDatabase { get; set; }
		public bool IsFirstDatabase { get; set; }
		public ByteDescription ByteDescription { get; set; }

		public RelayCommand ShowUnEqualBytesCommand { get; private set; }
		void OnShowUnEqualBytes()
		{
			BIDatabase.SelectedByteDescription = ByteDescription;
		}
	}
}
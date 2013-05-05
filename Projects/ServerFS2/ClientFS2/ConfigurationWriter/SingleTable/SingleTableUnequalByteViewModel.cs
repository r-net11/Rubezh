using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace ClientFS2.ConfigurationWriter
{
	public class SingleTableUnequalByteViewModel : BaseViewModel
	{
		public SingleTableUnequalByteViewModel(SingleTable singleTable, ByteDescription byteDescription)
		{
			ShowUnEqualBytesCommand = new RelayCommand(OnShowUnEqualBytes);
			SingleTable = singleTable;
			ByteDescription = byteDescription;
		}

		public SingleTable SingleTable { get; set; }
		public bool IsFirstDatabase { get; set; }
		public ByteDescription ByteDescription { get; set; }

		public RelayCommand ShowUnEqualBytesCommand { get; private set; }
		void OnShowUnEqualBytes()
		{
			SingleTable.SelectedByteDescription = ByteDescription;
		}
	}
}
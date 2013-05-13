using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using System.IO;

namespace ClientFS2.ConfigurationWriter
{
	public class SingleTable : BaseViewModel
	{
		public ConfigurationWriterHelper ConfigurationWriterHelper { get; set; }

		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase { get; set; }
		public List<ByteDescription> RootBytes { get; set; }

		public List<BytesDatabase> Tables = new List<BytesDatabase>();
		public BytesDatabase FirstTable;
		public ByteDescription Crc16ByteDescription;

		public List<SingleTableUnequalByteViewModel> UnequalBytes { get; set; }

		public SingleTable()
		{
			UnequalBytes = new List<SingleTableUnequalByteViewModel>();
		}

		ByteDescription _selectedByteDescription;
		public ByteDescription SelectedByteDescription
		{
			get { return _selectedByteDescription; }
			set
			{
				_selectedByteDescription = value;
				OnPropertyChanged("SelectedByteDescription");
			}
		}

		public void CreateRootBytes()
		{
			RootBytes = new List<ByteDescription>();
			var startOffset = 0;
			BytesDatabase.Order(startOffset);
			BytesDatabase.ResolveTableReferences();
			BytesDatabase.ResolverReferences();
		}

		public void MergeDatabase()
		{
			foreach (var byteDescription in BytesDatabase.ByteDescriptions)
			{
				byteDescription.IsNotEqualToOriginal = true;
			}

            var panelName = ParentPanel.Driver.ShortName + " " + ParentPanel.DottedAddress;
            if (!string.IsNullOrEmpty(ParentPanel.Description))
            {
                panelName += " (" + ParentPanel.Description + ")";
            }
            var fileName = @"C:\Program Files\Firesec\TstData\" + panelName + ".bin";
			if (File.Exists(fileName))
			{
				var byteArray = File.ReadAllBytes(fileName);
				if (byteArray != null)
				{
					var bytes = byteArray.ToList();
					var emptyBytes = new List<byte>();
					for (int i = 0; i < 0x4000; i++)
					{
						emptyBytes.Add(0);
					}
					bytes.InsertRange(0, emptyBytes);

					foreach (var byteDescription in BytesDatabase.ByteDescriptions)
					{
						if (bytes.Count > byteDescription.Offset)
						{
							var originalByte = bytes[byteDescription.Offset];
							byteDescription.OriginalValue = originalByte;
							if (byteDescription.Value == originalByte)
							{
								byteDescription.IsNotEqualToOriginal = false;
							}
							else if (!byteDescription.IsReadOnly)
							{
								var unequalByteViewModel = new SingleTableUnequalByteViewModel(this, byteDescription);
								UnequalBytes.Add(unequalByteViewModel);
							}
						}
					}
				}
			}

			BytesDatabase.ByteDescriptions.RemoveRange(0, 0x4000);
		}
	}
}
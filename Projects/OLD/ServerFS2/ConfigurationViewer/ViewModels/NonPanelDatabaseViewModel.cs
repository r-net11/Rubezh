using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using ServerFS2.ConfigurationWriter;

namespace ConfigurationViewer.ViewModels
{
	public class NonPanelDatabaseViewModel : BaseViewModel
	{
		public SystemDatabaseCreator ConfigurationWriterHelper { get; set; }

		NonPanelDatabase NonPanelDatabase;
		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase { get; set; }
		public List<ByteDescription> RootBytes { get; set; }
		public List<NonPanelUnequalByteViewModel> UnequalBytes { get; set; }

		public NonPanelDatabaseViewModel(NonPanelDatabase nonPanelDatabase)
		{
			NonPanelDatabase = nonPanelDatabase;
			ParentPanel = nonPanelDatabase.ParentPanel;
			BytesDatabase = nonPanelDatabase.BytesDatabase;
			RootBytes = nonPanelDatabase.RootBytes;
			UnequalBytes = new List<NonPanelUnequalByteViewModel>();
			MergeDatabase();
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

		void MergeDatabase()
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

					if (ParentPanel.Driver.DriverType == DriverType.IndicationBlock)
					{
						var crcBytes = bytes.ToList();
						crcBytes.RemoveRange(0, 10);

						for (int i = 0; i < 9; i++)
						{
							Trace.WriteLine("CRC bytes " + i.ToString() + " - " + crcBytes[i]);
						}
						var crc16Value = Crc16Helper.ComputeChecksum(crcBytes);
						Trace.WriteLine("CRC " + crc16Value / 256);
						Trace.WriteLine("CRC " + crc16Value % 256);
					}

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
							else if (!byteDescription.IsReadOnly && !byteDescription.IgnoreUnequal)
							{
								var unequalByteViewModel = new NonPanelUnequalByteViewModel(this, byteDescription);
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
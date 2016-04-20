using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Helpers;

namespace ConfigurationViewer.ViewModels
{
	public class PanelDatabaseViewModel : BaseViewModel
	{
		public RomDatabase RomDatabase { get; set; }
		public FlashDatabase FlashPanelDatabase { get; set; }
		public Device ParentPanel { get; set; }
		public List<PanelUnequalByteViewModel> RomUnequalBytes { get; set; }
		public List<PanelUnequalByteViewModel> FlashUnequalBytes { get; set; }
		PanelDatabase PanelDatabase;

		public PanelDatabaseViewModel(PanelDatabase panelDatabase)
		{
			PanelDatabase = panelDatabase;
			ParentPanel = panelDatabase.ParentPanel;
			RomDatabase = panelDatabase.RomDatabase;
			FlashPanelDatabase = panelDatabase.FlashDatabase;

			RomUnequalBytes = new List<PanelUnequalByteViewModel>();
			FlashUnequalBytes = new List<PanelUnequalByteViewModel>();
			MergeRomDatabase();
			MergeFlashDatabase();
		}

		ByteDescription _romSelectedByteDescription;
		public ByteDescription RomSelectedByteDescription
		{
			get { return _romSelectedByteDescription; }
			set
			{
				_romSelectedByteDescription = value;
				OnPropertyChanged("RomSelectedByteDescription");
			}
		}

		ByteDescription _flashSelectedByteDescription;
		public ByteDescription FlashSelectedByteDescription
		{
			get { return _flashSelectedByteDescription; }
			set
			{
				_flashSelectedByteDescription = value;
				OnPropertyChanged("FlashSelectedByteDescription");
			}
		}

		public bool HasUnequalBytes
		{
			get { return (RomUnequalBytes.Count + FlashUnequalBytes.Count) > 0; }
		}

		void MergeRomDatabase()
		{
			foreach (var byteDescription in RomDatabase.BytesDatabase.ByteDescriptions)
			{
				byteDescription.IsNotEqualToOriginal = true;
			}

			var bytes = FileDBHelper.GetRomDBFromFS1File(ParentPanel);
			//if (bytes.Count > 0x2000)
			//    bytes.RemoveRange(0, 0x2000);
			//var emptyBytes = new List<byte>();
			////for (int i = 0; i < 12288; i++)
			//for (int i = 0; i < 0x2000; i++)
			//{
			//    emptyBytes.Add(0);
			//}
			//bytes.InsertRange(0, emptyBytes);

			foreach (var byteDescription in RomDatabase.BytesDatabase.ByteDescriptions)
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
						var unequalByteViewModel = new PanelUnequalByteViewModel(this, true, byteDescription);
						RomUnequalBytes.Add(unequalByteViewModel);
					}
				}
			}
		}
		void MergeFlashDatabase()
		{
			foreach (var byteDescription in FlashPanelDatabase.BytesDatabase.ByteDescriptions)
			{
				byteDescription.IsNotEqualToOriginal = true;
			}

			var bytes = FileDBHelper.GetFlashDBFromFS1File(ParentPanel);

			foreach (var byteDescription in FlashPanelDatabase.BytesDatabase.ByteDescriptions)
			{
				if (bytes.Count > byteDescription.Offset)
				{
					var originalByte = bytes[byteDescription.Offset];
					byteDescription.OriginalValue = originalByte;
					if (byteDescription.Value == originalByte)
					{
						byteDescription.IsNotEqualToOriginal = false;
					}
				}
			}

			foreach (var byteDescription in FlashPanelDatabase.BytesDatabase.ByteDescriptions)
			{
				if (bytes.Count > byteDescription.Offset)
				{
					var originalByte = bytes[byteDescription.Offset];
					if (byteDescription.Value == originalByte)
					{
						byteDescription.IsNotEqualToOriginal = false;
					}
				}

				if (byteDescription.TableBaseReference != null)
				{
					int index = FlashPanelDatabase.BytesDatabase.ByteDescriptions.IndexOf(byteDescription);
					var originalReference = byteDescription.OriginalValue * 256 * 256 +
						FlashPanelDatabase.BytesDatabase.ByteDescriptions[index + 1].OriginalValue * 256 +
						FlashPanelDatabase.BytesDatabase.ByteDescriptions[index + 2].OriginalValue;
					var reference = FlashPanelDatabase.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.Offset == originalReference);
					if (reference != null)
					{
						var realReference = FlashPanelDatabase.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.Offset.ToString() == byteDescription.RealValue);
						EffectorDeviceTable effectorDeviceTable1 = realReference.TableHeader as EffectorDeviceTable;
						EffectorDeviceTable effectorDeviceTable2 = reference.TableHeader as EffectorDeviceTable;
						if (effectorDeviceTable1 != null && effectorDeviceTable2 != null)
						{
							if (effectorDeviceTable1.Device.PresentationAddress == effectorDeviceTable2.Device.PresentationAddress)
							{
								FlashPanelDatabase.BytesDatabase.ByteDescriptions[index + 0].IsNotEqualToOriginal = false;
								FlashPanelDatabase.BytesDatabase.ByteDescriptions[index + 1].IsNotEqualToOriginal = false;
								FlashPanelDatabase.BytesDatabase.ByteDescriptions[index + 2].IsNotEqualToOriginal = false;

								var refeenceName = originalReference.ToString() + " - " + effectorDeviceTable2.Device.DottedPresentationAddressAndName;
								byteDescription.OriginalReference = refeenceName;
							}
						}
					}
				}

				if (byteDescription.IsNotEqualToOriginal && !byteDescription.IgnoreUnequal)
				{
					var unequalByteViewModel = new PanelUnequalByteViewModel(this, false, byteDescription);
					FlashUnequalBytes.Add(unequalByteViewModel);
				}
			}
		}
	}
}
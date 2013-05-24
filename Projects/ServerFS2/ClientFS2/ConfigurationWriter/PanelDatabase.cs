using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using System.IO;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase : BaseViewModel
	{
		public PanelDatabase1 PanelDatabase1 { get; set; }
		public PanelDatabase2 PanelDatabase2 { get; set; }
		public Device ParentPanel { get; set; }
		public List<UnequalByteViewModel> UnequalBytes1 { get; set; }
		public List<UnequalByteViewModel> UnequalBytes2 { get; set; }

		static double Total_Miliseconds;

		public PanelDatabase(Device parentDevice)
		{
			var startDateTime = DateTime.Now;

			ParentPanel = parentDevice;
			PanelDatabase2 = new PanelDatabase2(parentDevice);
			//PanelDatabase1 = new PanelDatabase1(PanelDatabase2, 12288);
			PanelDatabase1 = new PanelDatabase1(PanelDatabase2, 0x3000);
			Trace.WriteLine("PanelDatabase Done");

			var deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Total_Miliseconds += deltaMiliseconds;
			Trace.WriteLine("PanelDatabase Total_Miliseconds=" + Total_Miliseconds.ToString());

			UnequalBytes1 = new List<UnequalByteViewModel>();
			UnequalBytes2 = new List<UnequalByteViewModel>();
			MergeDatabase1();
			MergeDatabase2();
		}

		ByteDescription _selectedByteDescription1;
		public ByteDescription SelectedByteDescription1
		{
			get { return _selectedByteDescription1; }
			set
			{
				_selectedByteDescription1 = value;
				OnPropertyChanged("SelectedByteDescription1");
			}
		}

		ByteDescription _selectedByteDescription2;
		public ByteDescription SelectedByteDescription2
		{
			get { return _selectedByteDescription2; }
			set
			{
				_selectedByteDescription2 = value;
				OnPropertyChanged("SelectedByteDescription2");
			}
		}

		void MergeDatabase1()
		{
			foreach (var byteDescription in PanelDatabase1.BytesDatabase.ByteDescriptions)
			{
				byteDescription.IsNotEqualToOriginal = true;
			}

			var fileName = @"C:\Program Files\Firesec\TstData\" + ParentPanel.Driver.ShortName + " - " + ParentPanel.DottedAddress + "_rom.bin";
			if (File.Exists(fileName))
			{
				var byteArray = File.ReadAllBytes(fileName);
				if (byteArray != null)
				{
					var bytes = byteArray.ToList();
					//bytes.RemoveRange(0, 0x2000);
					//var emptyBytes = new List<byte>();
					////for (int i = 0; i < 12288; i++)
					//for (int i = 0; i < 0x2000; i++)
					//{
					//    emptyBytes.Add(0);
					//}
					//bytes.InsertRange(0, emptyBytes);

					foreach (var byteDescription in PanelDatabase1.BytesDatabase.ByteDescriptions)
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
								var unequalByteViewModel = new UnequalByteViewModel(this, true, byteDescription);
								UnequalBytes1.Add(unequalByteViewModel);
							}
						}
					}
				}
			}
		}
		void MergeDatabase2()
		{
			foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
			{
				byteDescription.IsNotEqualToOriginal = true;
			}

			var fileName = @"C:\Program Files\Firesec\TstData\" + ParentPanel.Driver.ShortName + " - " + ParentPanel.DottedAddress + "_flash.bin";
			if (File.Exists(fileName))
			{
				var byteArray = File.ReadAllBytes(fileName);
				if (byteArray != null)
				{
					var bytes = byteArray.ToList();

					foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
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

					foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
					{
						if (bytes.Count > byteDescription.Offset)
						{
							var originalByte = bytes[byteDescription.Offset];
							//byteDescription.OriginalValue = originalByte;
							if (byteDescription.Value == originalByte)
							{
								byteDescription.IsNotEqualToOriginal = false;
							}
						}

						if (byteDescription.TableBaseReference != null)
						{
							int index = PanelDatabase2.BytesDatabase.ByteDescriptions.IndexOf(byteDescription);
							var originalReference = byteDescription.OriginalValue * 256 * 256 +
								PanelDatabase2.BytesDatabase.ByteDescriptions[index + 1].OriginalValue * 256 +
								PanelDatabase2.BytesDatabase.ByteDescriptions[index + 2].OriginalValue;
							var reference = PanelDatabase2.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.Offset == originalReference);
							if (reference != null)
							{
								var realReference = PanelDatabase2.BytesDatabase.ByteDescriptions.FirstOrDefault(x => x.Offset.ToString() == byteDescription.RealValue);
								EffectorDeviceTable effectorDeviceTable1 = realReference.TableHeader as EffectorDeviceTable;
								EffectorDeviceTable effectorDeviceTable2 = reference.TableHeader as EffectorDeviceTable;
								if (effectorDeviceTable1 != null && effectorDeviceTable2 != null)
								{
									if (effectorDeviceTable1.Device.PresentationAddress == effectorDeviceTable2.Device.PresentationAddress)
									{
										PanelDatabase2.BytesDatabase.ByteDescriptions[index + 0].IsNotEqualToOriginal = false;
										PanelDatabase2.BytesDatabase.ByteDescriptions[index + 1].IsNotEqualToOriginal = false;
										PanelDatabase2.BytesDatabase.ByteDescriptions[index + 2].IsNotEqualToOriginal = false;

										var refeenceName = originalReference.ToString() + " - " + effectorDeviceTable2.Device.DottedPresentationAddressAndName;
										byteDescription.OriginalReference = refeenceName;
									}
								}
							}
						}

						if (byteDescription.IsNotEqualToOriginal && !byteDescription.IgnoreUnequal)
						{
							var unequalByteViewModel = new UnequalByteViewModel(this, false, byteDescription);
							UnequalBytes2.Add(unequalByteViewModel);
						}
					}
				}
			}
		}
	}
}
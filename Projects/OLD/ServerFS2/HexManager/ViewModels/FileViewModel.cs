using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FS2Api;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ServerFS2;

namespace HexManager.ViewModels
{
	public class FileViewModel : BaseViewModel
	{
		public static FileViewModel FromFile(string fileName)
		{
			var hexFileInfo = new HEXFileInfo()
			{
				HexMemoryType = HexMemoryType.Controller_AVR,
				FileName = new FileInfo(fileName).Name,
				Lines = File.ReadAllLines(fileName).ToList()
			};
			var fileViewModel = new FileViewModel(hexFileInfo, true);
			return fileViewModel;
			//var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(fileName);
		}

		public FileViewModel(HEXFileInfo hexFileInfo, bool isNew)
		{
			HexMemoryTypes = Enum.GetValues(typeof(HexMemoryType)).Cast<HexMemoryType>().ToList();

			Error = "";
			FileName = hexFileInfo.FileName;
			SelectedHexMemoryType = hexFileInfo.HexMemoryType;
			Lines = new List<LineViewModel>();
			foreach (var line in hexFileInfo.Lines)
			{
				var lineViewModel = new LineViewModel(line);
				Lines.Add(lineViewModel);
			}

			var preLastLine = hexFileInfo.Lines[hexFileInfo.Lines.Count - 3];
			CRC = preLastLine.Substring(39, 2) + preLastLine.Substring(37, 2);
			var minorVersion = Convert.ToInt32(preLastLine.Substring(35, 2), 16);
			var majorVersion = Convert.ToInt32(preLastLine.Substring(33, 2), 16);
			Version = minorVersion.ToString() + "." + majorVersion.ToString();

			var firstOffset = Convert.ToInt32(Lines[1].StringOffset, 16);
			var lastOffset = firstOffset;
			var baseOffset = 0;
			for (int i = 1; i < Lines.Count - 2; i++)
			{
				var currentLine = Lines[i];
				if (currentLine.StringLineType == "00")
				{
					lastOffset = Convert.ToInt32(Lines[i].StringOffset, 16);
				}
				if (currentLine.StringLineType == "04")
				{
					baseOffset = Convert.ToInt32(Lines[i].Content, 16);
				}
			}
			lastOffset = baseOffset * 0x10000 + lastOffset + 0x0F;
			AddressRange = firstOffset.ToString("X8") + " - " + lastOffset.ToString("X8");

			if (isNew)
			{
				var memoryType = preLastLine.Substring(31, 2);
				switch (memoryType)
				{
					case "95":
						SelectedHexMemoryType = HexMemoryType.Controller_AVR;
						break;
					case "BB":
						SelectedHexMemoryType = HexMemoryType.RAM_RS485;
						break;
					case "CC":
						SelectedHexMemoryType = HexMemoryType.RAM_USB;
						break;
					case "AA":
						SelectedHexMemoryType = HexMemoryType.ROM;
						break;
					case "FF":
						SelectedHexMemoryType = HexMemoryType.User_ARM;
						break;
					default:
						Error += "Неизвестный тип памяти" + "\n";
						break;
				}
			}
		}

		public string FileName { get; set; }
		public List<HexMemoryType> HexMemoryTypes { get; set; }
		public List<LineViewModel> Lines { get; set; }

		HexMemoryType _selectedHexMemoryType;
		public HexMemoryType SelectedHexMemoryType
		{
			get { return _selectedHexMemoryType; }
			set
			{
				_selectedHexMemoryType = value;
				OnPropertyChanged("SelectedHexMemoryType");
			}
		}

		public string Version { get; set; }
		public string AddressRange { get; set; }
		public string CRC { get; set; }
		public string Error { get; set; }
	}
}
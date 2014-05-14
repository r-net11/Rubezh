using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace HexManager.ViewModels
{
	public class HexFileViewModel : BaseViewModel
	{
		public static HexFileViewModel FromFile(string fileName, XDriverType driverType)
		{
			return new HexFileViewModel(
			new HEXFileInfo()
			{
				FileName = new FileInfo(fileName).Name,
				DriverType = driverType,
				Lines = File.ReadAllLines(fileName).ToList()
			});
		}

		public HexFileViewModel(HEXFileInfo hexFileInfo)
		{
			HexFileInfo = hexFileInfo;
			FileName = hexFileInfo.FileName;
			DriverType = hexFileInfo.DriverType;
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
		}
		public HEXFileInfo HexFileInfo { get; set; }
		public string FileName { get; set; }
		public List<LineViewModel> Lines { get; set; }
		public XDriverType DriverType { get; set; }
		public string Version { get; set; }
		public string AddressRange { get; set; }
		public string CRC { get; set; }
	}
}
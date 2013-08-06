using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HexManager.Models;
using Infrastructure.Common.Windows.ViewModels;

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
			var fileViewModel = new FileViewModel(hexFileInfo);
			return fileViewModel;
			//var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(fileName);
		}

		public FileViewModel(HEXFileInfo hexFileInfo)
		{
			HexMemoryTypes = Enum.GetValues(typeof(HexMemoryType)).Cast<HexMemoryType>().ToList();

			FileName = hexFileInfo.FileName;
			SelectedHexMemoryType = hexFileInfo.HexMemoryType;
			Lines = new List<LineViewModel>();
			foreach (var line in hexFileInfo.Lines)
			{
				var lineViewModel = new LineViewModel(line);
				Lines.Add(lineViewModel);
			}

			Version = "Неизвестно";
			AddressRange = "Неизвестно";
			CRC = "Неизвестно";
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
	}
}
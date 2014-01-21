using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace HexManager.ViewModels
{
	public class HexFileInfoViewModel:BaseViewModel
	{
		public HexFileInfoViewModel(HEXFileInfo hexFileInfo)
		{
			DriverType = hexFileInfo.DriverType;
			FileName = hexFileInfo.FileName;
			Lines = hexFileInfo.Lines;
		}

		public static List<HexFileInfoViewModel> Initialize(List<HEXFileInfo> hexFileInfos)
		{
			return hexFileInfos.Select(hexFileInfo => new HexFileInfoViewModel(hexFileInfo)).ToList();
		}

		XDriverType driverType;
		public XDriverType DriverType 
		{ 
			get { return driverType; }
			set
			{
				driverType = value;
				OnPropertyChanged("DriverType");
			}
		}

		string fileName;
		public string FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				OnPropertyChanged("FileName");
			}
		}

		List<string> lines;
		public List<string> Lines
		{
			get { return lines; }
			set
			{
				lines = value;
				OnPropertyChanged("Lines");
			}
		}
	}
}

using System.IO;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using XFiresecAPI;

namespace HexManager.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			CreateNewCommand = new RelayCommand(OnCreateNew);
			AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanRemoveFile);
			SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
			LoadFileCommand = new RelayCommand(OnLoadFile);
			Files = new ObservableRangeCollection<FileViewModel>();
			Initialize(new HexFileCollectionInfo());
		}

		void Initialize(HexFileCollectionInfo hexFileCollectionInfo)
		{
			Name = hexFileCollectionInfo.Name;
			MinorVersion = hexFileCollectionInfo.MinorVersion;
			MajorVersion = hexFileCollectionInfo.MajorVersion;
			var hexFileInfos = new List<HEXFileInfo>()
			{
				new HEXFileInfo {DriverType = XDriverType.GK},
				new HEXFileInfo {DriverType = XDriverType.KAU},
				new HEXFileInfo {DriverType = XDriverType.RSR2_KAU}
			};
			AvailableHexFileInfos = new ObservableCollection<HEXFileInfo>(hexFileInfos);
			SelectedHexFileInfo = AvailableHexFileInfos.FirstOrDefault();
			HexFileInfos = new List<HEXFileInfo>();

			Files = new ObservableRangeCollection<FileViewModel>();
			foreach (var hexFileInfo in hexFileCollectionInfo.HexFileInfos)
			{
				HexFileInfos.Add(hexFileInfo);
				AvailableHexFileInfos.Remove(AvailableHexFileInfos.FirstOrDefault(x => x.DriverType == hexFileInfo.DriverType));
				Files.Add(new FileViewModel(hexFileInfo, hexFileInfo.DriverType, false));
			}
			SelectedFile = Files.FirstOrDefault();
			SelectedHexFileInfo = AvailableHexFileInfos.FirstOrDefault();
		}

		List<HEXFileInfo> HexFileInfos { get; set; }

		ObservableCollection<HEXFileInfo> _availableHexFileInfos;
		public ObservableCollection<HEXFileInfo> AvailableHexFileInfos
		{
			get { return _availableHexFileInfos; }
			set
			{
				_availableHexFileInfos = value;
				OnPropertyChanged("AvailableHexFileInfos");
			}
		}

		HEXFileInfo selectedHexFileInfo;
		public HEXFileInfo SelectedHexFileInfo
		{
			get { return selectedHexFileInfo; }
			set
			{
				selectedHexFileInfo = value;
				OnPropertyChanged("SelectedHexFileInfo");
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		int _minorVersion;
		public int MinorVersion
		{
			get { return _minorVersion; }
			set
			{
				_minorVersion = value;
				OnPropertyChanged("MinorVersion");
			}
		}

		int _majorVersion;
		public int MajorVersion
		{
			get { return _majorVersion; }
			set
			{
				_majorVersion = value;
				OnPropertyChanged("MajorVersion");
			}
		}

		ObservableCollection<FileViewModel> _files;
		public ObservableCollection<FileViewModel> Files
		{
			get { return _files; }
			set
			{
				_files = value;
				OnPropertyChanged("Files");
			}
		}

		FileViewModel _selectedFile;
		public FileViewModel SelectedFile
		{
			get { return _selectedFile; }
			set
			{
				_selectedFile = value;
				OnPropertyChanged("SelectedFile");
			}
		}

		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
			var hexFileCollectionInfo = new HexFileCollectionInfo();
			Initialize(hexFileCollectionInfo);
		}

		public RelayCommand AddFileCommand { get; private set; }
		void OnAddFile()
		{
			var openFileDialog = new OpenFileDialog()
			{
                Filter = "Пакет обновления (*.HCS)|*.HCS"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var fileName = openFileDialog.FileName;
				var fileViewModel = FileViewModel.FromFile(fileName, SelectedHexFileInfo.DriverType);
				SelectedHexFileInfo.FileName = fileViewModel.FileName;
				SelectedHexFileInfo.Lines = File.ReadAllLines(fileName).ToList();
				Files.Add(fileViewModel);
				Files = new ObservableCollection<FileViewModel>(Files.OrderBy(x => x.DriverType));
				SelectedFile = fileViewModel;
				HexFileInfos.Add(SelectedHexFileInfo);
				AvailableHexFileInfos.Remove(SelectedHexFileInfo);
				SelectedHexFileInfo = AvailableHexFileInfos.FirstOrDefault();
			}
		}
		bool CanAddFile()
		{
			return SelectedHexFileInfo != null;
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			var hexFileInfo = HexFileInfos.FirstOrDefault(x => x.DriverType == SelectedFile.DriverType);
			SelectedHexFileInfo = hexFileInfo;
			Files.Remove(SelectedFile);
			AvailableHexFileInfos.Add(hexFileInfo);
			AvailableHexFileInfos = new ObservableCollection<HEXFileInfo>(AvailableHexFileInfos.OrderBy(x => x.DriverType));
			HexFileInfos.Remove(hexFileInfo);
			SelectedFile = Files.FirstOrDefault();
		}
		bool CanRemoveFile()
		{
			return SelectedFile != null;
		}

		public RelayCommand SaveFileCommand { get; private set; }
		void OnSaveFile()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Пакет обновления (*.FSCS)|*.FSCS",
				DefaultExt = "Пакет обновления (*.FSCS)|*.FSCS"
			};
			if (saveDialog.ShowDialog().Value)
			{
				if (File.Exists(saveDialog.FileName))
					File.Delete(saveDialog.FileName);

				var hexFileCollectionInfo = new HexFileCollectionInfo()
				{
					Name = Name,
					MinorVersion = MinorVersion,
					MajorVersion = MajorVersion
				};
				foreach (var hexFileInfo in HexFileInfos)
				{
					hexFileCollectionInfo.HexFileInfos.Add(hexFileInfo);
				}
				HXCFileInfoHelper.Save(saveDialog.FileName, hexFileCollectionInfo);
			}
		}
		bool CanSaveFile()
		{
			return Files.Count > 0;
		}

		public RelayCommand LoadFileCommand { get; private set; }
		void OnLoadFile()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Пакет обновления (*.FSCS)|*.FSCS"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var hxcFileInfo = HXCFileInfoHelper.Load(openFileDialog.FileName);
				if (hxcFileInfo != null)
				{
					Initialize(hxcFileInfo);
				}
			}
		}
	}
}
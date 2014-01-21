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

		ObservableCollection<HexFileInfoViewModel> availableHexFileInfoViewModels;
		public ObservableCollection<HexFileInfoViewModel> AvailableHexFileInfoViewModels
		{
			get
			{
				return availableHexFileInfoViewModels;
			}
			set
			{
				availableHexFileInfoViewModels = value;
				OnPropertyChanged("AvailableHexFileInfoViewModels");
			}
		}
		List<HexFileInfoViewModel> HexFileInfoViewModels { get; set; }
		void Initialize (HexFileCollectionInfo hexFileCollectionInfo)
		{
			Name = hexFileCollectionInfo.Name;
			MinorVersion = hexFileCollectionInfo.MinorVersion;
			MajorVersion = hexFileCollectionInfo.MajorVersion;
			var hexFileInfos = new List<HEXFileInfo>()
			{
				new HEXFileInfo {DriverType = XDriverType.GK, Lines = new List<string>()},
				new HEXFileInfo {DriverType = XDriverType.KAU, Lines = new List<string>()},
				new HEXFileInfo {DriverType = XDriverType.RSR2_KAU, Lines = new List<string>()}
			};
			AvailableHexFileInfoViewModels = new ObservableCollection<HexFileInfoViewModel>(HexFileInfoViewModel.Initialize(hexFileInfos));
			SelectedHexFileInfo = AvailableHexFileInfoViewModels.FirstOrDefault();
			HexFileInfoViewModels = new List<HexFileInfoViewModel>();

			Files = new ObservableRangeCollection<FileViewModel>();
			foreach (var hexFileInfo in hexFileCollectionInfo.HexFileInfos)
			{
				HexFileInfoViewModels.Add(new HexFileInfoViewModel(hexFileInfo));
				AvailableHexFileInfoViewModels.Remove(AvailableHexFileInfoViewModels.FirstOrDefault(x => x.DriverType == hexFileInfo.DriverType));
				Files.Add(new FileViewModel(hexFileInfo, hexFileInfo.DriverType, false));
			}
			SelectedFile = Files.FirstOrDefault();
			SelectedHexFileInfo = AvailableHexFileInfoViewModels.FirstOrDefault();
		}

		HexFileInfoViewModel selectedHexFileInfo;
		public HexFileInfoViewModel SelectedHexFileInfo
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
				HexFileInfoViewModels.Add(SelectedHexFileInfo);
				AvailableHexFileInfoViewModels.Remove(SelectedHexFileInfo);
				SelectedHexFileInfo = AvailableHexFileInfoViewModels.FirstOrDefault();
			}
		}
		bool CanAddFile()
		{
			return SelectedHexFileInfo != null;
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			var hexFileInfo = HexFileInfoViewModels.FirstOrDefault(x => x.DriverType == SelectedFile.DriverType);
			SelectedHexFileInfo = hexFileInfo;
			Files.Remove(SelectedFile);
			AvailableHexFileInfoViewModels.Add(hexFileInfo);
			AvailableHexFileInfoViewModels = new ObservableCollection<HexFileInfoViewModel>(AvailableHexFileInfoViewModels.OrderBy(x => x.DriverType));
			HexFileInfoViewModels.Remove(hexFileInfo);
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
				foreach (var hexFileInfoViewModel in HexFileInfoViewModels)
				{
					var hexFileInfo = new HEXFileInfo();
					hexFileInfo.DriverType = hexFileInfoViewModel.DriverType;
					hexFileInfo.FileName = hexFileInfoViewModel.FileName;
					hexFileInfo.Lines = hexFileInfoViewModel.Lines;
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
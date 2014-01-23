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
			AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanRemoveFile);
			CreateNewCommand = new RelayCommand(OnCreateNew);
			SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
			LoadFileCommand = new RelayCommand(OnLoadFile);
			Initialize(new HexFileCollectionInfo());
		}

		void Initialize(HexFileCollectionInfo hexFileCollectionInfo)
		{
			Name = hexFileCollectionInfo.Name;
			MinorVersion = hexFileCollectionInfo.MinorVersion;
			MajorVersion = hexFileCollectionInfo.MajorVersion;
			AvailableDriverTypes = new ObservableCollection<XDriverType>()
		    { 
				XDriverType.GK, XDriverType.KAU, XDriverType.RSR2_KAU
			};
			SelectedDriverType = AvailableDriverTypes.FirstOrDefault();
			HexFileViewModels = new ObservableRangeCollection<HexFileViewModel>();
			foreach (var hexFileInfo in hexFileCollectionInfo.HexFileInfos)
			{
				AvailableDriverTypes.Remove(AvailableDriverTypes.FirstOrDefault(x => x == hexFileInfo.DriverType));
				HexFileViewModels.Add(new HexFileViewModel(hexFileInfo, hexFileInfo.DriverType, false));
			}
			SelectedHexFile = HexFileViewModels.FirstOrDefault();
			SelectedDriverType = AvailableDriverTypes.FirstOrDefault();
		}

		ObservableCollection<XDriverType> _availableDriverTypes;
		public ObservableCollection<XDriverType> AvailableDriverTypes
		{
			get { return _availableDriverTypes; }
			set
			{
				_availableDriverTypes = value;
				OnPropertyChanged("AvailableDriverTypes");
			}
		}

		XDriverType _selectedDriverType;
		public XDriverType SelectedDriverType
		{
			get { return _selectedDriverType; }
			set
			{
				_selectedDriverType = value;
				OnPropertyChanged("SelectedDriverType");
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

		ObservableCollection<HexFileViewModel> _hexFilesViewModel;
		public ObservableCollection<HexFileViewModel> HexFileViewModels
		{
			get { return _hexFilesViewModel; }
			set
			{
				_hexFilesViewModel = value;
				OnPropertyChanged("HexFileViewModels");
			}
		}

		HexFileViewModel _selectedHexFile;
		public HexFileViewModel SelectedHexFile
		{
			get { return _selectedHexFile; }
			set
			{
				_selectedHexFile = value;
				OnPropertyChanged("SelectedHexFile");
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
				SelectedHexFile = HexFileViewModel.FromFile(openFileDialog.FileName, SelectedDriverType);
				HexFileViewModels.Add(SelectedHexFile);
				HexFileViewModels = new ObservableCollection<HexFileViewModel>(HexFileViewModels.OrderBy(x => x.DriverType));
				AvailableDriverTypes.Remove(SelectedDriverType);
				SelectedDriverType = AvailableDriverTypes.FirstOrDefault();
			}
		}
		bool CanAddFile()
		{
			return SelectedDriverType != null;
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			SelectedDriverType = SelectedHexFile.DriverType;
			AvailableDriverTypes.Add(SelectedDriverType);
			AvailableDriverTypes = new ObservableCollection<XDriverType>(AvailableDriverTypes.OrderBy(x => x.ToString()));
			HexFileViewModels.Remove(SelectedHexFile);
			SelectedHexFile = HexFileViewModels.FirstOrDefault();
		}
		bool CanRemoveFile()
		{
			return SelectedHexFile != null;
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
				foreach (var file in HexFileViewModels)
				{
					var hexFileInfo = new HEXFileInfo();
					hexFileInfo.DriverType = file.DriverType;
					hexFileInfo.FileName = file.FileName;
					hexFileInfo.Lines = file.OriginalLines;
					hexFileCollectionInfo.HexFileInfos.Add(hexFileInfo);
				}
				HXCFileInfoHelper.Save(saveDialog.FileName, hexFileCollectionInfo);
			}
		}
		bool CanSaveFile()
		{
			return HexFileViewModels.Count > 0;
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
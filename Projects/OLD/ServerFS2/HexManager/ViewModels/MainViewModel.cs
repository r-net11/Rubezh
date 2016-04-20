using System.IO;
using System.Linq;
using FS2Api;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace HexManager.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			CreateNewCommand = new RelayCommand(OnCreateNew);
			AddFileCommand = new RelayCommand(OnAddFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanRemoveFile);
			SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
			LoadFileCommand = new RelayCommand(OnLoadFile);
			Files = new ObservableRangeCollection<FileViewModel>();

			Drivers = new ObservableCollection<DriverType>();
			Drivers.Add(DriverType.Rubezh_2AM);
			Drivers.Add(DriverType.Rubezh_4A);
			Drivers.Add(DriverType.Rubezh_2OP);
			Drivers.Add(DriverType.BUNS);
			Drivers.Add(DriverType.IndicationBlock);
			Drivers.Add(DriverType.PDU);
			Drivers.Add(DriverType.PDU_PT);
			Drivers.Add(DriverType.MS_1);
			Drivers.Add(DriverType.MS_2);
			Drivers.Add(DriverType.MS_3);
			Drivers.Add(DriverType.MS_4);
			Drivers.Add(DriverType.UOO_TL);

			OnCreateNew();
		}

		void CopyProperties(HexFileCollectionInfo hexFileCollectionInfo)
		{
			SelectedDriver = hexFileCollectionInfo.DriverType;
			Name = hexFileCollectionInfo.Name;
			MinorVersion = hexFileCollectionInfo.MinorVersion;
			MajorVersion = hexFileCollectionInfo.MajorVersion;
			AutorName = hexFileCollectionInfo.AutorName;

			Files = new ObservableRangeCollection<FileViewModel>();
			foreach (var fileInfo in hexFileCollectionInfo.FileInfos)
			{
				var fileViewModel = new FileViewModel(fileInfo, false);
				Files.Add(fileViewModel);
				SelectedFile = Files.FirstOrDefault();
			}
		}

		ObservableCollection<DriverType> _drivers;
		public ObservableCollection<DriverType> Drivers
		{
			get { return _drivers; }
			set
			{
				_drivers = value;
				OnPropertyChanged("Drives");
			}
		}

		DriverType _selectedDriver;
		public DriverType SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged("SelectedDriver");
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

		string _autorName;
		public string AutorName
		{
			get { return _autorName; }
			set
			{
				_autorName = value;
				OnPropertyChanged("AutorName");
			}
		}

		ObservableRangeCollection<FileViewModel> _files;
		public ObservableRangeCollection<FileViewModel> Files
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
			CopyProperties(hexFileCollectionInfo);
		}

		public RelayCommand AddFileCommand { get; private set; }
		void OnAddFile()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Пакет обновления (*.HEX)|*.HEX"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var fileName = openFileDialog.FileName;
				var fileViewModel = FileViewModel.FromFile(fileName);
				Files.Add(fileViewModel);
				SelectedFile = fileViewModel;
			}
		}

		public RelayCommand RemoveFileCommand { get; private set; }
		void OnRemoveFile()
		{
			Files.Remove(SelectedFile);
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
					DriverType = SelectedDriver,
					Name = Name,
					MinorVersion = MinorVersion,
					MajorVersion = MajorVersion,
					AutorName = AutorName
				};
				foreach (var file in Files)
				{
					var hexFileInfo = new HEXFileInfo()
					{
						HexMemoryType = file.SelectedHexMemoryType,
						FileName = file.FileName
					};
					foreach (var lineViewModel in file.Lines)
					{
						hexFileInfo.Lines.Add(lineViewModel.OriginalContent);
					}
					hexFileCollectionInfo.FileInfos.Add(hexFileInfo);
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
					CopyProperties(hxcFileInfo);
				}
			}
		}
	}
}
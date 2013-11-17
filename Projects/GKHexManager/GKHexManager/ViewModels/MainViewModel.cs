using System.IO;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using System.Collections.Generic;
using XFiresecAPI;

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

			Drivers = new ObservableCollection<XDriverType>();
			Drivers.Add(XDriverType.GK);
			Drivers.Add(XDriverType.KAU);
			Drivers.Add(XDriverType.RSR2_KAU);

			OnCreateNew();
		}

		void CopyProperties(HexFileCollectionInfo hexFileCollectionInfo)
		{
			SelectedDriver = hexFileCollectionInfo.DriverType;
			Name = hexFileCollectionInfo.Name;
			MinorVersion = hexFileCollectionInfo.MinorVersion;
			MajorVersion = hexFileCollectionInfo.MajorVersion;

			Files = new ObservableRangeCollection<FileViewModel>();
			foreach (var fileInfo in hexFileCollectionInfo.FileInfos)
			{
				var fileViewModel = new FileViewModel(fileInfo, false);
				Files.Add(fileViewModel);
				SelectedFile = Files.FirstOrDefault();
			}
		}

		ObservableCollection<XDriverType> _drivers;
		public ObservableCollection<XDriverType> Drivers
		{
			get { return _drivers; }
			set
			{
				_drivers = value;
				OnPropertyChanged("Drives");
			}
		}

		XDriverType _selectedDriver;
		public XDriverType SelectedDriver
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
                Filter = "Пакет обновления (*.HCS)|*.HCS"
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
					MajorVersion = MajorVersion
				};
				foreach (var file in Files)
				{
					var hexFileInfo = new HEXFileInfo()
					{
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
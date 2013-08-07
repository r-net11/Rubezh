using System.IO;
using System.Linq;
using HexManager.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace HexManager.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			CreateNewCommand = new RelayCommand(OnCreateNew, CanCreateNew);
			AddFileCommand = new RelayCommand(OnAddFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanRemoveFile);
			SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
			LoadFileCommand = new RelayCommand(OnLoadFile);
			Files = new ObservableRangeCollection<FileViewModel>();
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
			Files = new ObservableRangeCollection<FileViewModel>();
		}
		bool CanCreateNew()
		{
			return Files.Count > 0;
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

				var hxcFileInfo = new HXCFileInfo();
				foreach (var file in Files)
				{
					var hexFileInfo = new HEXFileInfo()
					{
						HexMemoryType = file.SelectedHexMemoryType,
						FileName = file.FileName
					};
					foreach (var lineViewModel in file.Lines)
					{
						hexFileInfo.Lines.Add(lineViewModel.Content);
					}
					hxcFileInfo.FileInfos.Add(hexFileInfo);
				}
				HXCFileInfoHelper.Save(saveDialog.FileName, hxcFileInfo);
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
					Files = new ObservableRangeCollection<FileViewModel>();
					foreach (var fileInfo in hxcFileInfo.FileInfos)
					{
						var fileViewModel = new FileViewModel(fileInfo);
						Files.Add(fileViewModel);
						SelectedFile = Files.FirstOrDefault();
					}
				}
			}
		}
	}
}
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using Infrastructure.Common.Windows;

namespace PowerCalculator.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			CreateNewCommand = new RelayCommand(OnCreateNew);
			SaveFileCommand = new RelayCommand(OnSaveFile, CanSaveFile);
			LoadFileCommand = new RelayCommand(OnLoadFile);
			AddLineCommand = new RelayCommand(OnAddLine, CanAddLine);
			RemoveLineCommand = new RelayCommand(OnRemoveFile, CanRemoveLine);
			Initialize(new HexFileCollectionInfo());
		}

		void Initialize(HexFileCollectionInfo hexFileCollectionInfo)
		{
			Lines = new ObservableRangeCollection<LineViewModel>();
			//foreach (var hexFileInfo in hexFileCollectionInfo.HexFileInfos)
			//{
			//    Lines.Add(new LineViewModel());
			//}
		}

		ObservableCollection<LineViewModel> _lines;
		public ObservableCollection<LineViewModel> Lines
		{
			get { return _lines; }
			set
			{
				_lines = value;
				OnPropertyChanged(() => Lines);
			}
		}

		LineViewModel _selectedLine;
		public LineViewModel SelectedLine
		{
			get { return _selectedLine; }
			set
			{
				_selectedLine = value;
				OnPropertyChanged(() => SelectedLine);
			}
		}

		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
		}

		public RelayCommand SaveFileCommand { get; private set; }
		void OnSaveFile()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Файл конфигурации АЛС (*.ALS)|*.ALS",
				DefaultExt = "Файл конфигурации АЛС (*.ALS)|*.ALS"
			};
			if (saveDialog.ShowDialog().Value)
			{
				if (File.Exists(saveDialog.FileName))
					File.Delete(saveDialog.FileName);
			}
		}
		bool CanSaveFile()
		{
			return true;
		}

		public RelayCommand LoadFileCommand { get; private set; }
		void OnLoadFile()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Файл конфигурации АЛС (*.ALS)|*.ALS"
			};
			if (openFileDialog.ShowDialog() == true)
			{
			}
		}

		public RelayCommand AddLineCommand { get; private set; }
		void OnAddLine()
		{
			var lineViewModel = new LineViewModel();
			Lines.Add(lineViewModel);
			SelectedLine = lineViewModel;
		}
		bool CanAddLine()
		{
			return true;
		}

		public RelayCommand RemoveLineCommand { get; private set; }
		void OnRemoveFile()
		{
			Lines.Remove(SelectedLine);
			SelectedLine = Lines.FirstOrDefault();
		}
		bool CanRemoveLine()
		{
			return SelectedLine != null;
		}		
	}
}
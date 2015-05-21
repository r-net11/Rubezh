using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		Configuration Configuration;

		public MainViewModel()
		{
			CreateNewCommand = new RelayCommand(OnCreateNew);
			SaveToFileCommand = new RelayCommand(OnSaveToFile);
			LoadFromFileCommand = new RelayCommand(OnLoadFromFile);
			AddLineCommand = new RelayCommand(OnAddLine);
			RemoveLineCommand = new RelayCommand(OnRemoveFile, CanRemoveLine);
			GenerateFromRepositoryCommand = new RelayCommand(OnGenerateFromRepository);
			ShowRepositoryCommand = new RelayCommand(OnShowRepository);
			CalculateCommand = new RelayCommand(OnCalculate);
			OnCreateNew();
		}

		void Initialize()
		{
			Lines = new ObservableRangeCollection<LineViewModel>();
			foreach (var line in Configuration.Lines)
			{
				var lineViewModel = new LineViewModel(line);
				foreach (var device in line.Devices)
				{
					var deviceViewModel = new DeviceViewModel(device);
					lineViewModel.Devices.Add(deviceViewModel);
				}
				Lines.Add(lineViewModel);
			}
			SelectedLine = Lines.FirstOrDefault();
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
			Configuration = new Configuration();
			Configuration.Lines.Add(new Line());
			Initialize();
		}

		public RelayCommand SaveToFileCommand { get; private set; }
		void OnSaveToFile()
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

		public RelayCommand LoadFromFileCommand { get; private set; }
		void OnLoadFromFile()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Файл конфигурации АЛС (*.ALS)|*.ALS"
			};
			if (openFileDialog.ShowDialog() == true)
			{
			}

			Initialize();
		}

		public RelayCommand AddLineCommand { get; private set; }
		void OnAddLine()
		{
			var line = new Line();
			Configuration.Lines.Add(line);
			var lineViewModel = new LineViewModel(line);
			Lines.Add(lineViewModel);
			SelectedLine = lineViewModel;
		}

		public RelayCommand RemoveLineCommand { get; private set; }
		void OnRemoveFile()
		{
			Configuration.Lines.Remove(SelectedLine.Line);
			Lines.Remove(SelectedLine);
			SelectedLine = Lines.FirstOrDefault();
		}
		bool CanRemoveLine()
		{
			return SelectedLine != null;
		}

		public RelayCommand GenerateFromRepositoryCommand { get; private set; }
		void OnGenerateFromRepository()
		{
			Processor.Processor.GenerateFromRepository(Configuration);
		}

		public RelayCommand ShowRepositoryCommand { get; private set; }
		void OnShowRepository()
		{
			var repositoryViewModel = new RepositoryViewModel(Configuration);
			DialogService.ShowModalWindow(repositoryViewModel);
		}

		public RelayCommand CalculateCommand { get; private set; }
		void OnCalculate()
		{
			Processor.Processor.Calculate(Configuration);
		}
	}
}
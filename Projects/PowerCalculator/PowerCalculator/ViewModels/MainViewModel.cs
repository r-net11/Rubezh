using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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
            
            Configuration.Lines.Add(new Line().Initialize());
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
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(Configuration));
					using (var streamWriter = new StreamWriter(saveDialog.FileName))
					{
						xmlSerializer.Serialize(streamWriter, Configuration);
					}
                }
                catch (Exception ex)
                {
                    MessageBoxService.ShowError("Ошибка сохранения:\n" + ex.Message);
                }
			}
		}

		public RelayCommand LoadFromFileCommand { get; private set; }
		void OnLoadFromFile()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Файл конфигурации АЛС (*.ALS)|*.ALS"
			};
			if (openFileDialog.ShowDialog().Value)
			{
                try
                {
                    SelectedLine = null;
                    var xmlSerializer = new XmlSerializer(typeof(Configuration));
					using (var streamReader = new StreamReader(openFileDialog.FileName))
					{
						var configuration = (Configuration)xmlSerializer.Deserialize(streamReader);
						Configuration = configuration;
					}       
                }
                catch (Exception ex)
                {
                    MessageBoxService.ShowError("Ошибка загрузки:\n" + ex.Message);
                }
			}

			Initialize();
		}

		public RelayCommand AddLineCommand { get; private set; }
		void OnAddLine()
		{
			var line = new Line().Initialize();
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

		public RelayCommand ShowRepositoryCommand { get; private set; }
		void OnShowRepository()
		{
			var repositoryViewModel = new RepositoryViewModel(Configuration);
			if (DialogService.ShowModalWindow(repositoryViewModel))
			{
				Initialize();
			}
		}

		public RelayCommand CalculateCommand { get; private set; }
		void OnCalculate()
		{
			foreach (var lineViewModel in Lines)
			{
				var lineErors = Processor.Processor.CalculateLine(lineViewModel.Line).ToList();

				foreach (var deviceViewModel in lineViewModel.Devices)
				{
					var lineError = lineErors.FirstOrDefault(x => x.Device == deviceViewModel.Device);
					if (lineError != null)
					{
						deviceViewModel.ErrorType = lineError.ErrorType;
						deviceViewModel.ErrorScale = lineError.ErrorScale;
					}
					else
					{
						deviceViewModel.ErrorType = ErrorType.None;
						deviceViewModel.ErrorScale = 0;
					}
				}
			}
		}
	}
}
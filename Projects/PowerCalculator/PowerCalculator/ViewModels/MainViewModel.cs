using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using PowerCalculator.Models;
using System.Xml.Serialization;
using System;



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
            CollectToRepositoryCommand = new RelayCommand(OnCollectToRepository);
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
            
            Configuration.Lines.Add(new Line().Init());
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
                    XmlSerializer ser = new XmlSerializer(typeof(Configuration));
                    TextWriter writer = new StreamWriter(saveDialog.FileName);
                    ser.Serialize(writer, Configuration);
                    writer.Close();
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
                    XmlSerializer ser = new XmlSerializer(typeof(Configuration));
                    TextReader reader = new StreamReader(openFileDialog.FileName);
                    Configuration conf = (Configuration)ser.Deserialize(reader);
                    reader.Close();

                    Configuration = conf;
                                                            
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
			var line = new Line().Init();
            
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

        public RelayCommand CollectToRepositoryCommand { get; private set; }
        void OnCollectToRepository()
        {
            Processor.Processor.CollectToRepository(Configuration);
            OnShowRepository();
 
        }

		public RelayCommand GenerateFromRepositoryCommand { get; private set; }
		void OnGenerateFromRepository()
		{
            if (Configuration.DeviceRepositoryItems.Count == 0)
            {
                MessageBoxService.ShowError("Репозиторий устройств не содержит элементов!");
                return;
            }

            if (Configuration.CableRepositoryItems.Count == 0)
            {
                MessageBoxService.ShowError("Репозиторий кабелей не содержит элементов!");
                return;
            }

			var cableRemains = Processor.Processor.GenerateFromRepository(Configuration);
            Initialize();
            if (cableRemains.Count() > 0)
            {
                string msg = "Неиспользованный кабель:\n";
                foreach (CableRepositoryItem cablePiece in cableRemains)
                    msg += String.Format("Длина = {0} м, Сопротивление = {1} Ом;\n", cablePiece.Length, cablePiece.Resistivity);
                MessageBoxService.Show(msg);
            }
			
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
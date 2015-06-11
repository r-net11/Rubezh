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
using System.Collections.Generic;
using PowerCalculator.Processor;

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
            EditCableTypesRepositoryCommand = new RelayCommand(OnEditCableTypesRepository);
			ShowSpecificationCommand = new RelayCommand(OnShowSpecification);
			PatchCommand = new RelayCommand(OnPatch);
			OnCreateNew();

            CableTypesRepository.LoadOrDefault(CableTypesPath);
		}

		void Initialize()
		{
			Lines = new ObservableRangeCollection<LineViewModel>();
			foreach (var line in Configuration.Lines)
			{
				var lineViewModel = new LineViewModel(line);
				Lines.Add(lineViewModel);
			}
            RenameLines();
			SelectedLine = Lines.FirstOrDefault();
		}

        string CableTypesPath { get { return AppDomain.CurrentDomain.BaseDirectory + "\\CableTypes.xml"; } }
                       
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

        void RenameLines(int startIndex = 0)
        {
            for (int i = startIndex; i < Lines.Count; i++)
            {
                int index1 = i / 8 + 1;
                int index2 = i % 8 + 1;
                Lines[i].Name = "АЛС " + index1.ToString() + "." + index2.ToString();
            }
        }

		public RelayCommand AddLineCommand { get; private set; }
		void OnAddLine()
		{
			var line = new Line();
            
			Configuration.Lines.Add(line);
			var lineViewModel = new LineViewModel(line);
			Lines.Add(lineViewModel);
            RenameLines(Lines.Count - 1);
			SelectedLine = lineViewModel;
		}

		public RelayCommand RemoveLineCommand { get; private set; }
		void OnRemoveFile()
		{
            int index = Lines.IndexOf(SelectedLine);
			Configuration.Lines.Remove(SelectedLine.Line);
			Lines.Remove(SelectedLine);
            RenameLines(index);
			SelectedLine = Lines.FirstOrDefault();
		}
		bool CanRemoveLine()
		{
			return SelectedLine != null;
		}

		public RelayCommand ShowSpecificationCommand { get; private set; }
		void OnShowSpecification()
		{
			var specificationViewModel = new SpecificationViewModel(Configuration, Initialize);
            DialogService.ShowModalWindow(specificationViewModel);
		}

        public RelayCommand EditCableTypesRepositoryCommand { get; private set; }
        void OnEditCableTypesRepository()
		{
            var cableTypesRepositoryViewModel = new CableTypesRepositoryViewModel();
            if (DialogService.ShowModalWindow(cableTypesRepositoryViewModel))
			{
                CableTypesRepository.SaveToFile(CableTypesPath);
			}
		}
        
		public RelayCommand PatchCommand { get; private set; }
		void OnPatch()
		{
            string message = String.Empty;
            foreach (var lineViewModel in Lines)
            {
                if (!lineViewModel.HasError)
                    continue;
                var patch = lineViewModel.GetPatch();
                if (patch == null)
                    message += lineViewModel.Name + ";\n";
                else
                    lineViewModel.InstallPatch(patch);
            }

            if (message != String.Empty)
                MessageBoxService.ShowWarning("Следующие АЛС не удалось исправить из-за отсутствия необходимого количества мест для модулей подпитки:\n" + message);
                
		}       
	}
}
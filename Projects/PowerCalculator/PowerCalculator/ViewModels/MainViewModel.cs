using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Microsoft.Win32;
using PowerCalculator.Models;
using System.Collections.Generic;
using PowerCalculator.Processor;
using System.Text;
using System.Diagnostics;

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
			RemoveLineCommand = new RelayCommand(OnRemoveLine, CanRemoveLine);
            PatchLineCommand = new RelayCommand(OnPatchLine, CanPatchLine);
            EditCableTypesRepositoryCommand = new RelayCommand(OnEditCableTypesRepository);
            ShowSpecificationCommand = new RelayCommand(OnShowSpecification);
            CollectToSpecificationCommand = new RelayCommand(OnCollectToSpecification, CanCollectToSpecification);
			PatchCommand = new RelayCommand(OnPatch, CanPatch);
            AboutCommand = new RelayCommand(OnAbout);
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

        string CableTypesPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CableTypes.xml"); } }
                       
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
		void OnRemoveLine()
		{
            int index = Lines.IndexOf(SelectedLine);
			Configuration.Lines.Remove(SelectedLine.Line);
			Lines.Remove(SelectedLine);
            RenameLines(index);
            index = Math.Min(index, Lines.Count - 1);
            if (index > -1)
                SelectedLine = Lines[index];
		}
		bool CanRemoveLine()
		{
			return SelectedLine != null;
		}

        public RelayCommand PatchLineCommand { get; private set; }
        void OnPatchLine()
        {
            var patch = SelectedLine.GetPatch();

            if (patch == null)
            {
                MessageBoxService.Show("Для исправления АЛС недостаточно свободных мест!");
            }
            else
            {
                var question = new StringBuilder().AppendLine("Список адресов недостающих модулей подпитки:");
                for (int i = 0; i < patch.Count(); i++)
                    question.Append(SelectedLine.Devices[patch[i] - i].Address + i).Append(i == patch.Count() - 1 ? "" : ", ");
                question.AppendLine();

                if (MessageBoxService.ShowQuestion(question.ToString()))
                    SelectedLine.InstallPatch(patch);
            }
        }

        bool CanPatchLine()
        {
            return SelectedLine != null && SelectedLine.HasError;
        }

		public RelayCommand ShowSpecificationCommand { get; private set; }
		void OnShowSpecification()
		{
			var specificationViewModel = new SpecificationViewModel(Configuration, Initialize);
            DialogService.ShowModalWindow(specificationViewModel);
		}

        public RelayCommand CollectToSpecificationCommand { get; private set; }
        void OnCollectToSpecification()
        {
            var specificationViewModel = 
                new SpecificationViewModel(Configuration, Initialize, 
                    Processor.Processor.CollectDevices(Configuration.Lines), 
                    Processor.Processor.CollectCables(Configuration.Lines));
            DialogService.ShowModalWindow(specificationViewModel);
        }
        bool CanCollectToSpecification()
        {
            return Lines.Sum(x=>x.Devices.Count) > 0;
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
            Dictionary<LineViewModel, List<int>> patches = new Dictionary<LineViewModel, List<int>>();
            foreach (var lineViewModel in Lines)
            {
                if (lineViewModel.HasError)
                    patches.Add(lineViewModel, lineViewModel.GetPatch());
            }

            var question = new StringBuilder().AppendLine("Список адресов недостающих модулей подпитки:");
            foreach (var patch in patches)
            {
                question.Append(patch.Key.Name).Append(": ");
                
                if (patch.Value == null)
                    question.Append("недостаточно мест;");
                else
                    for (int i = 0; i < patch.Value.Count(); i++)
                        question.Append(patch.Key.Devices[patch.Value[i] - i].Address + i).Append(i == patch.Value.Count() - 1 ? "" : ", ");
                
                question.AppendLine();
            }
            if (MessageBoxService.ShowQuestion(question.ToString()))
                foreach (var patch in patches)
                    if (patch.Value != null)
                        patch.Key.InstallPatch(patch.Value);
        }    
        
        bool CanPatch()
        {
            return Lines.Any(x=>x.HasError);
        }

        public RelayCommand AboutCommand { get; private set; }
        void OnAbout()
        {
            try
            {
                Process.Start(Path.Combine(Environment.CurrentDirectory, "PowerCalulatorManual.pdf"));
            }
            catch
            { 
            }
        }   
	}
}
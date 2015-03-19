using System.Collections.ObjectModel;
using System.Linq;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;

namespace GKModule.ViewModels
{
	public class DescriptorsViewModel : MenuViewPartViewModel
	{
		public DescriptorsViewModel()
		{
			Menu = new DescriptorsMenuViewModel(this);
			RefreshCommand = new RelayCommand(OnRefresh);
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			DescriptorsManager.Create();
			Databases = new ObservableCollection<CommonDatabase>();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				Databases.Add(gkDatabase);
			}
			foreach (var kauDatabase in DescriptorsManager.KauDatabases)
			{
				Databases.Add(kauDatabase);
			}
			SelectedDatabase = Databases.FirstOrDefault();

			foreach (var database in Databases)
			{
				foreach (var descriptor in database.Descriptors)
				{
					var isFormulaInvalid = descriptor.Formula.CalculateStackLevels();
					if (isFormulaInvalid)
					{
						MessageBoxService.ShowError("Ошибка глубины стека дескриптора " + descriptor.GKBase.GKDescriptorNo + " " + descriptor.GKBase.PresentationName);
						return;
					}
				}
			}
		}

		ObservableCollection<CommonDatabase> _databases;
		public ObservableCollection<CommonDatabase> Databases
		{
			get { return _databases; }
			set
			{
				_databases = value;
				OnPropertyChanged(() => Databases);
			}
		}

		CommonDatabase _selectedDatabase;
		public CommonDatabase SelectedDatabase
		{
			get { return _selectedDatabase; }
			set
			{
				_selectedDatabase = value;
				OnPropertyChanged(() => SelectedDatabase);
				if (value != null)
				{
					InitializeSelectedDB();
				}
			}
		}

		void InitializeSelectedDB()
		{
			Descriptors = new ObservableCollection<DescriptorViewModel>();
			foreach (var descriptor in SelectedDatabase.Descriptors)
			{
				var binObjectViewModel = new DescriptorViewModel(descriptor, this);
				Descriptors.Add(binObjectViewModel);
			}
			SelectedDescriptor = Descriptors.FirstOrDefault();

			foreach (var descriptorViewModel in Descriptors)
			{
				descriptorViewModel.InputDescriptors = new ObservableCollection<DescriptorViewModel>();
				if (descriptorViewModel.Descriptor.GKBase.InputGKBases != null)
				foreach (var inputBase in descriptorViewModel.Descriptor.GKBase.InputGKBases)
				{
					var inputDescriptor = SelectedDatabase.Descriptors.FirstOrDefault(x => x.GKBase.UID == inputBase.UID);
					if (inputDescriptor != null)
					{
						var inputDescriptorViewModel = Descriptors.FirstOrDefault(x => x.Descriptor.GKBase.UID == inputDescriptor.GKBase.UID);
						if (inputDescriptorViewModel != null)
							descriptorViewModel.InputDescriptors.Add(inputDescriptorViewModel);
						else
							MessageBoxService.ShowError("Отсутствует ссылка на входную зависимость" + descriptorViewModel.Descriptor.GKBase.GKDescriptorNo + " " + descriptorViewModel.Descriptor.GKBase.PresentationName);
					}
				}
			}

			foreach (var descriptorViewModel in Descriptors)
			{
				descriptorViewModel.OutputDescriptors = new ObservableCollection<DescriptorViewModel>();
				if (descriptorViewModel.Descriptor.GKBase.InputGKBases != null)
				foreach (var outputBase in descriptorViewModel.Descriptor.GKBase.OutputGKBases)
				{
					var outputDescriptor = SelectedDatabase.Descriptors.FirstOrDefault(x => x.GKBase.UID == outputBase.UID);
					if (outputDescriptor != null)
					{
						var outputDescriptorViewModel = Descriptors.FirstOrDefault(x => x.Descriptor.GKBase.UID == outputDescriptor.GKBase.UID);
						if (outputDescriptorViewModel == null)
						{
						}
						if (outputDescriptorViewModel != null)
							descriptorViewModel.OutputDescriptors.Add(outputDescriptorViewModel);
						else
							MessageBoxService.ShowError("Отсутствует ссылка на выходную зависимость" + descriptorViewModel.Descriptor.GKBase.GKDescriptorNo + " " + descriptorViewModel.Descriptor.GKBase.PresentationName);
					}
				}
			}

			foreach (var descriptorViewModel in Descriptors)
			{
				descriptorViewModel.InitializeLogic();
			}
		}

		ObservableCollection<DescriptorViewModel> _descriptors;
		public ObservableCollection<DescriptorViewModel> Descriptors
		{
			get { return _descriptors; }
			set
			{
				_descriptors = value;
				OnPropertyChanged(() => Descriptors);
			}
		}

		DescriptorViewModel _selectedDescriptor;
		public DescriptorViewModel SelectedDescriptor
		{
			get { return _selectedDescriptor; }
			set
			{
				_selectedDescriptor = value;
				OnPropertyChanged(() => SelectedDescriptor);
			}
		}
	}
}
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
						MessageBoxService.ShowError("Ошибка глубины стека дескриптора " + descriptor.XBase.GKDescriptorNo + " " + descriptor.XBase.PresentationName);
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
				foreach (var inputBase in descriptorViewModel.Descriptor.XBase.InputXBases)
				{
					foreach (var gkDatabase in DescriptorsManager.GkDatabases)
					{
						var inputDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.XBase.BaseUID == inputBase.BaseUID);
						if (inputDescriptor != null)
						{
							var inputDescriptorViewModel = Descriptors.FirstOrDefault(x => x.Descriptor.XBase.BaseUID == inputDescriptor.XBase.BaseUID);
							descriptorViewModel.InputDescriptors.Add(inputDescriptorViewModel);
						}
					}
				}
			}

			foreach (var descriptorViewModel in Descriptors)
			{
				descriptorViewModel.OutputDescriptors = new ObservableCollection<DescriptorViewModel>();
				foreach (var outputBase in descriptorViewModel.Descriptor.XBase.OutputXBases)
				{
					foreach (var gkDatabase in DescriptorsManager.GkDatabases)
					{
						var outputDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.XBase.BaseUID == outputBase.BaseUID);
						if (outputDescriptor != null)
						{
							var outputDescriptorViewModel = Descriptors.FirstOrDefault(x => x.Descriptor.XBase.BaseUID == outputDescriptor.XBase.BaseUID);
							descriptorViewModel.OutputDescriptors.Add(outputDescriptorViewModel);
						}
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DescriptorsViewModel : BaseViewModel
	{
		public DescriptorsViewModel()
		{
			Databases = new List<CommonDatabase>();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				Databases.Add(gkDatabase);
			}
			foreach (var kauDatabase in DescriptorsManager.KauDatabases)
			{
				Databases.Add(kauDatabase);
			}
			SelectedDatabase = Databases.FirstOrDefault();
		}

		public List<CommonDatabase> Databases { get; private set; }

		CommonDatabase _selectedDatabase;
        public CommonDatabase SelectedDatabase
        {
            get { return _selectedDatabase; }
            set
            {
                _selectedDatabase = value;
                OnPropertyChanged("SelectedDatabase");
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
				var binObjectViewModel = new DescriptorViewModel(descriptor);
				Descriptors.Add(binObjectViewModel);
			}
			SelectedDescriptor = Descriptors.FirstOrDefault();
		}

		ObservableCollection<DescriptorViewModel> _descriptors;
		public ObservableCollection<DescriptorViewModel> Descriptors
		{
			get { return _descriptors; }
			set
			{
				_descriptors = value;
				OnPropertyChanged("Descriptors");
			}
		}

		DescriptorViewModel _selectedDescriptor;
		public DescriptorViewModel SelectedDescriptor
		{
			get { return _selectedDescriptor; }
			set
			{
				_selectedDescriptor = value;
				OnPropertyChanged("SelectedDescriptor");
			}
		}
	}
}
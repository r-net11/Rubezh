using System.Collections.Generic;
using System.Linq;
using Common.GK;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DatabasesViewModel : DialogViewModel
	{
		public DatabasesViewModel()
		{
			Title = "Бинарный формат конфигурации";

			Databases = new List<CommonDatabase>();
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				Databases.Add(gkDatabase);
			}
			foreach (var kauDatabase in DatabaseManager.KauDatabases)
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
			BinaryObjects = new ObservableCollection<BinaryObjectViewModel>();
			foreach (var binaryObject in SelectedDatabase.BinaryObjects)
			{
				var binObjectViewModel = new BinaryObjectViewModel(binaryObject);
				BinaryObjects.Add(binObjectViewModel);
			}
			SelectedBinaryObject = BinaryObjects.FirstOrDefault();
		}

		ObservableCollection<BinaryObjectViewModel> _binaryObjects;
		public ObservableCollection<BinaryObjectViewModel> BinaryObjects
		{
			get { return _binaryObjects; }
			set
			{
				_binaryObjects = value;
				OnPropertyChanged("BinaryObjects");
			}
		}

		BinaryObjectViewModel _selectedBinaryObject;
		public BinaryObjectViewModel SelectedBinaryObject
		{
			get { return _selectedBinaryObject; }
			set
			{
				_selectedBinaryObject = value;
				OnPropertyChanged("SelectedBinaryObject");
			}
		}
	}
}
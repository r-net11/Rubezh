using System.Collections.Generic;
using Commom.GK;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DatabasesViewModel : DialogViewModel
	{
		public DatabasesViewModel()
		{
			Title = "Бинарный формат конфигурации";

			Databases = new List<CommonDatabase>();
			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				Databases.Add(gkDatabase);
			}
			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
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
				InitializeSelectedDB();
			}
		}

		void InitializeSelectedDB()
		{
			BinaryObjects = new List<BinaryObjectViewModel>();

			foreach (var binaryObject in SelectedDatabase.BinaryObjects)
			{
				var binObjectViewModel = new BinaryObjectViewModel(binaryObject);
				BinaryObjects.Add(binObjectViewModel);
			}
			SelectedBinaryObject = BinaryObjects.FirstOrDefault();
		}

		List<BinaryObjectViewModel> _binaryObjects;
		public List<BinaryObjectViewModel> BinaryObjects
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
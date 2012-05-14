using System.Collections.Generic;
using GKModule.Database;
using Infrastructure.Common;

namespace GKModule.ViewModels
{
	public class DatabasesViewModel : DialogContent
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

			if (Databases.Count > 0)
				SelectedDatabase = Databases[0];
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

			if (BinaryObjects.Count > 0)
				SelectedBinaryObject = BinaryObjects[0];
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
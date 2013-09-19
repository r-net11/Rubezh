using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using GKImitator.Processor;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.Models;
using Common.GK;

namespace GKImitator.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		GKProcessor GKProcessor;

		public MainViewModel()
		{
			Title = "Имитатор ГК";

			GetConfiguration();
			InitializeBinaryObjects();

			GKProcessor = new GKProcessor();
			GKProcessor.Start();
		}

		void GetConfiguration()
		{
			FiresecManager.Connect(ClientType.Other, ConnectionSettingsManager.ServerAddress, "adm", "");
			FiresecManager.GetConfiguration("GKImitator/Configuration");
			GKDriversCreator.Create();
			XManager.UpdateConfiguration();
			FiresecManager.Disconnect();
		}

		void InitializeBinaryObjects()
		{
			DatabaseManager.Convert();
			var gkDatabase = DatabaseManager.GkDatabases.FirstOrDefault();

			BinaryObjects = new List<BinaryObjectViewModel>();
			foreach (var binaryObject in gkDatabase.BinaryObjects)
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
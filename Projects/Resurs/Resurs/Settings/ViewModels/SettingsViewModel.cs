using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		public SettingsViewModel()
		{
			GenerateCommand = new RelayCommand(OnGenerate);
		}

		public RelayCommand GenerateCommand { get; private set; }
		void OnGenerate()
		{
			DropDB();
			CreateDevices();
			CreateApartments();
			CreateUsers();
		}

		void DropDB()
		{

		}

		void CreateDevices()
		{

		}

		void CreateApartments()
		{

		}

		void CreateUsers()
		{

		}
	}
}
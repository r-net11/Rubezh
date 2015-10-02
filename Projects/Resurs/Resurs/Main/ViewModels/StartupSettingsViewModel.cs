using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class StartupSettingsViewModel : SaveCancelDialogViewModel
	{
		public StartupSettingsViewModel()
		{
			Title = "Настройка соединения";
			SettingsManager.Load();
			ConnectionString = SettingsManager.ResursSettings.ConnectionString;
		}

		string _connectionString;
		public string ConnectionString
		{
			get { return _connectionString; }
			set
			{
				_connectionString = value;
				OnPropertyChanged(() => ConnectionString);
			}
		}

		protected override bool Save()
		{
			SettingsManager.ResursSettings.ConnectionString = ConnectionString;
			SettingsManager.Save();
			return base.Save();
		}
	}
}
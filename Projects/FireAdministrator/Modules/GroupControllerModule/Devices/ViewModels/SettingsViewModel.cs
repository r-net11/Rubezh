using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using System.Collections.ObjectModel;
using RubezhClient;

namespace GKModule.ViewModels
{
	public class SettingsViewModel : SaveCancelDialogViewModel
	{
		public SettingsViewModel()
		{
			Title = "Настройка проекта";
			NameGenerationTypes = new ObservableCollection<GKNameGenerationType>(Enum.GetValues(typeof(GKNameGenerationType)).Cast<GKNameGenerationType>());
			SelectedNameGenerationType = GKManager.DeviceConfiguration.GKNameGenerationType;
		}

		public ObservableCollection<GKNameGenerationType> NameGenerationTypes { get; private set; }

		GKNameGenerationType _selectedNameGenerationType;
		public GKNameGenerationType SelectedNameGenerationType
		{
			get { return _selectedNameGenerationType; }
			set
			{
				_selectedNameGenerationType = value;
				OnPropertyChanged(() => SelectedNameGenerationType);
			}
		}
	}
}
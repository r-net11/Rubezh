using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure;

namespace StrazhModule.ViewModels
{
	[SaveSizeAttribute]
	public class ZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		public ZoneSelectationViewModel(Guid zoneUID)
		{
			Title = "Выбор зоны";
			CreateCommand = new RelayCommand(OnCreate);
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in SKDManager.Zones.OrderBy(x => x.No))
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
		}
		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		void OnCreate()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				SKDManager.Zones.Add(zoneDetailsViewModel.Zone);
				var zoneViewModel = new ZoneViewModel(zoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
				ZonesViewModel.Current.Zones.Add(zoneViewModel);
				ServiceFactory.SaveService.SKDChanged = true;
				SaveCommand.Execute();
			}	
		}

		protected override bool CanSave()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}
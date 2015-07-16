using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecAPI.GK;
using System;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class OPCsViewModel : MenuViewPartViewModel
	{

		public OPCsViewModel()
		{
			AddCommand = new RelayCommand(Add);
			Menu = new OPCMenuViewModel(this);
					
		}
		public void Initialize()
		{
			Objects = new ObservableCollection<OPCItemViewModel>();

			foreach (var zone in GKManager.Zones.Where(x=> GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(zone);
				Objects.Add(OPCItem);
			}

			foreach (var device in GKManager.Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(device);
				Objects.Add(OPCItem);
			}

			foreach (var delay in GKManager.Delays.Where(x=> GKManager.DeviceConfiguration.OPCSettings.DelayUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(delay);
				Objects.Add(OPCItem);
			}

			foreach (var guardZone in GKManager.GuardZones.Where(x=> GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs.Contains(x.UID)))
			{
				var  OPCItem = new OPCItemViewModel(guardZone);
				Objects.Add(OPCItem);
			}

			foreach (var derection in GKManager.Directions.Where(x => GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(derection);
				Objects.Add(OPCItem);
			}

			foreach (var mpt in GKManager.MPTs.Where(x => GKManager.DeviceConfiguration.OPCSettings.MPTUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(mpt);
				Objects.Add(OPCItem);
			}

			foreach (var pump in GKManager.PumpStations.Where(x=> GKManager.DeviceConfiguration.OPCSettings.NSUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(pump);
				Objects.Add(OPCItem);	
			}

			SelectedObject = Objects.FirstOrDefault();
		}


		ObservableCollection<OPCItemViewModel> _obbjects;
		public ObservableCollection<OPCItemViewModel> Objects
		{
			get { return _obbjects; }
			private set
			{
				_obbjects = value;
				OnPropertyChanged(() => Objects);
			}
		}

		OPCItemViewModel _selectedObject;
		public OPCItemViewModel SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				OnPropertyChanged(() => SelectedObject);
			}
		}
		public RelayCommand AddCommand { get; private set; }
		void Add()
		{
			var opcDetailsViewModel = new OPCDetailsViewModel();
			if (DialogService.ShowModalWindow(opcDetailsViewModel))
			{
				Initialize();
			}			
		}

		public RelayCommand DeleteCommand { get; private set; }
		void Delete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить" + SelectedObject.Object.PresentationName))
			{
				var index = Objects.IndexOf(SelectedObject);
				Objects.Remove(SelectedObject);
				index = Math.Min(index, Objects.Count - 1);
				if (index > -1)
					SelectedObject = Objects[index];
				ServiceFactory.SaveService.GKChanged = true;
			}	
		}

		public override void OnShow()
		{
			Initialize();
			base.OnShow();
		}

		
	}
}
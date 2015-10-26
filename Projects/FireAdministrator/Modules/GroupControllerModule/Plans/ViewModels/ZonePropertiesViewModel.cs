﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		ZonesViewModel _zonesViewModel;

		public ZonePropertiesViewModel(IElementZone iElementZone, ZonesViewModel zonesViewModel)
		{
			_zonesViewModel = zonesViewModel;
			IElementZone = iElementZone;
			Title = "Свойства фигуры: Зона";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in GKManager.DeviceConfiguration.SortedZones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);

			ShowState = IElementZone.ShowState;
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

		bool _showState;
		public bool ShowState
		{
			get { return _showState; }
			set
			{
				_showState = value;
				OnPropertyChanged(() => ShowState);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		void OnCreate()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			var createZoneEventArg = new CreateGKZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
			{
				GKPlanExtension.Instance.Cache.BuildSafe<GKZone>();
				GKPlanExtension.Instance.SetItem<GKZone>(IElementZone, createZoneEventArg.Zone.UID);
			}
			UpdateZones(zoneUID);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update();
		}
		bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			IElementZone.ShowState = ShowState;
			Guid zoneUID = IElementZone.ZoneUID;			
			GKPlanExtension.Instance.SetItem<GKZone>(IElementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		void UpdateZones(Guid xzoneUID)
		{
			if (_zonesViewModel != null)
			{
				if (xzoneUID != IElementZone.ZoneUID)
					Update(xzoneUID);
				Update(IElementZone.ZoneUID);
				_zonesViewModel.LockedSelect(IElementZone.ZoneUID);
			}
		}
		void Update(Guid zoneUID)
		{
			var zone = _zonesViewModel.Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}
using System;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using SKDModule.Events;
using SKDModule.Plans.Designer;
using SKDModule.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SKDModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		private IElementZone IElementZone;

		public ZonePropertiesViewModel(IElementZone iElementZone, ZonesViewModel zonesViewModel)
		{
			Zones = zonesViewModel.Zones;
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Зона";
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
			IsHiddenZone = iElementZone.IsHiddenZone;
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		private ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		private bool _isHiddenZone;
		public bool IsHiddenZone
		{
			get { return _isHiddenZone; }
			set
			{
				_isHiddenZone = value;
				OnPropertyChanged(() => IsHiddenZone);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			var createZoneEventArg = new CreateSKDZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateSKDZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
			{
				IElementZone.ZoneUID = createZoneEventArg.Zone.UID;
				Helper.BuildMap();
				Helper.SetSKDZone(IElementZone);
				UpdateZones(zoneUID);
				Close(true);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditSKDZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update(SelectedZone.Zone);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		//protected override bool CanSave()
		//{
		//    return SelectedZone == null;
		//}
		protected override bool Save()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			Helper.SetSKDZone(IElementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		private void UpdateZones(Guid zoneUID)
		{
			IElementZone.IsHiddenZone = IsHiddenZone;
			if (Zones != null)
			{
				if (zoneUID != IElementZone.ZoneUID)
					Update(zoneUID);
				Update(IElementZone.ZoneUID);
				//Zones.LockedSelect(IElementZone.ZoneUID);
			}
		}
		private void Update(Guid zoneUID)
		{
			var zone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}
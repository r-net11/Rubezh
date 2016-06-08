using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Plans.Elements;
using StrazhAPI.SKD;
using StrazhModule.Events;
using StrazhModule.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace StrazhModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		readonly IElementZone _elementZone;

		public ZonePropertiesViewModel(IElementZone iElementZone, ZonesViewModel zonesViewModel)
		{
			Zones = zonesViewModel.Zones;
			_elementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, () => SelectedZone != null);
			Title = "Свойства фигуры: Зона СКД";
			SetSelectedZone();
		}

		private void SetSelectedZone()
		{
			if (_elementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == _elementZone.ZoneUID);
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
			var zoneUID = _elementZone.ZoneUID;
			var createZoneEventArg = new CreateSKDZoneEventArg();
			ServiceFactoryBase.Events.GetEvent<CreateSKDZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
			{
				SKDPlanExtension.Instance.Cache.BuildSafe<SKDZone>();
				SKDPlanExtension.Instance.SetItem<SKDZone>(_elementZone, createZoneEventArg.Zone.UID);
				UpdateZones(zoneUID);
				Close(true);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactoryBase.Events.GetEvent<EditSKDZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update(SelectedZone.Zone);
		}

		protected override bool Save()
		{
			var zoneUID = _elementZone.ZoneUID;
			SKDPlanExtension.Instance.SetItem(_elementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		void UpdateZones(Guid zoneUID)
		{
			if (Zones == null) return;

			if (zoneUID != _elementZone.ZoneUID)
				Update(zoneUID);
			Update(_elementZone.ZoneUID);
			//Zones.LockedSelect(IElementZone.ZoneUID);
		}
		void Update(Guid zoneUID)
		{
			var zone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}
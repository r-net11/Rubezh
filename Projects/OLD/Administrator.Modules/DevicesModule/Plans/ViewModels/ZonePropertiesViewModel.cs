using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Plans.Designer;
using DevicesModule.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;

namespace DevicesModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		private IElementZone IElementZone;
		private ZonesViewModel _zonesViewModel;

		public ZonePropertiesViewModel(IElementZone iElementZone, ZonesViewModel zonesViewModel)
		{
			_zonesViewModel = zonesViewModel;
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Зона";
			var zones = (from zone in FiresecManager.Zones orderby zone.No select zone);
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
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
			var createZoneEventArg = new CreateZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
				IElementZone.ZoneUID = createZoneEventArg.Zone.UID;
			Helper.BuildMap();
			Helper.SetZone(IElementZone);
			UpdateZones(zoneUID);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update(SelectedZone.Zone);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			Helper.SetZone(IElementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		private void UpdateZones(Guid zoneUID)
		{
			IElementZone.IsHiddenZone = IsHiddenZone;
			if (_zonesViewModel != null)
			{
				if (zoneUID != IElementZone.ZoneUID)
					Update(zoneUID);
				Update(IElementZone.ZoneUID);
				_zonesViewModel.LockedSelect(IElementZone.ZoneUID);
			}
		}
		private void Update(Guid zoneUID)
		{
			var zone = _zonesViewModel.Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}
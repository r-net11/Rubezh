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

namespace SKDModule.Plans.ViewModels
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
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = RootZone.GetAllChildren().FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
			IsHiddenZone = iElementZone.IsHiddenZone;
		}

		private ZoneViewModel AddZoneInternal(SKDZone zone, ZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new ZoneViewModel(zone);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
				AddZoneInternal(childZone, zoneViewModel);
			return zoneViewModel;
		}

		private ZoneViewModel _rootZone;
		public ZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged(() => RootZone);
			}
		}

		public ZoneViewModel[] RootZones
		{
			get { return new ZoneViewModel[] { RootZone }; }
		}

		private ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (SelectedZone != null)
					SelectedZone.ExpandToThis();
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
			var createZoneEventArg = new CreateSKDZoneEventArg()
			{
				ParentZoneUID = (SelectedZone == null ? RootZone : SelectedZone).Zone.UID
			};
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
			return SelectedZone != null && !SelectedZone.Zone.IsRootZone;
		}

		protected override bool CanSave()
		{
			return SelectedZone == null || !SelectedZone.Zone.IsRootZone;
		}
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
			var zone = _zonesViewModel.AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}
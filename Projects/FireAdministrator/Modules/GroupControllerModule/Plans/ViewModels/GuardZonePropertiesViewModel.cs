using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class GuardZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		public GuardZonePropertiesViewModel(IElementZone iElementZone)
		{
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Охранная зона";
			Zones = new ObservableCollection<GuardZoneViewModel>();
			foreach (var zone in GKManager.DeviceConfiguration.GuardZones)
			{
				var zoneViewModel = new GuardZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<GuardZoneViewModel> Zones { get; private set; }

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			var createGuardZoneEventArg = new CreateGKGuardZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Publish(createGuardZoneEventArg);
			if (createGuardZoneEventArg.Zone != null)
				GKPlanExtension.Instance.RewriteItem(IElementZone, createGuardZoneEventArg.Zone);
			if (!createGuardZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update();
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}
		protected override bool Save()
		{
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone.Zone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}
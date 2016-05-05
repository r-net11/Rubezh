using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
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
			Zones = new ObservableCollection<GKGuardZone>(GKManager.GuardZones);
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<GKGuardZone> Zones { get; private set; }

		GKGuardZone _selectedZone;
		public GKGuardZone SelectedZone
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
			ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Publish(SelectedZone.UID);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}
		protected override bool Save()
		{
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}
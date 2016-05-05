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
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		public ZonePropertiesViewModel(IElementZone iElementZone)
		{
			IElementZone = iElementZone;
			Title = "Свойства фигуры: Пожарная зона";
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
			var createZoneEventArg = new CreateGKZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
				GKPlanExtension.Instance.RewriteItem(IElementZone, createZoneEventArg.Zone);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Publish(SelectedZone.Zone.UID);
		}
		bool CanEdit()
		{
			return SelectedZone != null;
		}
		protected override bool Save()
		{
			IElementZone.ShowState = ShowState;
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone.Zone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}
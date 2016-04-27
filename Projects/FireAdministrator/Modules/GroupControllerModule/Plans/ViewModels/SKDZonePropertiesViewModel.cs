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
	public class SKDZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;

		public SKDZonePropertiesViewModel(IElementZone iElementZone)
		{
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: СКД Зона";
			var zones = GKManager.SKDZones;
			Zones = new ObservableCollection<SKDZoneViewModel>();
			foreach (var zone in zones)
			{
				var zoneViewModel = new SKDZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<SKDZoneViewModel> Zones { get; private set; }

		SKDZoneViewModel _selectedZone;
		public SKDZoneViewModel SelectedZone
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
			var createSKDZoneEventArg = new CreateGKSKDZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Publish(createSKDZoneEventArg);
			if (createSKDZoneEventArg.Zone != null)
				GKPlanExtension.Instance.RewriteItem(IElementZone, createSKDZoneEventArg.Zone);
			if (!createSKDZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Publish(SelectedZone.Zone.UID);
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
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class SKDZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }

		public SKDZonePropertiesViewModel(IElementZone element, CommonDesignerCanvas designerCanvas)
		{
			IElementZone = element;
			PositionSettingsViewModel = new PositionSettingsViewModel(element as ElementBase, designerCanvas);
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: СКД Зона";
			Zones = new ObservableCollection<GKSKDZone>(GKManager.SKDZones);
			if (element.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == element.ZoneUID);
		}

		public ObservableCollection<GKSKDZone> Zones { get; private set; }

		GKSKDZone _selectedZone;
		public GKSKDZone SelectedZone
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
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Publish(SelectedZone.UID);
			Zones = new ObservableCollection<GKSKDZone>(GKManager.SKDZones);
			OnPropertyChanged(() => Zones);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			PositionSettingsViewModel.SavePosition();
			GKPlanExtension.Instance.RewriteItem(IElementZone, SelectedZone);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}
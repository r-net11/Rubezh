using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Integration.OPC.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		private readonly IElementZone _zoneElement;
		private readonly BasePlanExtension _extension;
		OPCZone _selectedZone;

		public ObservableCollection<OPCZone> Zones { get; private set; }

		public OPCZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public ZonePropertiesViewModel(IElementZone zoneElement, IEnumerable<OPCZone> zones, BasePlanExtension extension)
		{
			_extension = extension;
			ShowPropertyCommand = new RelayCommand(OnShowPropertyCommand, () => SelectedZone != null);
			Title = "Свойства фигуры: Зона ОПС";
			_zoneElement = zoneElement;
			Zones = new ObservableCollection<OPCZone>(zones);
			SetSelectedZone();
		}

		private void SetSelectedZone()
		{
			if (_zoneElement != null)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == _zoneElement.ZoneUID);
		}

		public RelayCommand ShowPropertyCommand { get; set; }
		public RelayCommand CreateCommand { get; set; }

		private void OnShowPropertyCommand()
		{
			DialogService.ShowModalWindow(new PropertiesDialogViewModel(SelectedZone));
		}

		protected override bool Save()
		{
			var zoneUID = _zoneElement.ZoneUID;
			_extension.SetItem(_zoneElement, SelectedZone == null ? null : SKDManager.SKDConfiguration.OPCZones.FirstOrDefault(x => x.UID == SelectedZone.UID));
			UpdateZones(zoneUID);
			return base.Save();
		}

		void UpdateZones(Guid zoneUID)
		{
			if (Zones == null) return;

			if (zoneUID != _zoneElement.ZoneUID)
				Update(zoneUID);
			Update(_zoneElement.ZoneUID);
			//Zones.LockedSelect(IElementZone.ZoneUID);
		}
		void Update(Guid zoneUID)
		{
			var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
				zone.UpdateVisualizationState();
		}
	}
}

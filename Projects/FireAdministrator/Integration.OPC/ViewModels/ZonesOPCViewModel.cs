using System;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Events;
using Integration.OPC.Properties;
using Microsoft.Practices.Prism;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.SKD;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OPCSettings = Integration.OPC.Models.OPCSettings;
using OPCZone = Integration.OPC.Models.OPCZone;

namespace Integration.OPC.ViewModels
{
	public class ZonesOPCViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		private OPCZone _selectedZoneOPC;
		private bool _lockSelection;

		#region Properties
		public ObservableCollection<OPCZone> ZonesOPC { get; set; }

		public OPCZone SelectedZoneOPC
		{
			get { return _selectedZoneOPC; }
			set
			{
				if (_selectedZoneOPC == value) return;
				_selectedZoneOPC = value;
				OnPropertyChanged(() => SelectedZoneOPC);
			}
		}
		#endregion

		public ZonesOPCViewModel()
		{
			_lockSelection = false;
			SubscribeEvents();
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedZoneOPC != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedZoneOPC != null);
			SettingsCommand = new RelayCommand(OnSettings);
			IsRightPanelEnabled = true;
			Menu = new MenuViewModel(this);
			SelectedZoneOPC = ZonesOPC != null ? ZonesOPC.FirstOrDefault() : null;
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID == Guid.Empty) return;

			var zoneViewModel = ZonesOPC.FirstOrDefault(x => x.UID == zoneUID);
			SelectedZoneOPC = zoneViewModel;
		}

		public void Initialize(IEnumerable<OPCZone> existingZones)
		{
			ZonesOPC = new ObservableCollection<OPCZone>(existingZones);
		}

		public RelayCommand AddCommand { get; set; }
		public RelayCommand DeleteCommand { get; set; }
		public RelayCommand EditCommand { get; set; }
		public RelayCommand SettingsCommand { get; set; }

		public void OnAdd()
		{
			var addZoneDialog = new AddZoneDialogViewModel(ZonesOPC);

			if (DialogService.ShowModalWindow(addZoneDialog))
			{
				var addingZones = addZoneDialog.Zones.Where(x => x.CanAdd).ToList();
				ZonesOPC.AddRange(addingZones);
				SKDManager.SKDConfiguration.OPCZones.AddRange(addingZones.Select(x => x.ToDTO()));
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation(string.Format(Resources.MessageRemoveOPCZoneContent, SelectedZoneOPC.Name))) return;

			ZonesOPC.Remove(SelectedZoneOPC);

			SelectedZoneOPC = ZonesOPC.FirstOrDefault();
			ServiceFactory.SaveService.SKDChanged = true;
		}

		public void OnEdit()
		{
			var propertiesViewModel = new PropertiesDialogViewModel(SelectedZoneOPC);
			DialogService.ShowModalWindow(propertiesViewModel);
		}

		public void OnSettings()
		{
			var settingsView = new SettingsViewModel(SKDManager.SKDConfiguration.OPCSettings.IsActive)
			{
				Settings = new OPCSettings(SKDManager.SKDConfiguration.OPCSettings)
			};

			if (DialogService.ShowModalWindow(settingsView))
			{
				SKDManager.SKDConfiguration.OPCSettings = settingsView.Settings.ToDTO();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		#region Private Methods
		private void SubscribeEvents()
		{
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}

		private void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementOPCZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}

		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementOPCZone(element);
				if (elementZone != null)
					OnZoneChanged(elementZone.ZoneUID);
			});
			_lockSelection = false;
		}

		private void OnZoneChanged(Guid zoneUID)
		{
			var zone = ZonesOPC.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				zone.UpdateVisualizationState();
				if (!_lockSelection)
				{
					SelectedZoneOPC = zone;
				}
			}
		}

		private static IElementZone GetElementOPCZone(ElementBase element)
		{
			return element as ElementRectangleOPCZone ?? (IElementZone)(element as ElementPolygonOPCZone);
		}
		#endregion
	}
}

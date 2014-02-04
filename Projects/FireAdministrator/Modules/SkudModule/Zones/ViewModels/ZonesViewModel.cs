using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using FiresecAPI;
using Infrustructure.Plans.Events;
using FiresecClient;
using Infrastructure;
using System.Windows.Input;
using System.IO;
using Infrastructure.Common;
using Microsoft.Win32;
using Infrastructure.Common.Windows;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Elements;
using Infrastructure.Common.Ribbon;
using System.Collections.ObjectModel;
using XFiresecAPI;
using Infrastructure.Events;

namespace SKDModule.ViewModels
{
	public class ZonesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static ZonesViewModel Current { get; private set; }
		public ZoneDevicesViewModel ZoneDevices { get; set; }
		private bool _lockSelection;

		public ZonesViewModel()
		{
			_lockSelection = false;
			Menu = new ZonesMenuViewModel(this);
			Current = this;
			ZoneDevices = new ZoneDevicesViewModel();
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			BuildTree();
			if (RootZone != null)
			{
				RootZone.IsExpanded = true;
				SelectedZone = RootZone;
				foreach (var child in RootZone.Children)
				{
					child.IsExpanded = true;
				}
			}

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}

			OnPropertyChanged("RootZones");
		}

		#region ZoneSelection
		public List<ZoneViewModel> AllZones;

		public void FillAllZones()
		{
			AllZones = new List<ZoneViewModel>();
			AddChildPlainZones(RootZone);
		}

		void AddChildPlainZones(ZoneViewModel parentViewModel)
		{
			AllZones.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainZones(childViewModel);
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				FillAllZones();
				var deviceViewModel = AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				SelectedZone = deviceViewModel;
			}
		}
		#endregion

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
				UpdateRibbonItems();
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedZone.Zone.PlanElementUIDs);

				if (value != null)
				{
					ZoneDevices.Initialize(value.Zone);
				}
				else
				{
					ZoneDevices.Clear();
				}
			}
		}

		ZoneViewModel _rootZone;
		public ZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public ZoneViewModel[] RootZones
		{
			get { return new ZoneViewModel[] { RootZone }; }
		}

		void BuildTree()
		{
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			FillAllZones();
		}

		public ZoneViewModel AddZone(SKDZone zone, ZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = AddZoneInternal(zone, parentZoneViewModel);
			FillAllZones();
			return zoneViewModel;
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

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (SelectedZone.AddCommand.CanExecute(null))
						SelectedZone.AddCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.M, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (SelectedZone.AddToParentCommand.CanExecute(null))
						SelectedZone.AddToParentCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (SelectedZone.RemoveCommand.CanExecute(null))
						SelectedZone.RemoveCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (SelectedZone.EditCommand.CanExecute(null))
						SelectedZone.EditCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Right, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (SelectedZone.HasChildren && !SelectedZone.IsExpanded)
						SelectedZone.IsExpanded = true;
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Left, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (SelectedZone.HasChildren && SelectedZone.IsExpanded)
						SelectedZone.IsExpanded = false;
				}
			});
		}

		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
		}

		private void SubscribeEvents()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void OnDeviceChanged(Guid zoneUID)
		{
			var zone = AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
			{
				zone.Update();
				// TODO: FIX IT
				if (!_lockSelection)
				{
					zone.ExpandToThis();
					SelectedZone = zone;
				}
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			//elements.OfType<ElementXDevice>().ToList().ForEach(element => Helper.ResetXDevice(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				//ElementXDevice elementDevice = element as ElementXDevice;
				//if (elementDevice != null)
				//    OnDeviceChanged(elementDevice.XDeviceUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			//ElementXDevice elementXDevice = element as ElementXDevice;
			//if (elementXDevice != null)
			//{
			//    _lockSelection = true;
			//    Select(elementXDevice.XDeviceUID);
			//    _lockSelection = false;
			//}
		}

		public override void OnShow()
		{
			base.OnShow();
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = SelectedZone == null ? null : SelectedZone.AddCommand;
			RibbonItems[0][1].Command = SelectedZone == null ? null : SelectedZone.EditCommand;
			RibbonItems[0][2].Command = SelectedZone == null ? null : SelectedZone.RemoveCommand;

			//RibbonItems[1][6][0].Command = SelectedDevice == null ? null : SelectedDevice.ReadCommand;
			//RibbonItems[1][6][1].Command = SelectedDevice == null ? null : SelectedDevice.WriteCommand;
			//RibbonItems[1][6][2].Command = SelectedDevice == null ? null : SelectedDevice.ReadAllCommand;
			//RibbonItems[1][6][3].Command = SelectedDevice == null ? null : SelectedDevice.WriteAllCommand;
			//RibbonItems[1][6][4].Command = SelectedDevice == null ? null : SelectedDevice.SyncFromDeviceToSystemCommand;
			//RibbonItems[1][6][5].Command = SelectedDevice == null ? null : SelectedDevice.SyncFromAllDeviceToSystemCommand;
			//RibbonItems[1][6][6].Command = SelectedDevice == null ? null : SelectedDevice.CopyParamCommand;
			//RibbonItems[1][6][7].Command = SelectedDevice == null ? null : SelectedDevice.PasteParamCommand;
			//RibbonItems[1][6][8].Command = SelectedDevice == null ? null : SelectedDevice.PasteAllParamCommand;
			//RibbonItems[1][6][9].Command = SelectedDevice == null ? null : SelectedDevice.PasteTemplateCommand;
			//RibbonItems[1][6][10].Command = SelectedDevice == null ? null : SelectedDevice.PasteAllTemplateCommand;
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}

		public void CreateZone(CreateSKDZoneEventArg createZoneEventArg)
		{
			// Временная заглушка - всегда возвращает отмену
			createZoneEventArg.Cancel = true;
			createZoneEventArg.ZoneUID = Guid.Empty;
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zoneViewModel != null)
				zoneViewModel.EditCommand.Execute();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using SKDModule.Plans.Designer;
using KeyboardKey = System.Windows.Input.Key;

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
				var zoneViewModel = AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
				if (zoneViewModel != null)
					zoneViewModel.ExpandToThis();
				SelectedZone = zoneViewModel;
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
		private void OnZoneChanged(Guid zoneUID)
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
			elements.OfType<ElementRectangleSKDZone>().ToList().ForEach(element => Helper.ResetSKDZone(element));
			elements.OfType<ElementPolygonSKDZone>().ToList().ForEach(element => Helper.ResetSKDZone(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementSKDZone(element);
				if (elementZone != null)
					OnZoneChanged(elementZone.ZoneUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementSKDZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementSKDZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleSKDZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonSKDZone;
			return elementZone;
		}

		public override void OnShow()
		{
			SelectedZone = SelectedZone;
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
			var zoneViewModel = createZoneEventArg.ParentZoneUID == Guid.Empty ? null : AllZones.FirstOrDefault(x => x.Zone.UID == createZoneEventArg.ParentZoneUID);
			createZoneEventArg.Zone = zoneViewModel != null ? zoneViewModel.AddChildZone() : null;
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zoneViewModel != null)
				zoneViewModel.EditCommand.Execute();
		}
	}
}
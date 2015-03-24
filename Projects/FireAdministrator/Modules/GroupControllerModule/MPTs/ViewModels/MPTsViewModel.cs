using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;
using GKModule.Events;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class MPTsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		bool _lockSelection;

		public MPTsViewModel()
		{
			Menu = new MPTsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			MPTs = new ObservableCollection<MPTViewModel>();
			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.OrderBy(x => x.No))
			{
				var mptViewModel = new MPTViewModel(mpt);
				MPTs.Add(mptViewModel);
			}
			SelectedMPT = MPTs.FirstOrDefault();
		}

		ObservableCollection<MPTViewModel> _mpts;
		public ObservableCollection<MPTViewModel> MPTs
		{
			get { return _mpts; }
			set
			{
				_mpts = value;
				OnPropertyChanged(() => MPTs);
			}
		}

		MPTViewModel _selectedMPT;
		public MPTViewModel SelectedMPT
		{
			get { return _selectedMPT; }
			set
			{
				_selectedMPT = value;
				if (value != null)
					value.Update();
				OnPropertyChanged(() => SelectedMPT);
				OnPropertyChanged(() => HasSelectedMPT);
				if (!_lockSelection && _selectedMPT != null && _selectedMPT.MPT.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedMPT.MPT.PlanElementUIDs);
			}
		}

		public bool HasSelectedMPT
		{
			get { return SelectedMPT != null; }
		}

		bool CanEditDelete()
		{
			return SelectedMPT != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}

		MPTDetailsViewModel OnAddResult()
		{
			var mptDetailsViewModel = new MPTDetailsViewModel();
			if (DialogService.ShowModalWindow(mptDetailsViewModel))
			{
				GKManager.DeviceConfiguration.MPTs.Add(mptDetailsViewModel.MPT);
				var mptViewModel = new MPTViewModel(mptDetailsViewModel.MPT);
				MPTs.Add(mptViewModel);
				SelectedMPT = mptViewModel;
				OnPropertyChanged(() => HasSelectedMPT);
				ServiceFactory.SaveService.GKChanged = true;
			}
			return mptDetailsViewModel;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить МПТ " + SelectedMPT.MPT.Name))
			{
				foreach (var mptDevice in SelectedMPT.MPT.MPTDevices)
				{
					MPTViewModel.ChangeIsInMPT(mptDevice.Device, false);
				}
				var index = MPTs.IndexOf(SelectedMPT);
				GKManager.DeviceConfiguration.MPTs.Remove(SelectedMPT.MPT);
				MPTs.Remove(SelectedMPT);
				index = Math.Min(index, MPTs.Count - 1);
				if (index > -1)
					SelectedMPT = MPTs[index];
				OnPropertyChanged(() => HasSelectedMPT);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedMPT.MPT);
		}

		void OnEdit(GKMPT mpt)
		{
			var mptDetailsViewModel = new MPTDetailsViewModel(mpt);
			if (DialogService.ShowModalWindow(mptDetailsViewModel))
			{
				SelectedMPT.MPT = mptDetailsViewModel.MPT;
				SelectedMPT.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void CreateMPT(CreateGKMPTEventArg createMPTEventArg)
		{
			MPTDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createMPTEventArg.Cancel = true;
				createMPTEventArg.MPTUID = Guid.Empty;
			}
			else
			{
				createMPTEventArg.Cancel = false;
				createMPTEventArg.MPTUID = result.MPT.UID;
				createMPTEventArg.MPT = result.MPT;
			}
		}
		public void EditMPT(Guid mptUID)
		{
			var mptViewModel = mptUID == Guid.Empty ? null : MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
			if (mptViewModel != null)
				OnEdit(mptViewModel.MPT);
		}

		public void Select(Guid mptUID)
		{
			if (mptUID != Guid.Empty)
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedMPT = SelectedMPT;
		}

		public override void OnHide()
		{
			base.OnHide();
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
		}

		void SubscribeEvents()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void OnMPTChanged(Guid mptUID)
		{
			var mpt = MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
			if (mpt != null)
			{
				mpt.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedMPT = mpt;
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementMPT = GetElementMPT(element);
				if (elementMPT != null)
				{
					OnMPTChanged(elementMPT.MPTUID);
				}
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementMPT = GetElementMPT(element);
			if (elementMPT != null)
			{
				_lockSelection = true;
				Select(elementMPT.MPTUID);
				_lockSelection = false;
			}
		}
		private IElementMPT GetElementMPT(ElementBase element)
		{
			IElementMPT elementMPT = element as ElementRectangleGKMPT;
			if (elementMPT == null)
				elementMPT = element as ElementPolygonGKMPT;
			return elementMPT;
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}
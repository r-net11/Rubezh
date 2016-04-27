using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using Infrastructure.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class MPTsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;

		public MPTsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			Menu = new MPTsMenuViewModel(this);
			IsRightPanelEnabled = true;
			RegisterShortcuts();
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
				GKManager.AddMPT(mptDetailsViewModel.MPT);
				var mptViewModel = new MPTViewModel(mptDetailsViewModel.MPT);
				MPTs.Add(mptViewModel);
				SelectedMPT = mptViewModel;
				OnPropertyChanged(() => HasSelectedMPT);
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKMPT>();
			}
			return mptDetailsViewModel;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedMPT);
		}

		void OnEdit(MPTViewModel mptViewModel)
		{
			var mptDetailsViewModel = new MPTDetailsViewModel(mptViewModel.MPT);
			if (DialogService.ShowModalWindow(mptDetailsViewModel))
			{
				mptViewModel.Update();
				GKManager.EditMPT(mptViewModel.MPT);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить МПТ " + SelectedMPT.MPT.PresentationName + " ?"))
			{
				foreach (var mptDevice in SelectedMPT.MPT.MPTDevices)
				{
					MPTViewModel.ChangeIsInMPT(mptDevice.Device, false);
				}
				var index = MPTs.IndexOf(SelectedMPT);
				GKManager.RemoveMPT(SelectedMPT.MPT);
				MPTs.Remove(SelectedMPT);
				index = Math.Min(index, MPTs.Count - 1);
				if (index > -1)
					SelectedMPT = MPTs[index];
				OnPropertyChanged(() => HasSelectedMPT);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые МПТ ?"))
			{
				GetEmptyMPTs().ForEach(x =>
					{
						GKManager.RemoveMPT(x.MPT);
						MPTs.Remove(x);
					});
				SelectedMPT = MPTs.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GetEmptyMPTs().Any();
		}
		List<MPTViewModel> GetEmptyMPTs()
		{
			return MPTs.Where(x => !x.MPT.MPTDevices.Any() && !x.MPT.MptLogic.GetObjects().Any()).ToList();
		}
		public RelayCommand CopyLogicCommand { get; private set; }
		void OnCopyLogic()
		{
			GKManager.CopyLogic(SelectedMPT.MPT.MptLogic, true, false, true, false, true);
		}

		bool CanCopyLogic()
		{
			return SelectedMPT != null;
		}

		public RelayCommand PasteLogicCommand { get; private set; }
		void OnPasteLogic()
		{
			var result = GKManager.CompareLogic(new GKAdvancedLogic(true, false, true, false, true));
			var messageBoxResult = true;
			if (!String.IsNullOrEmpty(result))
				messageBoxResult = MessageBoxService.ShowConfirmation(result, "Копировать логику?");
			if (messageBoxResult)
			{
				SelectedMPT.MPT.MptLogic = GKManager.PasteLogic(new GKAdvancedLogic(true, false, true, false, true));
				SelectedMPT.Update();
				SelectedMPT.MPT.Invalidate(GKManager.DeviceConfiguration);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanPasteLogic()
		{
			return SelectedMPT != null && GKManager.LogicToCopy != null;
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedMPT != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedMPT.MPT.OutputDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
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
				OnEdit(mptViewModel);
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

		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
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

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Удалить все пустые МПТ", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
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
	}
}
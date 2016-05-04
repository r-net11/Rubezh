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
	public class DirectionsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static DirectionsViewModel Current { get; private set; }
		bool _lockSelection = false;

		public DirectionsViewModel()
		{
			Current = this;
			Menu = new DirectionsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SetRibbonItems();

			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Subscribe(CreateDirection);
			ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Subscribe(EditDirection);
		}
		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}
		protected override bool IsRightPanelVisibleByDefault
		{
			get { return true; }
		}

		public void Initialize()
		{
			Directions = new ObservableCollection<DirectionViewModel>(
				from direction in GKManager.Directions
				orderby direction.No
				select new DirectionViewModel(direction));
			SelectedDirection = Directions.FirstOrDefault();
		}

		ObservableCollection<DirectionViewModel> _directions;
		public ObservableCollection<DirectionViewModel> Directions
		{
			get { return _directions; }
			set
			{
				_directions = value;
				OnPropertyChanged(() => Directions);
			}
		}

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				OnPropertyChanged(() => SelectedDirection);
				if (!_lockSelection && _selectedDirection != null && _selectedDirection.Direction.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDirection.Direction.PlanElementUIDs);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		private DirectionDetailsViewModel OnAddResult()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel();
			if (ServiceFactory.DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				GKManager.AddDirection(directionDetailsViewModel.Direction);
				var directionViewModel = new DirectionViewModel(directionDetailsViewModel.Direction);
				Directions.Add(directionViewModel);
				SelectedDirection = directionViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKDirection>();
				return directionDetailsViewModel;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDirection);
		}
		void OnEdit(DirectionViewModel directionViewModel)
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(directionViewModel.Direction);
			if (ServiceFactory.DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				directionViewModel.Update();
				GKManager.EditDirection(directionViewModel.Direction);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить направление " + SelectedDirection.Direction.PresentationName + " ?"))
			{
				var index = Directions.IndexOf(SelectedDirection);
				GKManager.RemoveDirection(SelectedDirection.Direction);
				Directions.Remove(SelectedDirection);
				index = Math.Min(index, Directions.Count - 1);
				if (index > -1)
					SelectedDirection = Directions[index];
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKDirection>();
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые направления ?"))
			{
				GetEmptyDirections().ForEach(x =>
				{
					GKManager.RemoveDirection(x.Direction);
					Directions.Remove(x);
				});

				SelectedDirection = Directions.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GetEmptyDirections().Any();
		}
		List<DirectionViewModel> GetEmptyDirections()
		{
			return Directions.Where(x => !x.Direction.Logic.GetObjects().Any()).ToList();
		}

		GKDirection _directionToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_directionToCopy = SelectedDirection.Direction.Clone();
			var logicViewModel = new LogicViewModel(SelectedDirection.Direction, SelectedDirection.Direction.Logic, true);
			_directionToCopy.Logic = logicViewModel.GetModel();
		}

		bool CanCopy()
		{
			return SelectedDirection != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			_directionToCopy.UID = Guid.NewGuid();
			var directionViewModel = new DirectionViewModel(_directionToCopy.Clone());
			var logicViewModel = new LogicViewModel(SelectedDirection.Direction, _directionToCopy.Logic, true);
			directionViewModel.Direction.Logic = logicViewModel.GetModel();
			directionViewModel.Direction.No = (ushort)(GKManager.Directions.Select(x => x.No).Max() + 1);
			directionViewModel.Direction.Invalidate(GKManager.DeviceConfiguration);
			GKManager.AddDirection(directionViewModel.Direction);
			Directions.Add(directionViewModel);
			SelectedDirection = directionViewModel;
			ServiceFactory.SaveService.GKChanged = true;
		}

		bool CanPaste()
		{
			return _directionToCopy != null && SelectedDirection != null;
		}

		public RelayCommand CopyLogicCommand { get; private set; }
		void OnCopyLogic()
		{
			GKManager.CopyLogic(SelectedDirection.Direction.Logic, true, false, true, false, true);
		}

		bool CanCopyLogic()
		{
			return SelectedDirection != null;
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
				SelectedDirection.Direction.Logic = GKManager.PasteLogic(new GKAdvancedLogic(true, false, true, false, true));
				SelectedDirection.Direction.Invalidate(GKManager.DeviceConfiguration);
				SelectedDirection.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanPasteLogic()
		{
			return SelectedDirection != null && GKManager.LogicToCopy != null;
		}

		bool CanEditDelete()
		{
			return SelectedDirection != null;
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedDirection != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedDirection.Direction.OutputDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		public void CreateDirection(CreateGKDirectionEventArg createDirectionEventArg)
		{
			DirectionDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createDirectionEventArg.Cancel = true;
			}
			else
			{
				createDirectionEventArg.Cancel = false;
				createDirectionEventArg.Direction = result.Direction;
			}
		}
		public void EditDirection(Guid directionUID)
		{
			var directionViewModel = directionUID == Guid.Empty ? null : Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (directionViewModel != null)
				OnEdit(directionViewModel);
		}

		private void OnElementSelected(ElementBase element)
		{
			var elementDirection = GetElementDirection(element);
			if (elementDirection != null)
			{
				_lockSelection = true;
				Select(elementDirection.DirectionUID);
				_lockSelection = false;
			}
		}
		private IElementDirection GetElementDirection(ElementBase element)
		{
			IElementDirection elementDirection = element as ElementRectangleGKDirection;
			if (elementDirection == null)
				elementDirection = element as ElementPolygonGKDirection;
			return elementDirection;
		}
		public override void OnShow()
		{
			base.OnShow();
			SelectedDirection = SelectedDirection;
		}

		#region ISelectable<Guid> Members
		public void Select(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
		}
		#endregion

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Удалить все пустые направления", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}
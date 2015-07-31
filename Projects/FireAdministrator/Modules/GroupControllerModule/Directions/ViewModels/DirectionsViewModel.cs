using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class DirectionsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static DirectionsViewModel Current { get; private set; }
		bool _lockSelection;

		public DirectionsViewModel()
		{
			Current = this;
			Menu = new DirectionsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			CopyLogicCommand = new RelayCommand(OnCopyLogic, CanCopyLogic);
			PasteLogicCommand = new RelayCommand(OnPasteLogic, CanPasteLogic);

			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
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
				if (value != null)
					value.Update();
				OnPropertyChanged(() => SelectedDirection);
				if (!_lockSelection && _selectedDirection != null && _selectedDirection.Direction.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDirection.Direction.PlanElementUIDs);
			}
		}

		GKDirection _directionToCopy;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_directionToCopy = Utils.Clone(SelectedDirection.Direction);
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
			var directionViewModel = new DirectionViewModel(Utils.Clone(_directionToCopy));
			var logicViewModel = new LogicViewModel(SelectedDirection.Direction, _directionToCopy.Logic, true);
			directionViewModel.Direction.Logic = logicViewModel.GetModel();
			directionViewModel.Direction.No = (ushort)(GKManager.Directions.Select(x => x.No).Max() + 1);
			GKManager.Directions.Add(directionViewModel.Direction);
			Directions.Add(directionViewModel);
			SelectedDirection = directionViewModel;
			ServiceFactory.SaveService.GKChanged = true;
		}

		bool CanPaste()
		{
			return _directionToCopy != null;
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
				SelectedDirection.Update();
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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		private DirectionDetailsViewModel OnAddResult()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel();
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
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
		
		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить направление " + SelectedDirection.Direction.PresentationName))
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
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые направления ?"))
			{
				var emptyDirections = Directions.Where(x => !x.Direction.Logic.GetObjects().Any());
				foreach (var emptyDirection in emptyDirections)
				{
					GKManager.RemoveDirection(emptyDirection.Direction);
					Directions.Remove(emptyDirection);
				}
				SelectedDirection = Directions.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return Directions.Any(x => !x.Direction.Logic.GetObjects().Any());
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDirection.Direction);
		}
		void OnEdit(GKDirection direction)
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(direction);
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				SelectedDirection.Direction = directionDetailsViewModel.Direction;
				SelectedDirection.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void CreateDirection(CreateGKDirectionEventArg createDirectionEventArg)
		{
			DirectionDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createDirectionEventArg.Cancel = true;
				createDirectionEventArg.DirectionUID = Guid.Empty;
			}
			else
			{
				createDirectionEventArg.Cancel = false;
				createDirectionEventArg.DirectionUID = result.Direction.UID;
				createDirectionEventArg.Direction = result.Direction;
			}
		}
		public void EditDirection(Guid directionUID)
		{
			var directionViewModel = directionUID == Guid.Empty ? null : Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (directionViewModel != null)
				OnEdit(directionViewModel.Direction);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedDirection = SelectedDirection;
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		#region ISelectable<Guid> Members
		public void Select(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
		}
		#endregion

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(System.Windows.Input.Key.V, ModifierKeys.Control), PasteCommand);
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
		private void OnDirectionChanged(Guid directionUID)
		{
			var direction = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (direction != null)
			{
				direction.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedDirection = direction;
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementDirection = GetElementXDirection(element);
				if (elementDirection != null)
				{
					OnDirectionChanged(elementDirection.DirectionUID);
				}
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementDirection = GetElementXDirection(element);
			if (elementDirection != null)
			{
				_lockSelection = true;
				Select(elementDirection.DirectionUID);
				_lockSelection = false;
			}
		}
		private IElementDirection GetElementXDirection(ElementBase element)
		{
			IElementDirection elementDirection = element as ElementRectangleGKDirection;
			if (elementDirection == null)
				elementDirection = element as ElementPolygonGKDirection;
			return elementDirection;
		}

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
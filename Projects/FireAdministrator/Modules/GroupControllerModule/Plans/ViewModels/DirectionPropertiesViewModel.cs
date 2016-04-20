using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DirectionPropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementDirection _element;
		DirectionsViewModel _directionsViewModel;

		public DirectionPropertiesViewModel(IElementDirection element, DirectionsViewModel directionsViewModel)
		{
			_directionsViewModel = directionsViewModel;
			_element = element;
			Title = "Свойства фигуры: Направление";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Directions = new ObservableCollection<DirectionViewModel>();
			foreach (var direction in GKManager.Directions)
			{
				var directionViewModel = new DirectionViewModel(direction);
				Directions.Add(directionViewModel);
			}
			if (_element.DirectionUID != Guid.Empty)
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == _element.DirectionUID);

			ShowState = element.ShowState;
			ShowDelay = element.ShowDelay;
		}

		public ObservableCollection<DirectionViewModel> Directions { get; private set; }

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				OnPropertyChanged(() => SelectedDirection);
			}
		}

		bool _showState;
		public bool ShowState
		{
			get { return _showState; }
			set
			{
				_showState = value;
				OnPropertyChanged(() => ShowState);
			}
		}

		bool _showDelay;
		public bool ShowDelay
		{
			get { return _showDelay; }
			set
			{
				_showDelay = value;
				OnPropertyChanged(() => ShowDelay);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid directionUID = _element.DirectionUID;
			var createDirectionEventArg = new CreateGKDirectionEventArg();
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Publish(createDirectionEventArg);
			if (createDirectionEventArg.Direction != null)
				_element.DirectionUID = createDirectionEventArg.Direction.UID;
			GKPlanExtension.Instance.Cache.BuildSafe<GKDirection>();
			GKPlanExtension.Instance.SetItem<GKDirection>(_element);
			UpdateDirections(directionUID);
			if (!createDirectionEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Publish(SelectedDirection.Direction.UID);
			SelectedDirection.Update();
		}
		bool CanEdit()
		{
			return SelectedDirection != null;
		}

		protected override bool Save()
		{
			_element.ShowState = ShowState;
			_element.ShowDelay = ShowDelay;
			Guid directionUID = _element.DirectionUID;
			GKPlanExtension.Instance.SetItem<GKDirection>(_element, SelectedDirection == null ? null : SelectedDirection.Direction);
			UpdateDirections(directionUID);
			return base.Save();
		}
		void UpdateDirections(Guid directionUID)
		{
			if (_directionsViewModel != null)
			{
				if (directionUID != _element.DirectionUID)
					Update(directionUID);
				Update(_element.DirectionUID);
				_directionsViewModel.LockedSelect(_element.DirectionUID);
			}
		}
		void Update(Guid directionUID)
		{
			var direction = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (direction != null)
				direction.Update();
		}
	}
}
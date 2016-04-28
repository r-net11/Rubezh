using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DirectionPropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementDirection IElementDirection;

		public DirectionPropertiesViewModel(IElementDirection element)
		{
			IElementDirection = element;
			Title = "Свойства фигуры: Направление";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Directions = new ObservableCollection<DirectionViewModel>();
			foreach (var direction in GKManager.Directions)
			{
				var directionViewModel = new DirectionViewModel(direction);
				Directions.Add(directionViewModel);
			}
			if (IElementDirection.DirectionUID != Guid.Empty)
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == IElementDirection.DirectionUID);

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
			var createDirectionEventArg = new CreateGKDirectionEventArg();
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Publish(createDirectionEventArg);
			if (createDirectionEventArg.Direction != null)
				IElementDirection.DirectionUID = createDirectionEventArg.Direction.UID;
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
			IElementDirection.ShowState = ShowState;
			IElementDirection.ShowDelay = ShowDelay;
			GKPlanExtension.Instance.RewriteItem(IElementDirection, SelectedDirection.Direction);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedDirection != null;
		}
	}
}
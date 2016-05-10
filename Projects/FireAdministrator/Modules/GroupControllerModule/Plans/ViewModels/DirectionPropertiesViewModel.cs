using GKModule.Events;
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
		const int _sensivityFactor = 100;
		IElementDirection IElementDirection;
		ElementBaseRectangle ElementBaseRectangle { get; set; }
		public bool CanEditPosition { get; private set; }
		public DirectionPropertiesViewModel(IElementDirection element)
		{
			IElementDirection = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			CanEditPosition = ElementBaseRectangle != null;
			if (CanEditPosition)
			{
				Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
				Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);
			}
			Title = "Свойства фигуры: Направление";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Directions = new ObservableCollection<GKDirection>(GKManager.Directions);
			if (IElementDirection.DirectionUID != Guid.Empty)
				SelectedDirection = Directions.FirstOrDefault(x => x.UID == IElementDirection.DirectionUID);

			ShowState = element.ShowState;
			ShowDelay = element.ShowDelay;
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		public ObservableCollection<GKDirection> Directions { get; private set; }

		GKDirection _selectedDirection;
		public GKDirection SelectedDirection
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
			ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Publish(SelectedDirection.UID);
			Directions = new ObservableCollection<GKDirection>(GKManager.Directions);
			OnPropertyChanged(() => Directions);
		}
		bool CanEdit()
		{
			return SelectedDirection != null;
		}
		protected override bool Save()
		{
			IElementDirection.ShowState = ShowState;
			IElementDirection.ShowDelay = ShowDelay;
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(IElementDirection, SelectedDirection);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedDirection != null;
		}
	}
}
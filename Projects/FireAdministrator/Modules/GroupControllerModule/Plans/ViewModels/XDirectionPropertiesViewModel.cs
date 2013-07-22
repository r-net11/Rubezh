using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Plans.Designer;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.ViewModels
{
	public class XDirectionPropertiesViewModel : SaveCancelDialogViewModel
	{
		private IElementDirection _element;
		private DirectionsViewModel _directionsViewModel;

		public XDirectionPropertiesViewModel(IElementDirection element, DirectionsViewModel directionsViewModel)
		{
			_directionsViewModel = directionsViewModel;
			_element = element;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: ГК Направление";
			var directions = XManager.DeviceConfiguration.Directions;
			XDirections = new ObservableCollection<DirectionViewModel>();
			foreach (var direction in directions)
			{
				var directionViewModel = new DirectionViewModel(direction);
				XDirections.Add(directionViewModel);
			}
			if (_element.DirectionUID != Guid.Empty)
				SelectedXDirection = XDirections.FirstOrDefault(x => x.Direction.UID == _element.DirectionUID);
		}

		public ObservableCollection<DirectionViewModel> XDirections { get; private set; }

		private DirectionViewModel _selectedXDirection;
		public DirectionViewModel SelectedXDirection
		{
			get { return _selectedXDirection; }
			set
			{
				_selectedXDirection = value;
				OnPropertyChanged("SelectedXDirection");
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid xdirectionUID = _element.DirectionUID;
			var createDirectionEventArg = new CreateXDirectionEventArg();
			ServiceFactory.Events.GetEvent<CreateXDirectionEvent>().Publish(createDirectionEventArg);
			if (createDirectionEventArg.XDirection != null)
				_element.DirectionUID = createDirectionEventArg.XDirection.UID;
			Helper.BuildMap();
			Helper.SetXDirection(_element);
			UpdateDirections(xdirectionUID);
			if (!createDirectionEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditXDirectionEvent>().Publish(SelectedXDirection.Direction.UID);
			SelectedXDirection.Update(SelectedXDirection.Direction);
		}
		private bool CanEdit()
		{
			return SelectedXDirection != null;
		}

		protected override bool Save()
		{
			Guid directionUID = _element.DirectionUID;
			Helper.SetXDirection(_element, SelectedXDirection == null ? null : SelectedXDirection.Direction);
			UpdateDirections(directionUID);
			return base.Save();
		}
		private void UpdateDirections(Guid directionUID)
		{
			if (_directionsViewModel != null)
			{
				if (directionUID != _element.DirectionUID)
					Update(directionUID);
				Update(_element.DirectionUID);
				_directionsViewModel.LockedSelect(_element.DirectionUID);
			}
		}
		private void Update(Guid directionUID)
		{
			var direction = _directionsViewModel.Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (direction != null)
				direction.Update();
		}
	}
}
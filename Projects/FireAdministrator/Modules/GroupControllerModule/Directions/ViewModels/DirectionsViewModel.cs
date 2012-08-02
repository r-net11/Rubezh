using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class DirectionsViewModel : ViewPartViewModel, IEditingViewModel
    {
		public DirectionZonesViewModel DirectionZones { get; private set; }
        public static DirectionsViewModel Current { get; private set; }

		public DirectionsViewModel()
        {
            Current = this;
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DirectionZones = new DirectionZonesViewModel();
        }

        public void Initialize()
        {
			Directions = new ObservableCollection<DirectionViewModel>(
				from direction in XManager.DeviceConfiguration.Directions
                orderby direction.No
				select new DirectionViewModel(direction));
			SelectedDirection = Directions.FirstOrDefault();
        }

		ObservableCollection<DirectionViewModel> _direction;
		public ObservableCollection<DirectionViewModel> Directions
        {
            get { return _direction; }
            set
            {
                _direction = value;
				OnPropertyChanged("Directions");
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
					DirectionZones.Initialize(value.XDirection);
                else
					DirectionZones.Clear();

				OnPropertyChanged("SelectedDirection");
            }
        }

        bool CanEditDelete()
        {
            return SelectedDirection != null;
        }

        bool CanDeleteAll()
        {
			return Directions.Count > 0;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
			var directionDetailsViewModel = new DirectionDetailsViewModel();
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
            {
				XManager.DeviceConfiguration.Directions.Add(directionDetailsViewModel.XDirection);
				Directions.Add(new DirectionViewModel(directionDetailsViewModel.XDirection));

                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedDirection.XDirection.PresentationName);
            if (dialogResult == MessageBoxResult.Yes)
            {
				XManager.DeviceConfiguration.Directions.Remove(SelectedDirection.XDirection);
				Directions.Remove(SelectedDirection);
				SelectedDirection = Directions.FirstOrDefault();
				DirectionZones.UpdateAvailableZones();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
			var directionDetailsViewModel = new DirectionDetailsViewModel(SelectedDirection.XDirection);
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
            {
				SelectedDirection.XDirection = directionDetailsViewModel.XDirection;
                SelectedDirection.Update();

                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public override void OnShow()
        {
            SelectedDirection = SelectedDirection;
			ServiceFactory.Layout.ShowMenu(new DirectionsMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
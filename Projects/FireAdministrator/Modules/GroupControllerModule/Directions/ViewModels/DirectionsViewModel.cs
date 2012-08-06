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
        public static DirectionsViewModel Current { get; private set; }

		public DirectionsViewModel()
        {
            Current = this;
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
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
				OnPropertyChanged("SelectedDirection");
            }
        }

        bool CanEditDelete()
        {
            return SelectedDirection != null;
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
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить направление " + SelectedDirection.Direction.PresentationName);
            if (dialogResult == MessageBoxResult.Yes)
            {
				XManager.DeviceConfiguration.Directions.Remove(SelectedDirection.Direction);
				Directions.Remove(SelectedDirection);
				SelectedDirection = Directions.FirstOrDefault();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
			var directionDetailsViewModel = new DirectionDetailsViewModel(SelectedDirection.Direction);
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
            {
				SelectedDirection.Direction = directionDetailsViewModel.XDirection;
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
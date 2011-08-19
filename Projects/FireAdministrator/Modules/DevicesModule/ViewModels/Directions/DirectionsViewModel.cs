using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DirectionsViewModel : RegionViewModel
    {
        public DirectionsViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            AddCommand = new RelayCommand(OnAdd);
        }

        public void Initialize()
        {
            Directions = new ObservableCollection<DirectionViewModel>(
                from direction in FiresecManager.DeviceConfiguration.Directions
                select new DirectionViewModel(direction));

            if (Directions.Count > 0)
                SelectedDirection = Directions[0];
        }

        ObservableCollection<DirectionViewModel> _directions;
        public ObservableCollection<DirectionViewModel> Directions
        {
            get { return _directions; }
            set
            {
                _directions = value;
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

        bool CanDelete(object obj)
        {
            return (SelectedDirection != null);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (CanDelete(null))
            {
                FiresecManager.DeviceConfiguration.Directions.Remove(SelectedDirection.Direction);
                Directions.Remove(SelectedDirection);
                DevicesModule.HasChanges = true;
            }
        }

        bool CanEdit(object obj)
        {
            return (SelectedDirection != null);
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (CanEdit(null))
            {
                var directionDetailsViewModel = new DirectionDetailsViewModel();
                directionDetailsViewModel.Initialize(SelectedDirection.Direction);
                var result = ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel);
                if (result)
                {
                    SelectedDirection.Update();
                    DevicesModule.HasChanges = true;
                }
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var directionDetailsViewModel = new DirectionDetailsViewModel();
            directionDetailsViewModel.Initialize();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel);
            if (result)
            {
                FiresecManager.DeviceConfiguration.Directions.Add(directionDetailsViewModel.Direction);
                var directionViewModel = new DirectionViewModel(directionDetailsViewModel.Direction);
                Directions.Add(directionViewModel);
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var directionsMenuViewModel = new DirectionsMenuViewModel(AddCommand, DeleteCommand, EditCommand);
            ServiceFactory.Layout.ShowMenu(directionsMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}

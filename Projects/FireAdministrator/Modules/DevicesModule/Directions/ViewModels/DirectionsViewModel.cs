using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public class DirectionsViewModel : ViewPartViewModel, IEditingViewModel
    {
        public DirectionsViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditOrDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditOrDelete);
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

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.DeviceConfiguration.Directions.Remove(SelectedDirection.Direction);
            Directions.Remove(SelectedDirection);

            ServiceFactory.SaveService.DevicesChanged = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var directionDetailsViewModel = new DirectionDetailsViewModel(SelectedDirection.Direction);
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
            {
                SelectedDirection.Update();

                ServiceFactory.SaveService.DevicesChanged = true;
            }
        }

        bool CanEditOrDelete()
        {
            return SelectedDirection != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var directionDetailsViewModel = new DirectionDetailsViewModel();
            if (DialogService.ShowModalWindow(directionDetailsViewModel))
            {
                FiresecManager.DeviceConfiguration.Directions.Add(directionDetailsViewModel.Direction);
                Directions.Add(new DirectionViewModel(directionDetailsViewModel.Direction));

                ServiceFactory.SaveService.DevicesChanged = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new DirectionsMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
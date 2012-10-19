using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
    public class DirectionsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
    {
        public DirectionsViewModel()
        {
            Menu = new DirectionsMenuViewModel(this);
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            ChangeZonesCommand = new RelayCommand(OnChangeZones, CanEditDelete);
            ChangeDevicesCommand = new RelayCommand(OnChangeDevices, CanEditDelete);
            RegisterShortcuts();
        }

        public void Initialize()
        {
            Directions = new ObservableCollection<DirectionViewModel>(
                from direction in XManager.DeviceConfiguration.Directions
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
					value.Update();
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
                XManager.AddDirection(directionDetailsViewModel.XDirection);
                var directionViewModel = new DirectionViewModel(directionDetailsViewModel.XDirection);
                Directions.Add(directionViewModel);
                SelectedDirection = directionViewModel;
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить направление " + SelectedDirection.Direction.PresentationName);
            if (dialogResult == MessageBoxResult.Yes)
            {
                XManager.RemoveDirection(SelectedDirection.Direction);
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

        public RelayCommand ChangeZonesCommand { get; private set; }
        void OnChangeZones()
        {
            SelectedDirection.ChangeZones();
        }

        public RelayCommand ChangeDevicesCommand { get; private set; }
        void OnChangeDevices()
        {
            SelectedDirection.ChangeDevices();
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

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }
    }
}
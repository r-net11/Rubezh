using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using System;

namespace DevicesModule.ViewModels
{
    public class DirectionDetailsViewModel : SaveCancelDialogContent
    {
        public DirectionDetailsViewModel()
        {
            ResetRmCommand = new RelayCommand(OnResetRm);
            ResetButtonCommand = new RelayCommand(OnResetButton);
            ChooseRmCommand = new RelayCommand(OnChooseRm);
            ChooseButtonCommand = new RelayCommand(OnChooseButton);
        }

        bool _isNew;
        public Direction Direction { get; private set; }

        public void Initialize()
        {
            _isNew = true;
            Title = "Создать направление";

            Direction = new Direction();
            Direction.Name = "Новое направление";
            if (FiresecManager.DeviceConfiguration.Directions.Count > 0)
            {
                int maxId = FiresecManager.DeviceConfiguration.Directions.Max(x => x.Id);
                Id = maxId + 1;
            }
            else
            {
                Id = 0;
            }
        }

        public void Initialize(Direction direction)
        {
            _isNew = false;
            Title = "Редактировать направление";

            Direction = direction;

            Id = direction.Id;
            Name = direction.Name;
            Description = direction.Description;

            if (direction.DeviceRm != null)
            {
                DeviceRm = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == direction.DeviceRm);
            }
            if (direction.DeviceButton != null)
            {
                DeviceButton = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == direction.DeviceButton);
            }
        }

        int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        Device _deviceRm;
        public Device DeviceRm
        {
            get { return _deviceRm; }
            set
            {
                _deviceRm = value;
                OnPropertyChanged("DeviceRm");
            }
        }

        Device _deviceButton;
        public Device DeviceButton
        {
            get { return _deviceButton; }
            set
            {
                _deviceButton = value;
                OnPropertyChanged("DeviceButton");
            }
        }

        void SaveModel()
        {
            Direction.Id = Id;
            Direction.Name = Name;
            Direction.Description = Description;

            Direction.DeviceRm = Guid.Empty;
            Direction.DeviceButton = Guid.Empty;
            if (DeviceRm != null)
            {
                Direction.DeviceRm = DeviceRm.UID;
            }
            if (DeviceButton != null)
            {
                Direction.DeviceButton = DeviceButton.UID;
            }
        }

        public RelayCommand ResetRmCommand { get; private set; }
        void OnResetRm()
        {
            DeviceRm = null;
        }

        public RelayCommand ResetButtonCommand { get; private set; }
        void OnResetButton()
        {
            DeviceButton = null;
        }

        public RelayCommand ChooseRmCommand { get; private set; }
        void OnChooseRm()
        {
            var directionDeviceSelectorViewModel = new DirectionDeviceSelectorViewModel();
            directionDeviceSelectorViewModel.Initialize(Direction, true);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(directionDeviceSelectorViewModel);
            if (result)
            {
                DeviceRm = directionDeviceSelectorViewModel.SelectedDevice.Device;
            }
        }

        public RelayCommand ChooseButtonCommand { get; private set; }
        void OnChooseButton()
        {
            var directionDeviceSelectorViewModel = new DirectionDeviceSelectorViewModel();
            directionDeviceSelectorViewModel.Initialize(Direction, false);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(directionDeviceSelectorViewModel);
            if (result)
            {
                DeviceButton = directionDeviceSelectorViewModel.SelectedDevice.Device;
            }
        }

        protected override void Save()
        {
            if (_isNew)
            {
                if (FiresecManager.DeviceConfiguration.Directions.Any(x => x.Id == Id))
                {
                    Close(false);
                    return;
                }
                SaveModel();
            }
            else
            {
                if (Id != Direction.Id && FiresecManager.DeviceConfiguration.Directions.Any(x => x.Id == Id))
                {
                    Close(false);
                    return;
                }
                SaveModel();
            }
        }
    }
}
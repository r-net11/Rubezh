using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using VideoModule.Views;
using Infrastructure;

namespace VideoModule.ViewModels
{
    public class CameraDetailsViewModel : SaveCancelDialogContent
    {
        public Camera Camera { get; private set; }

        public CameraDetailsViewModel(Camera camera = null)
        {
            SelectZoneCommand = new RelayCommand(OnSelectZone);
            ClearZoneCommand = new RelayCommand(OnClearZone);
            TestCommand = new RelayCommand(OnTest);

            if (camera != null)
            {
                Title = "Редактировать камеру";
                Camera = camera;
                if (camera.ZoneNo.HasValue)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == camera.ZoneNo);
                    if (zone != null)
                        ZoneName = zone.PresentationName;
                }
            }
            else
            {
                Title = "Создать камеру";
                Camera = new Camera()
                {
                    Name = "Новая камера",
                    Address = "192.168.0.1"
                };
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Name = Camera.Name;
            Address = Camera.Address;
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

        string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }

        string _zoneName;
        public string ZoneName
        {
            get { return _zoneName; }
            set
            {
                _zoneName = value;
                OnPropertyChanged("ZoneName");
            }
        }

        public RelayCommand SelectZoneCommand { get; private set; }
        void OnSelectZone()
        {
            var zoneSelectationViewModel = new ZoneSelectationViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneSelectationViewModel))
            {

            }
        }

        public RelayCommand ClearZoneCommand { get; private set; }
        void OnClearZone()
        {
            Camera.ZoneNo = null;
            ZoneName = null;
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
        {
            var videoWindow = new VideoWindow()
            {
                Title = "Видео с камеры " + Address,
                Address = Address //"172.16.7.202"
            };
            videoWindow.ShowDialog();
        }

        protected override void Save(ref bool cancel)
        {
            Camera.Name = Name;
            Camera.Address = Address;
        }
    }
}
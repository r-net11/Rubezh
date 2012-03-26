using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using VideoModule.Views;

namespace VideoModule.ViewModels
{
    public class CameraDetailsViewModel : SaveCancelDialogContent
    {
        public Camera Camera { get; private set; }
        public List<ulong> Zones { get; set; }

        public CameraDetailsViewModel(Camera camera = null)
        {
            ShowZonesCommand = new RelayCommand(OnShowZones);
            TestCommand = new RelayCommand(OnTest);

            if (camera != null)
            {
                Title = "Редактировать камеру";
                Camera = camera;
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
            if (Camera.Zones == null)
                Camera.Zones = new List<ulong>();
            Zones = Camera.Zones.ToList();
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

        public string PresenrationZones
        {
            get
            {
                var presenrationZones = new StringBuilder();
                for (int i = 0; i < Zones.Count; i++)
                {
                    if (i > 0)
                        presenrationZones.Append(", ");
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Zones[i]);
                    if (zone != null)
                        presenrationZones.Append(zone.PresentationName);
                }

                return presenrationZones.ToString();
            }
        }

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zonesSelectationViewModel))
            {
                Zones = zonesSelectationViewModel.Zones;
                OnPropertyChanged("PresenrationZones");
            }
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
        {
            VideoService.ShowModal(Address); //"172.16.7.202"
        }

        protected override void Save(ref bool cancel)
        {
            Camera.Name = Name;
            Camera.Address = Address;
            Camera.Zones = Zones.ToList();
        }
    }
}
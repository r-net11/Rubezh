using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class ZonePropertiesViewModel : SaveCancelDialogContent
    {
        IElementZone IElementZone;

        public ZonePropertiesViewModel(IElementZone iElementZone)
        {
            IElementZone = iElementZone;
            CreateCommand = new RelayCommand(OnCreate);
            Title = "Свойства фигуры: Зона";
            Zones = new List<Zone>(FiresecManager.DeviceConfiguration.Zones);
            if (iElementZone.ZoneNo.HasValue)
                SelectedZone = Zones.FirstOrDefault(x => x.No == iElementZone.ZoneNo.Value);
        }

        public List<Zone> Zones { get; private set; }

        Zone _selectedZone;
        public Zone SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {

        }

        protected override void Save(ref bool cancel)
        {
            if (SelectedZone != null)
            {
                IElementZone.ZoneNo = SelectedZone.No;
                IElementZone.Zone = SelectedZone;
            }
        }
    }
}

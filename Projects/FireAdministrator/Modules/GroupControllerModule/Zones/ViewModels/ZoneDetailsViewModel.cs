using System.Linq;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class ZoneDetailsViewModel : SaveCancelDialogContent
    {
        bool _isNew;
        public XZone XZone;

        public ZoneDetailsViewModel(XZone xZone = null)
        {
            if (xZone == null)
            {
                _isNew = true;
                Title = "Создание новой зоны";

                XZone = new XZone()
                {
                    Name = "Новая зона",
                    No = 1
                };
                if (XManager.DeviceConfiguration.Zones.Count != 0)
                    XZone.No = (short)(XManager.DeviceConfiguration.Zones.Select(x => x.No).Max() + 1);
            }
            else
            {
                _isNew = false;
                Title = string.Format("Свойства зоны: {0}", xZone.PresentationName);
                XZone = xZone;
            }
            CopyProperties();
        }

        void CopyProperties()
        {
            Name = XZone.Name;
            No = XZone.No;
            Description = XZone.Description;
            DetectorCount = XZone.DetectorCount;
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

        short _no;
        public short No
        {
            get { return _no; }
            set
            {
                _no = value;
                OnPropertyChanged("No");
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

        short _detectorCount;
        public short DetectorCount
        {
            get { return _detectorCount; }
            set
            {
                _detectorCount = value;
                OnPropertyChanged("DetectorCount");
            }
        }

        protected override void Save(ref bool cancel)
        {
            if (XZone.No != No && XManager.DeviceConfiguration.Zones.Any(x => x.No == No))
            {
                MessageBoxService.Show("Зона с таким номером уже существует");
                return;
            }

            XZone.Name = Name;
            XZone.No = No;
            XZone.Description = Description;
            XZone.DetectorCount = DetectorCount;
        }
    }
}
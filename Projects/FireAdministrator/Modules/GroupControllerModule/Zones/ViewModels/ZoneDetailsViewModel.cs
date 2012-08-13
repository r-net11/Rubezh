using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class ZoneDetailsViewModel : SaveCancelDialogViewModel
    {
        public XZone XZone;

        public ZoneDetailsViewModel(XZone xZone = null)
        {
            if (xZone == null)
            {
                Title = "Создание новой зоны";

                XZone = new XZone()
                {
                    Name = "Новая зона",
                    No = 1
                };
                if (XManager.DeviceConfiguration.Zones.Count != 0)
                    XZone.No = (ushort)(XManager.DeviceConfiguration.Zones.Select(x => x.No).Max() + 1);
            }
            else
            {
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
            Fire1Count = XZone.Fire1Count;
            Fire2Count = XZone.Fire2Count;
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

        ushort _no;
        public ushort No
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

        ushort _fire1Count;
        public ushort Fire1Count
        {
            get { return _fire1Count; }
            set
            {
                _fire1Count = value;
                OnPropertyChanged("Fire1Count");
            }
        }

        ushort _fire2Count;
        public ushort Fire2Count
        {
            get { return _fire2Count; }
            set
            {
                _fire2Count = value;
                OnPropertyChanged("Fire2Count");
            }
        }

		protected override bool Save()
		{
            if (XZone.No != No && XManager.DeviceConfiguration.Zones.Any(x => x.No == No))
            {
                MessageBoxService.Show("Зона с таким номером уже существует");
                return false;
            }

            XZone.Name = Name;
            XZone.No = No;
            XZone.Description = Description;
            XZone.Fire1Count = Fire1Count;
            XZone.Fire2Count = Fire2Count;
			return base.Save();
		}
    }
}
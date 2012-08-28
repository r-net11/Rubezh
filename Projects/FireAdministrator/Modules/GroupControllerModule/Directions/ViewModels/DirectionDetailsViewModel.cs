using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DirectionDetailsViewModel : SaveCancelDialogViewModel
    {
		public DirectionDevicesViewModel DirectionDevicesViewModel { get; private set; }
		public XDirection XDirection { get; set; }

		public DirectionDetailsViewModel(XDirection direction = null)
        {
			if (direction == null)
            {
                Title = "Создание новоого направления";

				XDirection = new XDirection()
                {
                    Name = "Новое направление",
                    No = 1
                };
				if (XManager.DeviceConfiguration.Directions.Count != 0)
					XDirection.No = (ushort)(XManager.DeviceConfiguration.Directions.Select(x => x.No).Max() + 1);
            }
            else
            {
				Title = string.Format("Свойства направления: {0}", direction.PresentationName);
				XDirection = direction;
            }
			if (XDirection.DirectionDevices.Count == 0)
			{
				var directionDevice = new DirectionDevice();
				XDirection.DirectionDevices.Add(directionDevice);
			}
            CopyProperties();
			DirectionDevicesViewModel = new DirectionDevicesViewModel(XDirection);
        }

        void CopyProperties()
        {
			Name = XDirection.Name;
			No = XDirection.No;
			Delay = XDirection.Delay;
			Hold = XDirection.Hold;
			Regime = XDirection.Regime;
			Description = XDirection.Description;
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

		ushort _delay;
		public ushort Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged("Delay");
			}
		}

		ushort _hold;
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged("Hold");
			}
		}

		ushort _regime;
		public ushort Regime
		{
			get { return _regime; }
			set
			{
				_regime = value;
				OnPropertyChanged("Regime");
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

		protected override bool Save()
		{
			if (XDirection.No != No && XManager.DeviceConfiguration.Directions.Any(x => x.No == No))
            {
                MessageBoxService.Show("Направление с таким номером уже существует");
                return false;
            }

			XDirection.Name = Name;
			XDirection.No = No;
			XDirection.Delay = Delay;
			XDirection.Hold = Hold;
			XDirection.Regime = Regime;
			XDirection.Description = Description;
			DirectionDevicesViewModel.Save();
			return base.Save();
		}
    }
}
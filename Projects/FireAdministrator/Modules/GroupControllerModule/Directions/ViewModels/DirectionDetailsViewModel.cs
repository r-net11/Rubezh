using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DirectionDetailsViewModel : SaveCancelDialogViewModel
    {
		public XDirection XDirection { get; set; }

		public DirectionDetailsViewModel(XDirection direction = null)
		{
			ParametersHelper.AllParametersChanged -= ChangeParameter;
			ParametersHelper.AllParametersChanged += ChangeParameter;
			SetDirectionPropertiesCommand = new RelayCommand(OnSetDirectionProperties);
			GetDirectionPropertiesCommand = new RelayCommand(OnGetDirectionProperties);
			ResetAUPropertiesCommand = new RelayCommand(OnResetAUProperties);
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
            CopyProperties();
        }

		void ChangeParameter(ushort delay, ushort hold, ushort regime)
		{
			Delay = delay;
			Hold = hold;
			Regime = regime;
			OnPropertyChanged("Delay");
			OnPropertyChanged("Hold");
			OnPropertyChanged("Regime");
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
			return base.Save();
		}

		public RelayCommand GetDirectionPropertiesCommand { get; private set; }
		void OnGetDirectionProperties()
		{
			ParametersHelper.GetSingleDirectionParameter(XDirection);
		}

		public RelayCommand SetDirectionPropertiesCommand { get; private set; }
		void OnSetDirectionProperties()
		{
			XDirection.Name = Name;
			XDirection.No = No;
			XDirection.Description = Description;
			XDirection.Delay = Delay;
			XDirection.Hold = Hold;
			XDirection.Regime = Regime;
			ParametersHelper.SetSingleDirectionParameter(XDirection);
		}

		public RelayCommand ResetAUPropertiesCommand { get; private set; }
		void OnResetAUProperties()
		{
			Delay = 10;
			Hold = 10;
			Regime = 1;
		}
    }
}
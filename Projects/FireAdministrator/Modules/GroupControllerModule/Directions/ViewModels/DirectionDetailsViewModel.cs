using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;

namespace GKModule.ViewModels
{
    public class DirectionDetailsViewModel : SaveCancelDialogViewModel
    {
		public XDirection PumpStation { get; private set; }

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

				PumpStation = new XDirection()
                {
                    Name = "Новое направление",
                    No = 1
                };
				if (XManager.Directions.Count != 0)
					PumpStation.No = (ushort)(XManager.Directions.Select(x => x.No).Max() + 1);
            }
            else
            {
				Title = string.Format("Свойства направления: {0}", direction.PresentationName);
				PumpStation = direction;
            }
            CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDirection in XManager.Directions)
			{
				availableNames.Add(existingDirection.Name);
				availableDescription.Add(existingDirection.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
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
			Name = PumpStation.Name;
			No = PumpStation.No;
			Delay = PumpStation.Delay;
			Hold = PumpStation.Hold;
			Regime = PumpStation.Regime;
			Description = PumpStation.Description;
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

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (PumpStation.No != No && XManager.Directions.Any(x => x.No == No))
            {
                MessageBoxService.Show("Направление с таким номером уже существует");
                return false;
            }

			PumpStation.Name = Name;
			PumpStation.No = No;
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			PumpStation.Regime = Regime;
			PumpStation.Description = Description;
			return base.Save();
		}

		public RelayCommand GetDirectionPropertiesCommand { get; private set; }
		void OnGetDirectionProperties()
		{
			ParametersHelper.GetSingleDirectionParameter(PumpStation);
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand SetDirectionPropertiesCommand { get; private set; }
		void OnSetDirectionProperties()
		{
			PumpStation.Name = Name;
			PumpStation.No = No;
			PumpStation.Description = Description;
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			PumpStation.Regime = Regime;
			ParametersHelper.SetSingleDirectionParameter(PumpStation);
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
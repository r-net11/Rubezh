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
		public XDirection Direction { get; private set; }

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

				Direction = new XDirection()
                {
                    Name = "Новое направление",
                    No = 1
                };
				if (XManager.Directions.Count != 0)
					Direction.No = (ushort)(XManager.Directions.Select(x => x.No).Max() + 1);
            }
            else
            {
				Title = string.Format("Свойства направления: {0}", direction.PresentationName);
				Direction = direction;
            }
            CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingZone in XManager.Directions)
			{
				availableNames.Add(existingZone.Name);
				availableDescription.Add(existingZone.Description);
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
			Name = Direction.Name;
			No = Direction.No;
			Delay = Direction.Delay;
			Hold = Direction.Hold;
			Regime = Direction.Regime;
			Description = Direction.Description;
			IsNS = Direction.IsNS;
			NSPumpsCount = Direction.NSPumpsCount;
			NSDeltaTime = Direction.NSDeltaTime;
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

		#region NS
		bool _isNS;
		public bool IsNS
		{
			get { return _isNS; }
			set
			{
				_isNS = value;
				OnPropertyChanged("IsNS");
			}
		}

		int _mainPumpsCount;
		public int NSPumpsCount
		{
			get { return _mainPumpsCount; }
			set
			{
				_mainPumpsCount = value;
				OnPropertyChanged("MainPumpsCount");
			}
		}

		int _pumpsDeltaTime;
		public int NSDeltaTime
		{
			get { return _pumpsDeltaTime; }
			set
			{
				_pumpsDeltaTime = value;
				OnPropertyChanged("PumpsDeltaTime");
			}
		}
		#endregion

		protected override bool Save()
		{
			if (Direction.No != No && XManager.Directions.Any(x => x.No == No))
            {
                MessageBoxService.Show("Направление с таким номером уже существует");
                return false;
            }

			Direction.Name = Name;
			Direction.No = No;
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.Regime = Regime;
			Direction.Description = Description;
			Direction.IsNS = IsNS;
			Direction.NSPumpsCount = NSPumpsCount;
			Direction.NSDeltaTime = NSDeltaTime;
			return base.Save();
		}

		public RelayCommand GetDirectionPropertiesCommand { get; private set; }
		void OnGetDirectionProperties()
		{
			ParametersHelper.GetSingleDirectionParameter(Direction);
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand SetDirectionPropertiesCommand { get; private set; }
		void OnSetDirectionProperties()
		{
			Direction.Name = Name;
			Direction.No = No;
			Direction.Description = Description;
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.Regime = Regime;
			ParametersHelper.SetSingleDirectionParameter(Direction);
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		static int LastDetectorCount = 1;
		public Zone Zone;
		public bool ComboboxIsEnabled { get; private set; }

		public ZoneDetailsViewModel(ZoneType zoneType)
			:this(null)
		{
			ComboboxIsEnabled = false;
			ZoneType = zoneType;
		}

		public ZoneDetailsViewModel(Zone zone = null)
		{
			ComboboxIsEnabled = true;
			if (zone == null)
			{
				Title = "Создание новой зоны";

				Zone = new Zone()
				{
					Name = "Новая зона",
					No = 1,
					DetectorCount = LastDetectorCount
				};
				if (FiresecManager.Zones.Count != 0)
					Zone.No = FiresecManager.Zones.Select(x => x.No).Max() + 1;
				if (Zone.No > 99999)
					Zone.No = 99999;
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}
			CopyProperties();

			AvailableNames = new ObservableCollection<string>();
			AvailableDescription = new ObservableCollection<string>();
			foreach (var existingZone in FiresecManager.Zones)
			{
				AvailableNames.Add(existingZone.Name);
				AvailableDescription.Add(existingZone.Description);
			}
		}

		void CopyProperties()
		{
			ZoneType = Zone.ZoneType;
			Name = Zone.Name;
			No = Zone.No;
			Description = Zone.Description;
			DetectorCount = Zone.DetectorCount;
			AutoSet = Zone.AutoSet;
			Delay = Zone.Delay;
			Skipped = Zone.Skipped;
			GuardZoneType = Zone.GuardZoneType;
			EnableExitTime = Zone.EnableExitTime;
			ExitRestoreType = Zone.ExitRestoreType;
		}

		public List<ZoneType> AvailableZoneTypes
		{
			get { return Enum.GetValues(typeof(ZoneType)).Cast<ZoneType>().ToList(); }
		}

		ZoneType _zoneType;
		public ZoneType ZoneType
		{
			get { return _zoneType; }
			set
			{
				_zoneType = value;
				OnPropertyChanged(() => ZoneType);
				OnPropertyChanged(() => IsFireZone);
				OnPropertyChanged(() => IsGuardZone);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		int _detectorCount;
		public int DetectorCount
		{
			get { return _detectorCount; }
			set
			{
				_detectorCount = value;
				OnPropertyChanged(() => DetectorCount);
			}
		}

		public string EvacuationTime
		{
			get
			{
				var mptDevice = Zone.DevicesInZone.FirstOrDefault(x => x.Driver.DriverType == DriverType.MPT);
				if (mptDevice != null)
				{
					var timeoutProperty = mptDevice.Properties.FirstOrDefault(x => x.Name == "AU_Delay");
					if (timeoutProperty != null)
					{
						return timeoutProperty.Value;
					}
				}
				return null;
			}
		}

		GuardZoneType _guardZoneType;
		public GuardZoneType GuardZoneType
		{
			get { return _guardZoneType; }
			set
			{
				_guardZoneType = value;
				OnPropertyChanged(() => GuardZoneType);
				OnPropertyChanged(() => CanDelay);
				OnPropertyChanged(() => CanAutoSet);
				if (value == GuardZoneType.CanNotReset)
					Skipped = true;
			}
		}

		bool _skipped;
		public bool Skipped
		{
			get { return _skipped; }
			set
			{
				_skipped = value;
				OnPropertyChanged(() => Skipped);
			}
		}

		int _delay;
		public int Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		int _autoSet;
		public int AutoSet
		{
			get { return _autoSet; }
			set
			{
				_autoSet = value;
				OnPropertyChanged(() => AutoSet);
			}
		}

		bool _enableExitTime;
		public bool EnableExitTime
		{
			get { return _enableExitTime; }
			set
			{
				_enableExitTime = value;
				OnPropertyChanged(() => EnableExitTime);
			}
		}

		ExitRestoreType _exitRestoreType;
		public ExitRestoreType ExitRestoreType
		{
			get { return _exitRestoreType; }
			set
			{
				_exitRestoreType = value;
				OnPropertyChanged(() => ExitRestoreType);
			}
		}

		public bool CanDelay
		{
			get { return (GuardZoneType == GuardZoneType.Delay); }
		}

		public bool CanAutoSet
		{
			get { return (GuardZoneType != GuardZoneType.CanNotReset); }
		}

		public List<GuardZoneType> AvailableGuardZoneTypes
		{
			get { return Enum.GetValues(typeof(GuardZoneType)).Cast<GuardZoneType>().ToList(); }
		}

		public List<ExitRestoreType> AvailableExitRestoreTypes
		{
			get { return Enum.GetValues(typeof(ExitRestoreType)).Cast<ExitRestoreType>().ToList(); }
		}

		public bool IsFireZone
		{
			get { return ZoneType == ZoneType.Fire; }
		}

		public bool IsGuardZone
		{
			get { return ZoneType == ZoneType.Guard; }
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (Zone.No != No && FiresecManager.Zones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			LastDetectorCount = DetectorCount;

			Zone.ZoneType = ZoneType;
			Zone.Name = Name;
			Zone.No = No;
			Zone.Description = Description;
			Zone.DetectorCount = DetectorCount;
			Zone.GuardZoneType = GuardZoneType;
			Zone.Skipped = Skipped;
			Zone.Delay = Delay;
			Zone.AutoSet = AutoSet;
			Zone.EnableExitTime = EnableExitTime;
			Zone.ExitRestoreType = ExitRestoreType;
			FiresecManager.FiresecConfiguration.ChangeZone(Zone);
			Zone.OnColorTypeChanged();
			return base.Save();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public Zone Zone;

		public ZoneDetailsViewModel(Zone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";

				Zone = new Zone()
				{
					Name = "Новая зона",
					No = 1
				};
				if (FiresecManager.Zones.Count != 0)
					Zone.No = FiresecManager.Zones.Select(x => x.No).Max() + 1;
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}
			CopyProperties();
		}

		void CopyProperties()
		{
			ZoneType = Zone.ZoneType;
			Name = Zone.Name;
			No = Zone.No;
			Description = Zone.Description;
			DetectorCount = Zone.DetectorCount;
			EvacuationTime = Zone.EvacuationTime;
			AutoSet = Zone.AutoSet;
			Delay = Zone.Delay;
			Skipped = Zone.Skipped;
			GuardZoneType = Zone.GuardZoneType;
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
				OnPropertyChanged("ZoneType");
				OnPropertyChanged("IsFireZone");
				OnPropertyChanged("IsGuardZone");
			}
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

		int _no;
		public int No
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

		int _detectorCount;
		public int DetectorCount
		{
			get { return _detectorCount; }
			set
			{
				_detectorCount = value;
				OnPropertyChanged("DetectorCount");
			}
		}

		string _evacuationTime;
		public string EvacuationTime
		{
			get { return _evacuationTime; }
			set
			{
				_evacuationTime = value;
				OnPropertyChanged("EvacuationTime");
			}
		}

		GuardZoneType _guardZoneType;
		public GuardZoneType GuardZoneType
		{
			get { return _guardZoneType; }
			set
			{
				_guardZoneType = value;
				OnPropertyChanged("GuardZoneType");
				OnPropertyChanged("CanDelay");
				OnPropertyChanged("CanAutoSet");
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
				OnPropertyChanged("Skipped");
			}
		}

		string _delay;
		public string Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged("Delay");
			}
		}

		string _autoSet;
		public string AutoSet
		{
			get { return _autoSet; }
			set
			{
				_autoSet = value;
				OnPropertyChanged("AutoSet");
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

		public bool IsFireZone
		{
			get { return ZoneType == ZoneType.Fire; }
		}

		public bool IsGuardZone
		{
			get { return ZoneType == ZoneType.Guard; }
		}

		protected override bool Save()
		{
			if (Zone.No != No && FiresecManager.Zones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Zone.ZoneType = ZoneType;
			Zone.Name = Name;
			Zone.No = No;
			Zone.Description = Description;
			Zone.DetectorCount = DetectorCount;
			Zone.EvacuationTime = EvacuationTime;
			Zone.GuardZoneType = GuardZoneType;
			Zone.Skipped = Skipped;
			Zone.Delay = Delay;
			Zone.AutoSet = AutoSet;
			FiresecManager.FiresecConfiguration.ChangeZone(Zone);
			Zone.OnColorTypeChanged();
			return base.Save();
		}
	}
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class GuardZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public XGuardZone Zone;

		public GuardZoneDetailsViewModel(XGuardZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";

				Zone = new XGuardZone()
				{
					Name = "Новая зона",
					No = 1
				};
				if (XManager.DeviceConfiguration.GuardZones.Count != 0)
					Zone.No = (ushort)(XManager.DeviceConfiguration.GuardZones.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}
			CopyProperties();


			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingZone in XManager.DeviceConfiguration.GuardZones)
			{
				availableNames.Add(existingZone.Name);
				availableDescription.Add(existingZone.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			No = Zone.No;
			Name = Zone.Name;
			Description = Zone.Description;
			Level = Zone.Level;
			Delay = Zone.Delay;
		}

		ushort _no;
		public ushort No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
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

		int _level;
		public int Level
		{
			get { return _level; }
			set
			{
				_level = value;
				OnPropertyChanged(() => Level);
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

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (Zone.No != No && XManager.DeviceConfiguration.GuardZones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Zone.No = No;
			Zone.Name = Name;
			Zone.Description = Description;
			Zone.Level = Level;
			Zone.Delay = Delay;
			return base.Save();
		}
	}
}
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public XZone Zone;

		public ZoneDetailsViewModel(XZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";

				Zone = new XZone()
				{
					Name = "Новая зона",
					No = 1
				};
				if (XManager.Zones.Count != 0)
					Zone.No = (ushort)(XManager.Zones.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}
			CopyProperties();


			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingZone in XManager.Zones)
			{
				availableNames.Add(existingZone.Name);
				availableDescription.Add(existingZone.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			Name = Zone.Name;
			No = Zone.No;
			Description = Zone.Description;
			Fire1Count = Zone.Fire1Count;
			Fire2Count = Zone.Fire2Count;
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

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (Zone.No != No && XManager.Zones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Zone.Name = Name;
			Zone.No = No;
			Zone.Description = Description;
			Zone.Fire1Count = Fire1Count;
			Zone.Fire2Count = Fire2Count;
			return base.Save();
		}
	}
}
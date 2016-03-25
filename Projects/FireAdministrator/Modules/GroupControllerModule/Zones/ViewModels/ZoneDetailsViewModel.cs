using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		static int LastFire1Count = 2;
		static int LastFire2Count = 3;
		public GKZone Zone;

		public ZoneDetailsViewModel(GKZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";

				Zone = new GKZone()
				{
					Name = "Новая зона",
					No = 1,
					Fire1Count = LastFire1Count,
					Fire2Count = LastFire2Count
				};
				if (GKManager.Zones.Count != 0)
					Zone.No = (GKManager.Zones.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
				Zone = zone;
			}
			CopyProperties();


			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingZone in GKManager.Zones)
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
			Fire1Count = Zone.Fire1Count;
			Fire2Count = Zone.Fire2Count;
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

		int _fire1Count;
		public int Fire1Count
		{
			get { return _fire1Count; }
			set
			{
				_fire1Count = value;
				OnPropertyChanged(() => Fire1Count);
			}
		}

		int _fire2Count;
		public int Fire2Count
		{
			get { return _fire2Count; }
			set
			{
				_fire2Count = value;
				OnPropertyChanged(() => Fire2Count);
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (Zone.No != No && GKManager.Zones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			LastFire1Count = Fire1Count;
			LastFire2Count = Fire2Count;

			Zone.No = No;
			Zone.Name = Name;
			Zone.Description = Description;
			Zone.Fire1Count = Fire1Count;
			Zone.Fire2Count = Fire2Count;
			return base.Save();
		}
	}
}
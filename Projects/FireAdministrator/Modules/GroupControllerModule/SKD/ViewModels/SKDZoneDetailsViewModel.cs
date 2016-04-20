using RubezhAPI.SKD;
using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows.Windows;
using RubezhAPI.GK;
using RubezhClient;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class SKDZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKSKDZone Zone { get; private set; }

		public SKDZoneDetailsViewModel(GKSKDZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";
				Zone = new GKSKDZone()
				{
					Name = "Новая зона",
					No = 1,
				};
				if (GKManager.SKDZones.Count != 0)
					Zone.No = (GKManager.SKDZones.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.Name);
				Zone = zone;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescriptions = new HashSet<string>();
			foreach (var existingSKDZone in GKManager.SKDZones)
			{
				availableNames.Add(existingSKDZone.Name);
				availableDescriptions.Add(existingSKDZone.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescriptions = new ObservableCollection<string>(availableDescriptions);
		}

		public void CopyProperties()
		{
			No = Zone.No;
			Name = Zone.Name;
			Description = Zone.Description;
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
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(() => Name);
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged(() => Description);
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (No <= 0)
			{
				MessageBoxService.Show("Номер должен быть положительным числом");
				return false;
			}
			if (Zone.No != No && GKManager.SKDZones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Zone.No = No;
			Zone.Name = Name;
			Zone.Description = Description;
			return true;
		}

		public ObservableCollection<string> AvailableNames { get; set; }

		public ObservableCollection<string> AvailableDescriptions { get; set; }
	}
}
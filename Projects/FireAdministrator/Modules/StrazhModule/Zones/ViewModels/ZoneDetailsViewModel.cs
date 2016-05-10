using StrazhAPI.SKD;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace StrazhModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDZone Zone { get; private set; }

		public ZoneDetailsViewModel(SKDZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";
				Zone = new SKDZone()
				{
					Name = "Новая зона",
					No = 1,
				};
				if (SKDManager.Zones.Count != 0)
					Zone.No = (ushort)(SKDManager.Zones.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.Name);
				Zone = zone;
			}
			CopyProperties();
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
				OnPropertyChanged("No");
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
			if (Zone.No != No && SKDManager.Zones.Any(x => x.No == No))
			{
				MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
			}

			Zone.No = No;
			Zone.Name = Name;
			Zone.Description = Description;
			return true;
		}
	}
}
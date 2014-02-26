using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class GUDDetailsViewModel : SaveCancelDialogViewModel
	{
		GUDsViewModel GUDsViewModel;
		public GUD GUD { get; private set; }
		public AccessZonesSelectationViewModel AccessZonesSelectationViewModel { get; private set; }

		public GUDDetailsViewModel(GUDsViewModel gudsViewModel, GUD gud = null)
		{
			GUDsViewModel = gudsViewModel;
			if (gud == null)
			{
				Title = "Создание ГУД";
				gud = new GUD()
				{
					Name = "Новый ГУД",
				};
			}
			else
			{
				Title = string.Format("Свойства ГУД: {0}", gud.Name);
			}
			GUD = gud;
			CopyProperties();

			AccessZonesSelectationViewModel = new AccessZonesSelectationViewModel(GUD.CardZones, GUD.UID);
		}

		public void CopyProperties()
		{
			Name = GUD.Name;
			Description = GUD.Description;
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
					OnPropertyChanged("Name");
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
					OnPropertyChanged("Description");
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (GUDsViewModel.GUDs.Any(x => x.GUD.Name == Name && x.GUD.UID != GUD.UID))
			{
				MessageBoxService.ShowWarning("Название ГУД совпадает с введеннымы ранее");
				return false;
			}

			GUD.Name = Name;
			GUD.Description = Description;
			GUD.CardZones = AccessZonesSelectationViewModel.GetCardZones();
			return true;
		}
	}
}
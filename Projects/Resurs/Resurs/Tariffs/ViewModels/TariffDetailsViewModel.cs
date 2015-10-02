using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class TariffDetailsViewModel : SaveCancelDialogViewModel
	{
		public Tariff Tariff;
		private string _viewModel;

		public string ViewModel
		{
			get { return _viewModel; }
			set
			{
				_viewModel = value;
				OnPropertyChanged(() => ViewModel);
			}
		}

		public TariffDetailsViewModel(Tariff tariff = null)
		{
			TariffType = new ObservableCollection<TariffType>(Enum.GetValues(typeof(TariffType)).Cast<TariffType>());
			SelectedTariffType = ResursAPI.TariffType.ColdWater;
			if (tariff == null)
			{
				tariff = new Tariff
					{
						Description = "",
						Devices = new List<Device>(),
						Name = "Новый тариф",
						TariffParts = new List<TariffPart>(),
					};
				Tariff = tariff;
				
				Title = "Создание нового тарифа";
				TariffParts = 1;
			}
			else
			{
				Title = "Редактирование тарифа";
			}

			Name = tariff.Name;
			Description = tariff.Description;
		}
		public ObservableCollection<TariffType> TariffType { get; set; }
		public TariffType SelectedTariffType 
		{ 
			get 
			{
				return _selectedTariffType;
			} 
			set
			{ 
				_selectedTariffType = value; 
				OnPropertyChanged(() => SelectedTariffType); 
			}
		}

		TariffType _selectedTariffType;

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
		private Guid _UID;

		public Guid UID
		{
			get { return _UID; }
			set { _UID = value; }
		}


		private ushort _desiredTariffParts;

		public ushort DesiredTariffParts
		{
			get { return _desiredTariffParts; }
			set 
			{ 
				_desiredTariffParts = value;
				OnPropertyChanged(() => DesiredTariffParts); 
			}
		}


		private ushort _tariffParts;

		public ushort TariffParts
		{
			get { return _tariffParts; }
			set 
			{ 
				_tariffParts = (ushort)Tariff.TariffParts.Count; 
				OnPropertyChanged(() => TariffParts); 
			}
		}
		protected override bool Save()
		{
			Tariff.Name = Name;
			Tariff.Description = Description;
			Tariff.TariffType = SelectedTariffType;
			for (int i = 0; i < DesiredTariffParts; i++)
			{
				Tariff.TariffParts.Add(new TariffPart(Tariff));
			}
			return base.Save();
		}
	}
}
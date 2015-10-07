using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Resurs.ViewModels
{
	public class TariffDetailsViewModel : SaveCancelDialogViewModel
	{
		public bool IsNew { get; set; }
		
		public Tariff Tariff;
		
		public TariffDetailsViewModel(Tariff tariff = null)
		{
			TariffType = new ObservableCollection<TariffType>(Enum.GetValues(typeof(TariffType)).Cast<TariffType>());
			TariffParts = new ObservableCollection<TariffPartViewModel>();
			if (tariff == null)
			{
				IsNew = true;
				tariff = new Tariff
					{
						Description = "",
						Devices = new List<Device>(),
						Name = "Новый тариф",
						TariffParts = new List<TariffPart>(),
					};
				Tariff = tariff;
				Title = "Создание нового тарифа";
				SelectedTariffPartsNumber = 1;
			}
			else
			{
				Tariff = tariff;
				Title = "Редактирование тарифа";
				SelectedTariffPartsNumber = (byte)tariff.TariffParts.Count;
			}
		}
		
		public RelayCommand EditDevicesCommand { get; set; }
		
		void OnEditDevicesCommand()
		{

		}
		
		public ObservableCollection<TariffType> TariffType { get; set; }
		
		public bool IsDiscount
		{
			get { return Tariff.IsDiscount; }
			set 
			{ 
				Tariff.IsDiscount = value;
				OnPropertyChanged(() => IsDiscount);
			}
		}
		
		public TariffType SelectedTariffType
		{
			get
			{
				return Tariff.TariffType;
			}
			set
			{
				Tariff.TariffType = value;
				OnPropertyChanged(() => SelectedTariffType);
			}
		}
		
		public string Name
		{
			get { return Tariff.Name; }
			set
			{
				Tariff.Name = value;
				OnPropertyChanged(() => Name);
			}
		}
		
		public string Description
		{
			get { return Tariff.Description; }
			set
			{
				Tariff.Description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private ObservableCollection<TariffPartViewModel> _tariffParts;

		public ObservableCollection<TariffPartViewModel> TariffParts
		{
			get { return _tariffParts; }
			set { _tariffParts = value; }
		}

		public byte[] TariffPartsNumberEnum
		{
			get { return new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }; }
		}
		
		private byte _selectedTariffPartsNumber;
		
		public byte SelectedTariffPartsNumber
		{
			get { return _selectedTariffPartsNumber; }
			set
			{
				if (IsNew)
				{
					TariffParts.Add(new TariffPartViewModel(new TariffPart(Tariff)));
					Tariff.TariffParts.Add(new TariffPart(Tariff));
					IsNew = false;
				}
				else
				{
					//if add tariff intervals
					if (Tariff.TariffParts.Count < value)
					{
						for (int i = Tariff.TariffParts.Count; i < value; i++)
						{
							TariffParts.Add(new TariffPartViewModel(new TariffPart(Tariff)));
							Tariff.TariffParts.Add(new TariffPart(Tariff));
						}
					}
					//if remove tariff intervals
					else
					{
						TariffParts.Clear();
						for (int i = 0; i < value; i++)
						{
							TariffParts.Add(new TariffPartViewModel(Tariff.TariffParts.ElementAt(i)));
						}
					}
				}
				_selectedTariffPartsNumber = value;
				OnPropertyChanged(() => SelectedTariffPartsNumber);
			}
		}
		
		public byte TariffPartsNumber
		{
			get { return (byte)Tariff.TariffParts.Count; }
		}
		
		protected override bool Save()
		{
			Tariff.Name = Name;
			Tariff.Description = Description;
			Tariff.TariffType = SelectedTariffType;
			Tariff.IsDiscount = IsDiscount;
			Tariff.TariffParts.Clear();
			foreach (var item in TariffParts)
			{
				Tariff.TariffParts.Add(new TariffPart
				{
					Discount = item.Discount,
					EndTime = item.EndTime,
					Price = item.Price,
					StartTime = item.StartTime,
					Tariff = Tariff,
					Threshold = item.Threshold,
					UID = item.TariffPart.UID
				});
			}
			return base.Save();
		}
	}
}
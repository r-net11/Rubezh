using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
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

		public TariffDetailsViewModel()
		{
			EditDevicesCommand = new RelayCommand(OnEditDevicesCommand);
			TariffType = new ObservableCollection<TariffType>(Enum.GetValues(typeof(TariffType)).Cast<TariffType>());
			TariffParts = new ObservableCollection<TariffPartViewModel>();
			IsNew = true;
			Tariff = new Tariff
				{
					Description = "",
					Devices = new List<Device>(),
					Name = "Новый тариф",
					TariffParts = new List<TariffPart>(),
				};
			Title = "Создание нового тарифа";
			SelectedTariffPartsNumber = 1;
		}

		public TariffDetailsViewModel(Tariff tariff)
		{
			EditDevicesCommand = new RelayCommand(OnEditDevicesCommand);
			TariffType = new ObservableCollection<TariffType>(Enum.GetValues(typeof(TariffType)).Cast<TariffType>());
			TariffParts = new ObservableCollection<TariffPartViewModel>();
			IsNew = false;
			Tariff = tariff;
			Title = "Редактирование тарифа";
			SelectedTariffPartsNumber = (byte)tariff.TariffParts.Count;
		}

		public RelayCommand EditDevicesCommand { get; set; }

		void OnEditDevicesCommand()
		{
			var tariffDevicesViewModel = new TariffDevicesViewModel(this);
			if(DialogService.ShowModalWindow(tariffDevicesViewModel))
			{
				Tariff.Devices.Clear();
				foreach (var item in tariffDevicesViewModel.SelectedDevices)
				{
					Tariff.Devices.Add(item.Device);
					DBCash.GetDevice(item.Device.UID).TariffUID = Tariff.UID;
				}
			}
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
			get { return Tariff.TariffType; }
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

		public byte SelectedTariffPartsNumber
		{
			get { return (byte)TariffParts.Count; }
			set
			{
				if ((TariffParts.Count == 0) && IsNew)
				{
					TariffParts.Add(new TariffPartViewModel(new TariffPart(Tariff)));
					Tariff.TariffParts.Add(new TariffPart(Tariff));
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
						for (var i = 0; i < value; i++)
						{
							if (TariffParts.ElementAtOrDefault(i) != null)
							{
								Tariff.TariffParts.Add(TariffParts.ElementAt(i).TariffPart);
							}
						}
						TariffParts.Clear();
						for (int i = 0; i < value; i++)
						{
							TariffParts.Add(new TariffPartViewModel(Tariff.TariffParts.ElementAt(i)));
						}
					}
				}
				OnPropertyChanged(() => SelectedTariffPartsNumber);
			}
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
					UID = item.TariffPart.UID,
					Price = item.Price,
					Discount = item.Discount,
					Threshold = item.Threshold,
					EndTime = item.EndTime,
					StartTime = item.StartTime,
					Tariff = Tariff,
				});
			}
			return base.Save();
		}
	}
}
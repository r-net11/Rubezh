using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Resurs.ViewModels
{
	public class TariffDetailsViewModel : SaveCancelDialogViewModel
	{
		public TariffDetailsViewModel(Tariff tariff = null)
		{
			EditDevicesCommand = new RelayCommand(OnEditDevicesCommand);
			TariffTypes = new ObservableCollection<TariffType>(Enum.GetValues(typeof(TariffType)).Cast<TariffType>());
			TariffParts = new ObservableCollection<TariffPartViewModel>();
			if (tariff == null)
			{
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
			else
			{
				IsNew = false;
				Tariff = tariff;
				Title = "Редактирование тарифа: " + tariff.TariffType.ToDescription();
				SelectedTariffPartsNumber = tariff.TariffParts.Count;
				SelectedTariffType = tariff.TariffType;
			}
			GetMaxTariffParts();
		}
		public bool IsNew { get; private set; }

		public Tariff Tariff;

		public RelayCommand EditDevicesCommand { get; private set; }

		void OnEditDevicesCommand()
		{
			var tariffDevicesViewModel = new TariffDevicesViewModel(Tariff);
			if (DialogService.ShowModalWindow(tariffDevicesViewModel))
			{
				foreach (var device in Tariff.Devices)
				{
					DbCache.Devices.Single(x => x.UID == device.UID).TariffUID = null;
				}
				Tariff.Devices.Clear();
				foreach (var item in tariffDevicesViewModel.SelectedDevices)
				{
					Tariff.Devices.Add(item.Device);
					DbCache.Devices.Single(x => x.UID == item.Device.UID).TariffUID = Tariff.UID;
				}
				CanSave();
				GetMaxTariffParts();
			}
		}
		int _maxTariffParts;
		void GetMaxTariffParts()
		{
			_maxTariffParts = 8;
			foreach (var device in Tariff.Devices)
			{
				if (_maxTariffParts.CompareTo(device.MaxTatiffParts) > 0)
				{
					_maxTariffParts = device.MaxTatiffParts;
					if (SelectedTariffPartsNumber > _maxTariffParts)
					{
						SelectedTariffPartsNumber = _maxTariffParts;
					}
				}
			}
			OnPropertyChanged(() => TariffPartsNumberEnum);
		}
		public ObservableCollection<TariffType> TariffTypes { get; private set; }
		public TariffType SelectedTariffType
		{
			get { return Tariff.TariffType; }
			set
			{
				Tariff.TariffType = value;
				OnPropertyChanged(() => SelectedTariffType);
			}
		}

		public bool IsDiscount
		{
			get { return Tariff.IsDiscount; }
			set
			{
				Tariff.IsDiscount = value;
				OnPropertyChanged(() => IsDiscount);
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

		public ObservableCollection<TariffPartViewModel> TariffParts { get; private set; }

		public IEnumerable TariffPartsNumberEnum
		{
			get
			{
				return Enumerable.Range(1, _maxTariffParts).Select(x => x);
			}
		}

		public int SelectedTariffPartsNumber
		{
			get { return TariffParts.Count; }
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

		bool ValidateTariff()
		{
			foreach (var device in Tariff.Devices)
			{
				if (device.TariffType != Tariff.TariffType)
					return false;
			}
			return true;
		}

		public bool IsTariffValid { get { return !ValidateTariff(); } }

		public bool IsEnable { get { return DbCache.CheckPermission(PermissionType.ViewDevice); } }

		protected override bool CanSave()
		{
			var result = ValidateTariff();
			OnPropertyChanged(() => IsTariffValid);
			AllowSave = true;
			return result;
		}

		protected override bool Save()
		{
			if (ValidateTariff())
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
						StartTime = item.StartTime,
						Tariff = Tariff,
					});
				}
				return base.Save();
			}
			else
			{
				MessageBoxService.ShowWarning("Имеются ошибки конфигурации. Изменения не сохранены.");
				return false;
			}
		}
	}
}
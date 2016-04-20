using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		public SettingsViewModel()
		{
			GenerateCommand = new RelayCommand(OnGenerate);
			LicenseViewModel = new LicenseViewModel();
		}

		public LicenseViewModel LicenseViewModel { get; private set; }

		public RelayCommand GenerateCommand { get; private set; }
		void OnGenerate()
		{
			DropDB();
			CreateDevices();
			CreateConsumers();
			CreateUsers();
			CreateMeasures();
			CreateTariffs();
		}

		List<Device> avaliableDevices = new List<Device>();
		void GetAllDevices(Device device)
		{
			foreach (var child in device.Children)
			{
				if (child.DeviceType == DeviceType.Counter)
				{
					avaliableDevices.Add(child);
				}
				else
				{
					GetAllDevices(child);
				}
			}
		}
		void CreateTariffs()
		{
			var random = new Random();
			var tariffs = new List<Tariff>();
			Array TariffType = Enum.GetValues(typeof(TariffType));
			GetAllDevices(DbCache.GetRootDevice());
			for (int i = 0; i < 25; i++)
			{
				var tariffParts = new List<TariffPart>();
				tariffs.Add(new Tariff
				{
					Description = "Описание тарифа " + i.ToString(),
					Devices = new List<Device>(),
					IsDiscount = random.NextDouble() > 0.9,
					Name = "Тестовый тариф" + i.ToString(),
					TariffParts = new List<TariffPart>(),
					TariffType = (TariffType)TariffType.GetValue(random.Next(TariffType.Length)),
				});
				int tariffPartsNumber = random.Next(1, 8);
				for (int j = 0; j < tariffPartsNumber; j++)
				{
					tariffParts.Add(new TariffPart
					{
						Discount = random.Next(0, 1000),
						StartTime = new TimeSpan(random.Next(0, 23), random.Next(0, 59), random.Next(0, 59)),
						Price = random.Next(0, 1000),
						Tariff = tariffs[i],
						Threshold = random.Next(0, 1000),
					});
				}
				tariffs[i].TariffParts = tariffParts;
				
				foreach (var device in avaliableDevices)
				{
					if (device.TariffType == tariffs[i].TariffType)
					{
						tariffs[i].Devices.Add(device);
						avaliableDevices.Remove(device);
						break;
					}
				}
			}
			DbCache.CreateTariffs(tariffs);
		}

		void DropDB()
		{

		}

		void CreateDevices()
		{
			DbCache.GenerateTestDevices();
		}

		void CreateConsumers()
		{
			DbCache.CreateConsumers();
		}

		void CreateUsers()
		{
			DbCache.CreateUsers();
		}

		void CreateMeasures()
		{
			var counters = DbCache.GetAllChildren(DbCache.RootDevice).Where(x => x.DeviceType == DeviceType.Counter);
			var measures = new List<Measure>();
			var nowDate = DateTime.Now;
			var startDate = nowDate.AddDays(-7);
			foreach (var counter in counters)
			{
				float val0 = 0;
				float val1 = 0;
				float val2 = 0;
				float val3 = 0;
				var random = new Random();
				for (int i = 0; i < 7; i++)
				{
					var date = startDate.AddDays(i);
					val0 += (float)(random.NextDouble() * 10);
					val1 += (float)(random.NextDouble() * 10);
					val2 += (float)(random.NextDouble() * 10);
					val3 += (float)(random.NextDouble() * 10);
					measures.Add(DbCache.CreateMeasure(counter.UID, 0, val0, val0, date));
					measures.Add(DbCache.CreateMeasure(counter.UID, 1, val1, val1, date));
					measures.Add(DbCache.CreateMeasure(counter.UID, 2, val2, val2, date));
					measures.Add(DbCache.CreateMeasure(counter.UID, 3, val3, val3, date));
				} 
			}
			DbCache.AddRangeMeasures(measures);
			

		}
	}
}
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
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
		}

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

		void CreateTariffs()
		{
			var random = new Random();
			var tariffs = new List<Tariff>();
			Array TariffType = Enum.GetValues(typeof(TariffType));

			for (int i = 0; i < 150; i++)
			{
				tariffs.Add(new Tariff
				{
					Description = "Описание тарифа " + i.ToString(),
					Devices = new List<Device>(),
					IsDiscount = random.NextDouble() > 0.9,
					Name = "Тестовый тариф" + i.ToString(),
					TariffParts = new List<TariffPart>(),
					TariffType = (TariffType)TariffType.GetValue(random.Next(TariffType.Length)),
				});
				tariffs[i].TariffParts.Add(new TariffPart
				{
					Discount = random.Next(0, 1000),
					EndTime = new TimeSpan(random.Next(0, 23), random.Next(0, 59), random.Next(0, 59)),
					StartTime = new TimeSpan(random.Next(0, 23), random.Next(0, 59), random.Next(0, 59)),
					Price = random.Next(0, 1000),
					Tariff = tariffs[i],
					Threshold = random.Next(0, 1000),
				});
			}
			DBCash.CreateTariffs(tariffs);

		}

		void DropDB()
		{

		}

		void CreateDevices()
		{
			DBCash.CreateSystem();
		}

		void CreateConsumers()
		{
			DBCash.CreateConsumers();
		}

		void CreateUsers()
		{

		}

		void CreateMeasures()
		{
			var counters = DBCash.GetAllChildren(DBCash.RootDevice).Where(x => x.DeviceType == DeviceType.Counter);
			var measures = new List<Measure>();
			var nowDate= DateTime.Now;
			var startDate = nowDate.AddDays(-7);
			foreach (var counter in counters)
			{
				double val0 = 0;
				double val1 = 0;
				double val2 = 0;
				double val3 = 0;
				var random = new Random();
				for (int i = 0; i < 7; i++)
				{
					var date = startDate.AddDays(i);
					val0 += random.NextDouble() * 10;
					val1 += random.NextDouble() * 10;
					val2 += random.NextDouble() * 10;
					val3 += random.NextDouble() * 10;
					measures.Add(DBCash.CreateMeasure(counter.UID, 0, val0, date));
					measures.Add(DBCash.CreateMeasure(counter.UID, 1, val1, date));
					measures.Add(DBCash.CreateMeasure(counter.UID, 2, val2, date));
					measures.Add(DBCash.CreateMeasure(counter.UID, 3, val3, date));
				} 
			}
			DBCash.AddRangeMeasures(measures);
			

		}
	}
}
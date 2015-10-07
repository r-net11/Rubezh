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
			CreateApartments();
			CreateUsers();
			CreateMeasures();
			
		}

		void DropDB()
		{

		}

		void CreateDevices()
		{

		}

		void CreateApartments()
		{

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
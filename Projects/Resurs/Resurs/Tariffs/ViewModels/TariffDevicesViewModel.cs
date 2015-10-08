using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.ObjectModel;

namespace Resurs.ViewModels
{
	class TariffDevicesViewModel : SaveCancelDialogViewModel
	{
		public TariffDevicesViewModel(TariffDetailsViewModel tariff)
		{
			Title = "Выбор счётчиков для привязки";
			//Devices = new ObservableCollection<Device>();
			//foreach (var item in DBCash.GetRootDevice().Children)
			//{
			//	Devices.Add(item); 
			//}
		}

		private TimeSpan myVar;

		public TimeSpan Myvar
		{
			get { return myVar; }
			set { myVar = value; }
		}


		//public ObservableCollection<Device> Devices;
		//protected override bool Save()
		//{
		//	return base.Save();
		//}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Common.GK;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItemViewModel(JournalItem journalItem)
		{
			JournalItem = journalItem;
			var internalAddress = journalItem.InternalJournalItem.ObjectDeviceAddress;
			var internalType = journalItem.InternalJournalItem.ObjectDeviceType;

			Address = internalAddress.ToString();

			var driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null)
			{
				TypeName = driver.ShortName;
				Address = (internalAddress / 256 + 1).ToString() + "." + (internalAddress % 256).ToString();
			}
			if (internalType == 0x100)
				TypeName = "Зона";
			if (internalType == 0x106)
				TypeName = "Направление";
		}

		public JournalItem JournalItem { get; private set; }
		public string TypeName { get; private set; }
		public string Address { get; private set; }
	}
}
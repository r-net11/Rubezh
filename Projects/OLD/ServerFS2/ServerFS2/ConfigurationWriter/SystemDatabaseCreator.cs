﻿using System.Collections.Generic;
using FiresecAPI.Models;
using ServerFS2.Service;

namespace ServerFS2.ConfigurationWriter
{
	public class SystemDatabaseCreator
	{
		public List<PanelDatabase> PanelDatabases { get; set; }
		public List<NonPanelDatabase> NonPanelDatabases { get; set; }
		public static BinaryConfigurationHelper BinaryConfigurationHelper { get; set; }

		public SystemDatabaseCreator()
		{
			PanelDatabases = new List<PanelDatabase>();
			NonPanelDatabases = new List<NonPanelDatabase>();
		}

		public void Create(int offset)
		{
			FS2Contract.CheckCancellationAndNotify("Формирования связей");
			BinaryConfigurationHelper = new BinaryConfigurationHelper();

			foreach (var device in ConfigurationManager.Devices)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.Rubezh_2AM:
					case DriverType.Rubezh_4A:
					case DriverType.Rubezh_2OP:
					case DriverType.BUNS:
					case DriverType.Rubezh_P:
					case DriverType.USB_Rubezh_2AM:
					case DriverType.USB_Rubezh_4A:
					case DriverType.USB_Rubezh_2OP:
					case DriverType.USB_BUNS:
					case DriverType.USB_Rubezh_P:
						FS2Contract.CheckCancellationAndNotify("Формирования базы прибора " + device.DottedPresentationNameAndAddress);
						var panelDatabase = new PanelDatabase(device, offset);
						PanelDatabases.Add(panelDatabase);
						break;
				}
			}

			BytesHelper.IsRevese = true;
			foreach (var device in ConfigurationManager.Devices)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.IndicationBlock:
						FS2Contract.CheckCancellationAndNotify("Формирования базы прибора " + device.DottedPresentationNameAndAddress);
						var biDatabase = new BIDatabase(device, this);
						NonPanelDatabases.Add(biDatabase);
						break;

					case DriverType.PDU:
						FS2Contract.CheckCancellationAndNotify("Формирования базы прибора " + device.DottedPresentationNameAndAddress);
						var pduDatabase = new PDUDatabase(device, this);
						NonPanelDatabases.Add(pduDatabase);
						break;

					case DriverType.PDU_PT:
						FS2Contract.CheckCancellationAndNotify("Формирования базы прибора " + device.DottedPresentationNameAndAddress);
						var pduPTDatabase = new PDUPTDatabase(device, this);
						NonPanelDatabases.Add(pduPTDatabase);
						break;
				}
			}
			BytesHelper.IsRevese = false;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class EffectorDeviceTable : TableBase
	{
		Device Device;
		bool IsOuter;

		public override Guid UID
		{
			get { return Device.UID; }
		}

		public EffectorDeviceTable(PanelDatabase panelDatabase, Device device, bool isOuter)
			: base(panelDatabase)
		{
			Device = device;
			IsOuter = isOuter;
		}

		public override void  Create()
{
			if (IsOuter)
			{
				var deviceCode = DriversHelper.GetCodeForFriver(Device.Driver.DriverType);
				BytesDatabase.AddByte((byte)deviceCode, "Тип внешнего ИУ");
			}
			var outerPanelAddress = 0;
			if (IsOuter)
			{
				outerPanelAddress = ParentPanel.IntAddress;
			}
			BytesDatabase.AddByte((byte)outerPanelAddress, "Адрес прибора привязки в сети");
			BytesDatabase.AddByte((byte)(Device.AddressOnShleif), "Адрес");
			BytesDatabase.AddByte((byte)Device.ShleifNo, "Номер шлейфа");
			BytesDatabase.AddShort(0, "Внутренние параметры");
			BytesDatabase.AddByte(0, "Динамические параметры для базы");
			BytesDatabase.AddString(Device.Description, "Описание");
			var configLengtByteDescription = BytesDatabase.AddByte(0, "Длина переменной части блока с конфигурацией и сырыми параметрами");
			var lengtByteDescription = BytesDatabase.AddShort(0, "Общая длина записи");
			var configLengt1 = BytesDatabase.ByteDescriptions.Count;
			AddDynamicBlock();
			AddConfig();
			var configLengt2 = BytesDatabase.ByteDescriptions.Count;
			configLengtByteDescription.Value = configLengt2 - configLengt1;
			AddLogic();
			BytesDatabase.SetShort(lengtByteDescription, (short)BytesDatabase.ByteDescriptions.Count);
			BytesDatabase.SetGroupName(Device.PresentationAddressAndName);
		}

		void AddDynamicBlock()
		{
			var count = 2;
			switch (Device.Driver.DriverType)
			{
				case DriverType.Valve:
					count = 6;
					break;

				case DriverType.MPT:
					count = 5;
					break;

				case DriverType.PumpStation:
					count = 0;
					break;

				case DriverType.MDU:
					count = 4;
					break;

				case DriverType.MRO_2:
					count = 3;
					break;
			}

			for (int i = 0; i < count; i++)
			{
				BytesDatabase.AddByte(0, "Сырой параметр устройства " + (i + 1).ToString());
			}
		}

		void AddConfig()
		{
			var config = 0;
			switch (Device.Driver.DriverType)
			{
				case DriverType.MPT:
					var isAutoBlock = false;
					var isAutoBlockProperty = Device.Properties.FirstOrDefault(x => x.Name == "Config");
					if ((isAutoBlockProperty == null) || (isAutoBlockProperty.Value == null))
						isAutoBlock = false;
					else
						isAutoBlock = true;
					if (isAutoBlock)
						config += 2;
					break;

				case DriverType.Valve:
					var valveAction = 0;
					var actionProperty = Device.Properties.FirstOrDefault(x => x.Name == "Action");
					if ((actionProperty == null) || (actionProperty.Value == null))
						valveAction = 0;
					else
						valveAction = 1;
					config = valveAction;
					break;

				case DriverType.MDU:
					var mduType = 0;
					var mduTypeProperty = Device.Properties.FirstOrDefault(x => x.Name == "начальное положение для привода пружинный ДУ");
					if ((mduTypeProperty == null) || (mduTypeProperty.Value == null))
						mduType = 0;
					else
						mduType = Int32.Parse(mduTypeProperty.Value);
					config = mduType;
					break;

				case DriverType.RM_1:
					if (Device.IsRmAlarmDevice)
						config += 0x80;
					if (Device.Parent.Driver.DriverType == DriverType.RM_2 || Device.Parent.Driver.DriverType == DriverType.RM_3 || Device.Parent.Driver.DriverType == DriverType.RM_4 || Device.Parent.Driver.DriverType == DriverType.RM_5)
					{
						var childIndex = Device.Parent.Children.IndexOf(Device);
						config += (childIndex << 2);
					}
					break;

				case DriverType.RM_2:
					config += (2 << 5);
					break;

				case DriverType.RM_3:
					config += (3 << 5);
					break;

				case DriverType.RM_4:
					config += (4 << 5);
					break;

				case DriverType.RM_5:
					config += (5 << 5);
					break;
			}
			BytesDatabase.AddByte((byte)config, "Конфиг");

			switch (Device.Driver.DriverType)
			{
				case DriverType.MPT:
					var mptParentAddress = 0;
					var mptParentShleif = 0;
					if (Device.Parent.Driver.DriverType == DriverType.MPT)
					{
						mptParentAddress = Device.Parent.AddressOnShleif;
						mptParentShleif = Device.Parent.ShleifNo;
					}
					BytesDatabase.AddByte((byte)mptParentAddress, "Адрес родителя");
					BytesDatabase.AddByte((byte)mptParentShleif, "Шлейф родителя");
					int delay = 0;
					var delayProperty = Device.Properties.FirstOrDefault(x => x.Name == "RunDelay");
					if ((delayProperty == null) || (delayProperty.Value == null))
						delay = 0;
					else
						delay = int.Parse(delayProperty.Value);
					BytesDatabase.AddShort((short)delay, "Задержка запуска");
					var localZoneNo = 0;
					if (Device.Zone != null)
					{
						if (ZonePanelRelations.IsLocalZone(Device.Zone, ParentPanel))
						{
							localZoneNo = ZonePanelRelations.GetLocalZoneNo(Device.Zone, ParentPanel);
						}
					}
					BytesDatabase.AddShort((short)localZoneNo, "Номер привязанной зоны");
					break;

				case DriverType.MRO_2:
					var mroParentAddress = 0;
					var mroParentShleif = 0;
					if (Device.Parent.Driver.DriverType == DriverType.MRO_2)
					{
						mroParentAddress = Device.Parent.AddressOnShleif;
						mroParentShleif = Device.Parent.ShleifNo;
					}
					BytesDatabase.AddByte((byte)mroParentAddress, "Адрес родителя");
					BytesDatabase.AddByte((byte)mroParentShleif, "Шлейф родителя");
					break;

				case DriverType.MDU:
					BytesDatabase.AddShort((short)0, "Общее количество связанных ИУ");
					for (int i = 0; i < ParentPanel.ShleifNo; i++)
					{
						BytesDatabase.AddByte((byte)0, "Количество связанных ИУ шлейфа " + (i + 1).ToString());
						BytesDatabase.AddReference(null, "Указатель на размещение абсолютного адреса размещения первого саиска связанного ИУ шлейфа " + (i + 1).ToString());
					}
					break;
			}
		}

		void AddLogic()
		{
			foreach (var clause in Device.ZoneLogic.Clauses)
			{
				var mroLogic = 0;
				BytesDatabase.AddByte((byte)mroLogic, "Логика МРО внутри группы зон");

				var state = 0;
				switch (clause.State)
				{
					case ZoneLogicState.MPTAutomaticOn:
						state = 0x01;
						break;

					case ZoneLogicState.Alarm:
						state = 0x02;
						break;

					case ZoneLogicState.GuardSet:
						state = 0x03;
						break;

					case ZoneLogicState.GuardUnSet:
						state = 0x05;
						break;

					case ZoneLogicState.PCN:
						state = 0x06;
						break;

					case ZoneLogicState.Fire:
						state = 0x04;
						break;

					case ZoneLogicState.Failure:
						state = 0x08;
						break;

					case ZoneLogicState.PumpStationOn:
						state = 0x09;
						break;

					case ZoneLogicState.PumpStationAutomaticOff:
						state = 0x0A;
						break;

					case ZoneLogicState.Attention:
						state = 0x20;
						break;

					case ZoneLogicState.MPTOn:
						state = 0x40;
						break;

					case ZoneLogicState.Firefighting:
						state = 0x80;
						break;

					case ZoneLogicState.AM1TOn:
						state = 0x0B;
						break;
				}
				BytesDatabase.AddByte((byte)state, "Состояние");

				var joinOperator = 0;
				if(Device.ZoneLogic.Clauses.IndexOf(clause) < Device.ZoneLogic.Clauses.Count - 1)
				{
				switch(Device.ZoneLogic.JoinOperator)
				{
					case ZoneLogicJoinOperator.And:
						joinOperator = 1;
						break;

					case ZoneLogicJoinOperator.Or:
						joinOperator = 2;
						break;
				}
				}
				BytesDatabase.AddByte((byte)joinOperator, "Логика операции со следующей логической группой");

				var zonesOrDevicesCount = 0;
				zonesOrDevicesCount = clause.Zones.Count;
				if(clause.Device != null)
					zonesOrDevicesCount = 1;
				BytesDatabase.AddShort((short)zonesOrDevicesCount, "Количество зон в этой группе или ИУ, по активации которого должно включится");

				foreach (var zone in clause.Zones)
				{
					if (ZonePanelRelations.IsLocalZone(zone, ParentPanel))
					{
						var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == zone.UID);
						BytesDatabase.AddReferenceToTable(table, "Указатель на участвующую в логике ЛОКАЛЬНУЮ зону, в которой не локальные ИП управляют данным локальным ИУ по логике межприборное И или ИУ, по активации которого должно включится");
					}
					else
					{
						var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == zone.UID);
						BytesDatabase.AddReferenceToTable(table, "Указатель на участвующую в логике ВНЕШНЮЮ зону, в которой не локальные ИП управляют данным локальным ИУ по логике межприборное И или ИУ, по активации которого должно включится");
					}
				}
			}
		}
	}
}
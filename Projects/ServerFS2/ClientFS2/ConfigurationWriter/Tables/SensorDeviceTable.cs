using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class SensorDeviceTable : TableBase
	{
		Device Device;

		public override Guid UID
		{
			get { return Device.UID; }
		}

		public SensorDeviceTable(PanelDatabase panelDatabase, Device device)
			: base(panelDatabase)
		{
			Device = device;
		}

		public override void Create()
		{
			BytesDatabase.AddByte((byte)Device.AddressOnShleif, "Адрес");
			BytesDatabase.AddByte((byte)Device.ShleifNo, "Шлейф");
			BytesDatabase.AddShort((byte)0, "Внутренние параметры");
			BytesDatabase.AddByte((byte)0, "Динамические параметры для базы");
			var zoneNo = 0;
			if (Device.Zone != null)
			{
				zoneNo = ZonePanelRelations.GetLocalZoneNo(Device.Zone, ParentPanel);
			}
			BytesDatabase.AddShort((short)zoneNo, "Номер зоны");
			var lengtByteDescription = BytesDatabase.AddByte((byte)0, "Длина блока данных устройства");
			for (int i = 0; i < Get80ByteCount(); i++)
			{
				BytesDatabase.AddByte((byte)0, "Байт 0x80 или 0x81");
			}
			AddDetectorProperties();

			switch (Device.Driver.DriverType)
			{
				case DriverType.AM_1:
				case DriverType.StopButton:
				case DriverType.StartButton:
				case DriverType.AutomaticButton:
				case DriverType.ShuzOnButton:
				case DriverType.ShuzOffButton:
				case DriverType.ShuzUnblockButton:
					AddAM1Config();
					break;

				case DriverType.AMT_4:
					AddAMT4Config();
					break;

				case DriverType.AM1_T:
					AddAM1_TConfig();
					break;

				case DriverType.AMP_4:
					AddAMP_4Config();
					break;

				case DriverType.AM1_O:
					AddAM1_OConfig();
					break;

				case DriverType.PowerSupply:
					AddPowerSupplyConfig();
					break;
			}
			lengtByteDescription.Value = BytesDatabase.ByteDescriptions.Count - 8;
			BytesDatabase.SetGroupName(Device.PresentationAddressAndName);
		}

		int Get80ByteCount()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.AMT_4:
				case DriverType.AM1_T:
				case DriverType.AM_1:
				case DriverType.AM1_O:
				case DriverType.StopButton:
				case DriverType.StartButton:
				case DriverType.AutomaticButton:
				case DriverType.ShuzOnButton:
				case DriverType.ShuzOffButton:
				case DriverType.ShuzUnblockButton:
					return 1;
			}
			return 2;
		}

		void AddDetectorProperties()
		{
			var smokeParameterValue = 18;
			var smokeParameter = Device.Properties.FirstOrDefault(x => x.Name == "AU_Smoke");
			if (smokeParameter != null)
			{
				smokeParameterValue = Int32.Parse(smokeParameter.Value);
			}

			var temperatureParameterValue = 18;
			var temperatureParameter = Device.Properties.FirstOrDefault(x => x.Name == "AU_Temperature");
			if (temperatureParameter != null)
			{
				temperatureParameterValue = Int32.Parse(temperatureParameter.Value);
			}

			byte computerConfigurationData = 0;

			switch (Device.Driver.DriverType)
			{
				case DriverType.SmokeDetector:
					BytesDatabase.AddByte((byte)smokeParameterValue, "Запыленность");
					break;
				case DriverType.HeatDetector:
					BytesDatabase.AddByte((byte)temperatureParameterValue, "Температура");
					break;
				case DriverType.CombinedDetector:
					BytesDatabase.AddByte((byte)computerConfigurationData, "Конфигурация с компа");
					BytesDatabase.AddByte((byte)smokeParameterValue, "Запыленность");
					BytesDatabase.AddByte((byte)temperatureParameterValue, "Температура");
					break;
				case DriverType.RadioSmokeDetector:
					BytesDatabase.AddByte((byte)smokeParameterValue, "Запыленность");
					BytesDatabase.AddByte((byte)(Device.Parent.AddressOnShleif), "Адрес родительского МРК");
					break;
				case DriverType.RadioHandDetector:
					BytesDatabase.AddByte((byte)(Device.Parent.AddressOnShleif), "Адрес родительского МРК");
					break;
			}
		}

		void AddAM1Config()
		{
			var config = 0;
			if (Device.Parent.Driver.DriverType == DriverType.AM4)
			{
				var childIndex = Device.Parent.Children.IndexOf(Device);
				childIndex = childIndex << 2;
				config += childIndex;
			}
			BytesDatabase.AddByte((byte)config, "Конфиг с компа");

			var amVitualType = GetAMVitualType();
			BytesDatabase.AddByte((byte)amVitualType, "ID виртуального типа");

			BytesDatabase.AddShort((short)0, "Общее количество привязанных к сработке виртуальных кнопок ИУ");
			for (int i = 0; i < ParentPanel.Driver.ShleifCount; i++)
			{
				BytesDatabase.AddByte((byte)0, "Количество связанных ИУ шлейфа " + (i+1).ToString());
				BytesDatabase.AddReference((ByteDescription)null, "Указатель на размещение абсолютного адреса размещения первого в списке связанного ИУ шлейфа " + (i + 1).ToString());
			}
		}

		void AddAMT4Config()
		{
			var config = 0;
			BytesDatabase.AddByte((byte)config, "Конфиг с компа");
			BytesDatabase.AddByte((byte)0, "Байт конфига порого минимума");
			BytesDatabase.AddByte((byte)0, "Байт конфига порого максимума");
			BytesDatabase.AddByte((byte)0, "Максимум измеряемого датчиком давления");
		}

		void AddAM1_TConfig()
		{
			var config = 0;
			BytesDatabase.AddByte((byte)config, "Конфиг с компа");
			BytesDatabase.AddString(Device.Description, "Описание");

			var event1PropertyValue = "";
			var event1Property = Device.Properties.FirstOrDefault(x => x.Name == "Event1");
			if (event1Property != null)
			{
				event1PropertyValue = event1Property.Value;
			}
			var event2PropertyValue = "";
			var event2Property = Device.Properties.FirstOrDefault(x => x.Name == "Event2");
			if (event2Property != null)
			{
				event2PropertyValue = event2Property.Value;
			}
			BytesDatabase.AddString(event1PropertyValue, "Описание сработавшего состояния");
			BytesDatabase.AddString(event2PropertyValue, "Описание состояния норма");

			BytesDatabase.AddShort((short)0, "Общее количество привязанных к сработке виртуальных кнопок ИУ");
			for (int i = 0; i < ParentPanel.Driver.ShleifCount; i++)
			{
				BytesDatabase.AddByte((byte)0, "Количество связанных ИУ шлейфа " + (i + 1).ToString());
				BytesDatabase.AddReference((ByteDescription)null, "Указатель на размещение абсолютного адреса размещения первого в списке связанного ИУ шлейфа " + (i + 1).ToString());
			}
		}

		void AddAMP_4Config()
		{
			var config = 0;
			BytesDatabase.AddByte((byte)config, "Конфиг с компа");
		}

		void AddAM1_OConfig()
		{
			BytesDatabase.AddByte((byte)0, "Неиспользуемый байт, можно убрать с инкрементом версии базы");
			BytesDatabase.AddByte((byte)0, "Байт подтипа извещателя");
			var config = 0;
			BytesDatabase.AddByte((byte)config, "Конфиг с компа");
		}

		void AddPowerSupplyConfig()
		{
			BytesDatabase.AddString(Device.Description, "Описание");
		}

		int GetAMVitualType()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.StopButton:
					return 0;
				case DriverType.StartButton:
					return 1;
				case DriverType.AutomaticButton:
					return 2;
				case DriverType.ShuzOnButton:
					return 3;
				case DriverType.ShuzOffButton:
					return 4;
				case DriverType.ShuzUnblockButton:
					return 5;
			}
			return 0;
		}
	}
}
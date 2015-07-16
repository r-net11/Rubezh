using FiresecAPI.GKModels.OPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Конфигурация ГК
	/// </summary>
	[DataContract]
	public partial class GKDeviceConfiguration : VersionedConfiguration
	{
		public GKDeviceConfiguration()
		{
			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			Directions = new List<GKDirection>();
			PumpStations = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			Delays = new List<GKDelay>();
			GuardZones = new List<GKGuardZone>();
			Codes = new List<GKCode>();
			Doors = new List<GKDoor>();
			SKDZones = new List<GKSKDZone>();
			OPCSettings = new OPCSettings();

			Instructions = new List<GKInstruction>();
			ParameterTemplates = new List<GKParameterTemplate>();
			GKNameGenerationType = GKNameGenerationType.DriverTypePlusAddressPlusDescription;
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		/// <summary>
		/// Устройство верхнего уровня
		/// </summary>
		[DataMember]
		public GKDevice RootDevice { get; set; }

		/// <summary>
		/// Пожарные зоны
		/// </summary>
		[DataMember]
		public List<GKZone> Zones { get; set; }

		/// <summary>
		/// Направления
		/// </summary>
		[DataMember]
		public List<GKDirection> Directions { get; set; }

		/// <summary>
		/// Насосные станции
		/// </summary>
		[DataMember]
		public List<GKPumpStation> PumpStations { get; set; }

		/// <summary>
		/// Модули пожаротушения
		/// </summary>
		[DataMember]
		public List<GKMPT> MPTs { get; set; }

		/// <summary>
		/// Задержки
		/// </summary>
		[DataMember]
		public List<GKDelay> Delays { get; set; }

		/// <summary>
		/// Инструкции
		/// </summary>
		[DataMember]
		public List<GKInstruction> Instructions { get; set; }

		/// <summary>
		/// Коды
		/// </summary>
		[DataMember]
		public List<GKCode> Codes { get; set; }

		/// <summary>
		/// Охранные зоны
		/// </summary>
		[DataMember]
		public List<GKGuardZone> GuardZones { get; set; }

		/// <summary>
		/// Точки доступа
		/// </summary>
		[DataMember]
		public List<GKDoor> Doors { get; set; }

		/// <summary>
		/// Зоны СКД
		/// </summary>
		[DataMember]
		public List<GKSKDZone> SKDZones { get; set; }

		/// <summary>
		/// Шаблоны параметров устройств
		/// </summary>
		[DataMember]
		public List<GKParameterTemplate> ParameterTemplates { get; set; }

		/// <summary>
		/// Тип генерации названия компонентка в ГК
		/// </summary>
		[DataMember]
		public GKNameGenerationType GKNameGenerationType { get; set; }

		[DataMember]
		public OPCSettings OPCSettings { get; set; }

		public void Update()
		{
			Devices = new List<GKDevice>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChild(RootDevice);
			}
		}

		void AddChild(GKDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChild(device);
			}
		}

		[XmlIgnore]
		public List<GKZone> SortedZones
		{
			get
			{
				return (
				from GKZone zone in Zones
				orderby zone.No
				select zone).ToList();
			}
		}

		public void Reorder()
		{
			foreach (var device in Devices)
			{
				device.SynchronizeChildern();
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;

			if (RootDevice == null)
			{
				var device = new GKDevice();
				device.DriverUID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
				RootDevice = device;
				result = false;
			}

			Update();

			foreach (var delay in Delays)
			{
			}
			foreach (var mpt in MPTs)
			{
				foreach (var mptDevice in mpt.MPTDevices)
				{
				}
			}
			foreach (var device in Devices)
			{
			}
			foreach (var pumpStation in PumpStations)
			{
			}
			foreach (var parameterTemplate in ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
				}
			}
			return result;
		}
	}
}
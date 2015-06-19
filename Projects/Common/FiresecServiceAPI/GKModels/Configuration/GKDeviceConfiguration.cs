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
			Doors = new List<GKDoor>();
			SKDZones = new List<GKSKDZone>();
			Holidays = new List<GKHoliday>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		/// <summary>
		/// Устройство верхнего уровня
		/// </summary>
		[DataMember]
		public GKDevice RootDevice { get; set; }

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

		///// <summary>
		///// Графики работ
		///// </summary>
		//[DataMember]
		//public List<GKDaySchedule> DaySchedules { get; set; }

		// ///<summary>
		// ///Графики работ
		// ///</summary>
		//[DataMember]
		//public List<GKSchedule> Schedules { get; set; }

		/// <summary>
		/// Праздники
		/// </summary>
		[DataMember]
		public List<GKHoliday> Holidays { get; set; }

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
			return result;
		}
	}
}
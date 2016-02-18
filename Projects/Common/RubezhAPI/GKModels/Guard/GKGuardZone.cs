﻿using Common;
using Infrustructure.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Охранная зона ГК
	/// </summary>
	[DataContract]
	public class GKGuardZone : GKBase, IPlanPresentable
	{
		public GKGuardZone()
		{
			PlanElementUIDs = new List<Guid>();
			GuardZoneDevices = new List<GKGuardZoneDevice>();
			Pim = new GKPim()
			{
				IsAutoGenerated = true,
				GuardZoneUID = UID,
				UID = GuidHelper.CreateOn(UID, 0)
			};
			AlarmDelay = 1;
		}

		public override void Invalidate(GKDeviceConfiguration deviceConfiguration)
		{
			var guardZoneDevices = new List<GKGuardZoneDevice>();
			foreach (var guardZoneDevice in GuardZoneDevices)
			{
				var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == guardZoneDevice.DeviceUID);
				if (device != null)
				{
					if (device.DriverType == GKDriverType.RSR2_GuardDetector || device.DriverType == GKDriverType.RSR2_GuardDetectorSound || device.DriverType == GKDriverType.RSR2_AM_1 || device.DriverType == GKDriverType.RSR2_MAP4 || device.DriverType == GKDriverType.RSR2_CodeReader || device.DriverType == GKDriverType.RSR2_CardReader)
					{
						guardZoneDevice.Device = device;
						guardZoneDevices.Add(guardZoneDevice);
					}
					if (device.Driver.IsCardReaderOrCodeReader)
					{
						deviceConfiguration.InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.SetGuardSettings);
						deviceConfiguration.InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.ResetGuardSettings);
						deviceConfiguration.InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.ChangeGuardSettings);
						deviceConfiguration.InvalidateGKCodeReaderSettingsPart(guardZoneDevice.CodeReaderSettings.AlarmSettings);
					}
				}
			}
			GuardZoneDevices = guardZoneDevices;
		}

		bool _isLogicOnKau;
		[XmlIgnore]
		public override bool IsLogicOnKau
		{
			get { return _isLogicOnKau; }
			set
			{
				_isLogicOnKau = value;
				if (Pim != null)
					Pim.IsLogicOnKau = value;
			}
		}

		[XmlIgnore]
		public GKPim Pim { get; private set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.GuardZone; } }

		[XmlIgnore]
		public List<Guid> PlanElementUIDs { get; set; }

		/// <summary>
		/// Задержка на постановку
		/// </summary>
		[DataMember]
		public int SetDelay { get; set; }

		/// <summary>
		/// Задержка на снятие
		/// </summary>
		[DataMember]
		public int ResetDelay { get; set; }

		/// <summary>
		/// Задержка на вызов тревоги
		/// </summary>
		[DataMember]
		public int AlarmDelay { get; set; }

		/// <summary>
		/// Особо охраняемая
		/// </summary>
		[DataMember]
		public bool IsExtraProtected { get; set; }

		/// <summary>
		/// Устройства охранной зоны
		/// </summary>
		[DataMember]
		public List<GKGuardZoneDevice> GuardZoneDevices { get; set; }

		[XmlIgnore]
		public override string PresentationName
		{
			get
			{
				var presentationName = No + "." + Name;
				if (Pim != null)
					Pim.Name = presentationName;
				return presentationName;
			}
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/GuardZone.png"; }
		}

		public bool HasAccessLevel
		{
			get
			{
				foreach (var codeDevice in GuardZoneDevices.Where(x => x.Device.Driver.IsCardReaderOrCodeReader))
				{
					if (codeDevice.CodeReaderSettings.AlarmSettings.AccessLevel > 0)
						return true;
					if (codeDevice.CodeReaderSettings.ChangeGuardSettings.AccessLevel > 0)
						return true;
					if (codeDevice.CodeReaderSettings.ResetGuardSettings.AccessLevel > 0)
						return true;
					if (codeDevice.CodeReaderSettings.SetGuardSettings.AccessLevel > 0)
						return true;
				}
				return false;
			}
		}
	}
}
﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using FiresecClient;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
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

		public override void Update(GKDevice device)
		{
			GKManager.RemoveDeviceFromGuardZone(device, this);
			UnLinkObject(device);
		}

		public override void Update(GKDirection direction)
		{

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

		public List<Guid> GetCodeUids()
		{
			var codeUids = new List<Guid>();
			foreach (var guardZoneDevice in GuardZoneDevices)
			{
				codeUids.AddRange(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs);
				codeUids.AddRange(guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs);
				codeUids.AddRange(guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs);
				codeUids.AddRange(guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs);
			}
			return codeUids;
		}

		public bool HasAccessLevel
		{
			get
			{
				foreach (var codeDevice in GuardZoneDevices.Where(x => x.Device.DriverType == GKDriverType.RSR2_CodeReader || x.Device.DriverType == GKDriverType.RSR2_CardReader))
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
﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

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
			Pim = new GKPim();
			Pim.Name = PresentationName;
			Pim.IsAutoGenerated = true;
			Pim.GuardZoneUID = UID;
			Pim.UID = GuidHelper.CreateOn(UID, 0);
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
		/// Метод ввода
		/// </summary>
		[DataMember]
		public GKGuardZoneEnterMethod GuardZoneEnterMethod { get; set; }

		/// <summary>
		/// Минимальный уровень на постановку
		/// </summary>
		[DataMember]
		public int SetGuardLevel { get; set; }

		/// <summary>
		/// Минимальный уровень на снятие
		/// </summary>
		[DataMember]
		public int ResetGuardLevel { get; set; }

		/// <summary>
		/// Минимальный уровень на вызов тревоги
		/// </summary>
		[DataMember]
		public int SetAlarmLevel { get; set; }

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
	}
}
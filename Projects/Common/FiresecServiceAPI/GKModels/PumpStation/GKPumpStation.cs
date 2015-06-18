﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Насосная станция ГК
	/// </summary>
	[DataContract]
	public class GKPumpStation : GKBase
	{
		public GKPumpStation()
		{
			Delay = 30;
			Hold = 60;
			NSPumpsCount = 1;
			NSDeltaTime = 15;
			StartLogic = new GKLogic();
			StopLogic = new GKLogic();
			AutomaticOffLogic = new GKLogic();

			NSDevices = new List<GKDevice>();
			NSDeviceUIDs = new List<Guid>();

			Pim = new GKPim();
			Pim.IsAutoGenerated = true;
			Pim.PumpStationUID = UID;
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
					Pim.IsLogicOnKau = true;
			}
		}
		[XmlIgnore]
		public GKPim Pim { get; private set; }
		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.PumpStation; } }
		[XmlIgnore]
		public List<GKDevice> NSDevices { get; set; }

		/// <summary>
		/// Время задержки
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		ushort _hold;
		/// <summary>
		/// Время удержания
		/// </summary>		
		[DataMember]
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
			}
		}

		/// <summary>
		/// Количество основных насосов
		/// </summary>
		[DataMember]
		public int NSPumpsCount { get; set; }

		/// <summary>
		/// Время разновременного пуска
		/// </summary>
		[DataMember]
		public int NSDeltaTime { get; set; }

		/// <summary>
		/// Идентификаторы устройств, входящих в НС
		/// </summary>
		[DataMember]
		public List<Guid> NSDeviceUIDs { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic StartLogic { get; set; }

		/// <summary>
		/// Логика выключения
		/// </summary>
		[DataMember]
		public GKLogic StopLogic { get; set; }

		/// <summary>
		/// Логика отключения автоматики
		/// </summary>
		[DataMember]
		public GKLogic AutomaticOffLogic { get; set; }

		[XmlIgnore]
		public override string PresentationName
		{
			get 
			{
				var presentationName = "0" + No + "." + Name;
				if (Pim != null)
					Pim.Name = presentationName;
				return presentationName; 
			}
		}
	}
}
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
			DelayRegime = DelayRegime.Off;
			NSPumpsCount = 1;
			NSDeltaTime = 15;
			StartLogic = new GKLogic();
			StopLogic = new GKLogic();
			AutomaticOffLogic = new GKLogic();

			NSDevices = new List<GKDevice>();
			NSDeviceUIDs = new List<Guid>();

			MainDelay = new GKDelay();			
			MainDelay.Hold = 5;
			MainDelay.DelayRegime = DelayRegime.Off;

			Pim = new GKPim();
			Pim.IsAutoGenerated = true;
			Pim.PumpStationUID = UID;
			Pim.UID = GuidHelper.CreateOn(UID, 0);
		}

		public override void Update(GKDevice device)
		{
			StartLogic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			StopLogic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			AutomaticOffLogic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			NSDevices.Remove(device);
			NSDevices.ForEach(d => d.NSLogic.GetAllClauses().FindAll(x => { d.UnLinkObject(device); return x.Devices.Contains(device); }).ForEach(y =>{ y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); }));
			NSDeviceUIDs.Remove(device.UID);
			UnLinkObject(device);
			OnChanged();
		}

		public override void Update(GKDirection direction)
		{
			StartLogic.GetAllClauses().FindAll(x => x.Directions.Contains(direction)).ForEach(y => { y.Directions.Remove(direction); y.DirectionUIDs.Remove(direction.UID); });
			StopLogic.GetAllClauses().FindAll(x => x.Directions.Contains(direction)).ForEach(y => { y.Directions.Remove(direction); y.DirectionUIDs.Remove(direction.UID); });
			AutomaticOffLogic.GetAllClauses().FindAll(x => x.Directions.Contains(direction)).ForEach(y => { y.Directions.Remove(direction); y.DirectionUIDs.Remove(direction.UID); });
			UnLinkObject(direction);
			OnChanged();
		}

		[XmlIgnore]
		public override bool IsLogicOnKau { get; set; }

		[XmlIgnore]
		public GKPim Pim { get; private set; }
		[XmlIgnore]
		public GKDelay MainDelay { get; private set; }
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
				if (MainDelay != null)
					MainDelay.DelayTime = (ushort)(_hold - 5);
			}
		}
		/// <summary>
		/// Режим после удержания
		/// </summary>
		[DataMember]
		public DelayRegime DelayRegime { get; set; }

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
				if (MainDelay != null)
					MainDelay.Name = "Тушение " + presentationName;
				return presentationName; 
			}
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/BPumpStation.png"; }
		}
	}
}
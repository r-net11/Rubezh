﻿using Common;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Точка доступа ГК
	/// </summary>
	public class GKDoor : GKBase, IPlanPresentable
	{
		public GKDoor()
		{
			PlanElementUIDs = new List<Guid>();
			OpenRegimeLogic = new GKLogic();
			NormRegimeLogic = new GKLogic();
			CloseRegimeLogic = new GKLogic();
			Delay = 15;
			Hold = 20;
			PimEnter = new GKPim
			{
				IsAutoGenerated = true,
				DoorUID = UID,
				UID = GuidHelper.CreateOn(UID, 0)
			};
			PimExit = new GKPim
			{
				IsAutoGenerated = true,
				DoorUID = UID,
				UID = GuidHelper.CreateOn(UID, 1)
			};

			PimCrossing = new GKPim
			{
				IsAutoGenerated = true,
				DoorUID = UID,
				UID = GuidHelper.CreateOn(UID, 2)
			};
		}
		GKDevice InvalidateDoorDevice(Guid deviceUid, GKDeviceConfiguration deviceCongiguration)
		{
			if (deviceUid != Guid.Empty)
			{
				var device = deviceCongiguration.Devices.Find(x => x.UID == deviceUid);
				if (device == null)
					return null;
				device.Door = this;
				AddDependentElement(device);
				return device;
			}
			return null;
		}
		public override void Invalidate(GKDeviceConfiguration deviceConfiguration)
		{
			UpdateLogic(deviceConfiguration);
			OpenRegimeLogic.GetObjects().ForEach(AddDependentElement);
			NormRegimeLogic.GetObjects().ForEach(AddDependentElement);
			CloseRegimeLogic.GetObjects().ForEach(AddDependentElement);

			EnterDevice = InvalidateDoorDevice(EnterDeviceUID, deviceConfiguration);
			EnterDeviceUID = EnterDevice != null ? EnterDevice.UID : Guid.Empty;
			ExitDevice = InvalidateDoorDevice(ExitDeviceUID, deviceConfiguration);
			ExitDeviceUID = ExitDevice != null ? ExitDevice.UID : Guid.Empty;
			LockDevice = InvalidateDoorDevice(LockDeviceUID, deviceConfiguration);
			LockDeviceUID = LockDevice != null ? LockDevice.UID : Guid.Empty;
			LockControlDevice = InvalidateDoorDevice(LockControlDeviceUID, deviceConfiguration);
			LockControlDeviceUID = LockControlDevice != null ? LockControlDevice.UID : Guid.Empty;

			if (DoorType == GKDoorType.AirlockBooth)
			{
				EnterButton = InvalidateDoorDevice(EnterButtonUID, deviceConfiguration);
				EnterButtonUID = EnterButton != null ? EnterButton.UID : Guid.Empty;
				ExitButton = InvalidateDoorDevice(ExitButtonUID, deviceConfiguration);
				ExitButtonUID = ExitButton != null ? ExitButton.UID : Guid.Empty;
			}
			if (DoorType == GKDoorType.AirlockBooth || DoorType == GKDoorType.Barrier)
			{
				LockControlDeviceExit = InvalidateDoorDevice(LockControlDeviceExitUID, deviceConfiguration);
				LockControlDeviceExitUID = LockControlDeviceExit != null ? LockControlDeviceExit.UID : Guid.Empty;
			}
			if (DoorType == GKDoorType.Barrier || DoorType == GKDoorType.Turnstile || DoorType == GKDoorType.OneWay || DoorType == GKDoorType.TwoWay)
			{
				EnterButton = null;
				EnterButtonUID = Guid.Empty;
				ExitButton = null;
				ExitButtonUID = Guid.Empty;
			}
			if (DoorType == GKDoorType.Turnstile || DoorType == GKDoorType.OneWay || DoorType == GKDoorType.TwoWay)
			{
				LockControlDeviceExit = null;
				LockControlDeviceExitUID = Guid.Empty;
			}
			if (DoorType == GKDoorType.AirlockBooth || DoorType == GKDoorType.Barrier || DoorType == GKDoorType.Turnstile)
			{
				LockDeviceExit = InvalidateDoorDevice(LockDeviceExitUID, deviceConfiguration);
				LockDeviceExitUID = LockDeviceExit != null ? LockDeviceExit.UID : Guid.Empty;
			}
			if (DoorType == GKDoorType.OneWay || DoorType == GKDoorType.TwoWay)
			{
				LockDeviceExit = null;
				LockDeviceExitUID = Guid.Empty;
			}
		}

		public override void UpdateLogic(GKDeviceConfiguration deviceConfiguration)
		{
			deviceConfiguration.InvalidateOneLogic(this, OpenRegimeLogic);
			deviceConfiguration.InvalidateOneLogic(this, NormRegimeLogic);
			deviceConfiguration.InvalidateOneLogic(this, CloseRegimeLogic);
		}
		[XmlIgnore]
		public GKPim PimEnter { get; private set; }

		[XmlIgnore]
		public GKPim PimExit { get; private set; }

		[XmlIgnore]
		public GKPim PimCrossing { get; private set; }

		bool _isLogicOnKau;
		[XmlIgnore]
		public override bool IsLogicOnKau
		{
			get { return _isLogicOnKau; }
			set
			{
				_isLogicOnKau = value;
				if (PimEnter != null)
					PimEnter.IsLogicOnKau = value;
				if (PimExit != null)
					PimExit.IsLogicOnKau = value;
				if (PimCrossing != null)
					PimCrossing.IsLogicOnKau = value;
			}
		}

		[XmlIgnore]
		public override string PresentationName
		{
			get
			{
				var presentationName = No + "." + Name;
				if (PimEnter != null)
					PimEnter.Name = presentationName + " (вход)";
				if (PimExit != null)
					PimExit.Name = presentationName + " (выход)";
				if (PimCrossing != null)
					PimCrossing.Name = presentationName + " (пересечение)";
				return presentationName;
			}
		}
		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Door.png"; }
		}
		[XmlIgnore]
		public GKDevice EnterDevice { get; set; }
		[XmlIgnore]
		public GKDevice ExitDevice { get; set; }
		[XmlIgnore]
		public GKDevice EnterButton { get; set; }
		[XmlIgnore]
		public GKDevice ExitButton { get; set; }
		[XmlIgnore]
		public GKDevice LockDevice { get; set; }
		[XmlIgnore]
		public GKDevice LockDeviceExit { get; set; }
		[XmlIgnore]
		public GKDevice LockControlDevice { get; set; }
		[XmlIgnore]
		public GKDevice LockControlDeviceExit { get; set; }

		/// <summary>
		/// Тип ТД
		/// </summary>
		[DataMember]
		public GKDoorType DoorType { get; set; }

		/// <summary>
		/// Режим запрета повторного прохода
		/// </summary>
		[DataMember]
		public bool AntipassbackOn { get; set; }

		/// <summary>
		/// Задержка
		/// </summary>
		[DataMember]
		public int Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public int Hold { get; set; }

		/// <summary>
		/// Минимальный индивидуальный уровень на вход
		/// </summary>
		[DataMember]
		public int EnterLevel { get; set; }

		/// <summary>
		/// Идентификатор устройства на вход
		/// </summary>
		[DataMember]
		public Guid EnterDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства на выход
		/// </summary>
		[DataMember]
		public Guid ExitDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор кнопки на вход
		/// </summary>
		[DataMember]
		public Guid EnterButtonUID { get; set; }

		/// <summary>
		/// Идентификатор кнопки на выход
		/// </summary>
		[DataMember]
		public Guid ExitButtonUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Замок
		/// </summary>
		[DataMember]
		public Guid LockDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Замок на выход
		/// </summary>
		[DataMember]
		public Guid LockDeviceExitUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Датчик контроля двери
		/// </summary>
		[DataMember]
		public Guid LockControlDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Датчик контроля двери на выход
		/// </summary>
		[DataMember]
		public Guid LockControlDeviceExitUID { get; set; }

		/// <summary>
		/// Идентификатор зоны входа
		/// </summary>
		[DataMember]
		public Guid EnterZoneUID { get; set; }

		/// <summary>
		/// Идентификатор зоны выхода
		/// </summary>
		[DataMember]
		public Guid ExitZoneUID { get; set; }

		/// <summary>
		/// Логика перевода ТД в состояние Всегда Открыто
		/// </summary>
		[DataMember]
		public GKLogic OpenRegimeLogic { get; set; }

		/// <summary>
		/// Логика перевода ТД в состояние Норма
		/// </summary>
		[DataMember]
		public GKLogic NormRegimeLogic { get; set; }

		/// <summary>
		/// Логика перевода ТД в состояние Всегда Закрыто
		/// </summary>
		[DataMember]
		public GKLogic CloseRegimeLogic { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Door; } }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		/// <summary>
		/// Разрещить множественную визуализация на плане
		/// </summary>
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }
	}
}
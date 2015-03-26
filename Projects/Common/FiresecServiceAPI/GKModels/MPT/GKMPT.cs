using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using System;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Модуль пожаротушения ГК
	/// </summary>
	[DataContract]
	public class GKMPT : GKBase, IPlanPresentable
	{
		public GKMPT()
		{
			StartLogic = new GKLogic();
			StopLogic = new GKLogic();
			SuspendLogic = new GKLogic();
			MPTDevices = new List<GKMPTDevice>();
			Delay = 60;
			Devices = new List<GKDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		bool _isLogicOnKau;
		[XmlIgnore]
		public override bool IsLogicOnKau
		{
			get { return _isLogicOnKau; }
			set
			{
				_isLogicOnKau = value;
			}
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		/// <summary>
		/// Время задержки на включение
		/// </summary>
		[DataMember]
		public int Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public int Hold { get; set; }

		/// <summary>
		/// Режим после окончания удержания
		/// </summary>
		[DataMember]
		public DelayRegime DelayRegime { get; set; }

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
		/// Логика приостановки
		/// </summary>
		[DataMember]
		public GKLogic SuspendLogic { get; set; }

		/// <summary>
		/// Устройства МПТ
		/// </summary>
		[DataMember]
		public List<GKMPTDevice> MPTDevices { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.MPT; } }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "MПТ" + "." + No + "." + Name; }
		}

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		public List<Guid> GetCodeUids()
		{
			var codeUids = new List<Guid>();
			foreach (var mptDevice in MPTDevices)
			{
				codeUids.AddRange(mptDevice.CodeReaderSettings.AutomaticOnSettings.CodeUIDs);
				codeUids.AddRange(mptDevice.CodeReaderSettings.AutomaticOffSettings.CodeUIDs);
				codeUids.AddRange(mptDevice.CodeReaderSettings.StartSettings.CodeUIDs);
				codeUids.AddRange(mptDevice.CodeReaderSettings.StopSettings.CodeUIDs);
			}
			return codeUids;
		}
	}
}
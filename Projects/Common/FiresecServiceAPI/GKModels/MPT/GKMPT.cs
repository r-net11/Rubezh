using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Interfaces;
using FiresecClient;
using System.Linq;

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
			MptLogic = new GKLogic();
			MPTDevices = new List<GKMPTDevice>();
			Delay = 60;
			PlanElementUIDs = new List<Guid>();
		}

		public override void Update(GKDevice device)
		{
			MptLogic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			MPTDevices.RemoveAll(x => x.Device == device);
			UnLinkObject(device);
			OnChanged();
		}

		public override void Update(GKDirection direction)
		{
			MptLogic.GetAllClauses().FindAll(x => x.Directions.Contains(direction)).ForEach(y => { y.Directions.Remove(direction); y.DirectionUIDs.Remove(direction.UID); });
			UnLinkObject(direction);
			OnChanged();
		}
		public override void Invalidate()
		{
			foreach (var mptDevice in MPTDevices)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == mptDevice.DeviceUID);
				if (device != null && GKMPTDevice.GetAvailableMPTDriverTypes(mptDevice.MPTDeviceType).Contains(device.DriverType))
				{
					mptDevice.Device = device;
					device.IsInMPT = true;
					if (!device.OutDependentElements.Contains(this))
						device.OutDependentElements.Add(this);

					if (!InputDependentElements.Contains(device))
					{
						InputDependentElements.Add(device);
					}
				}
			}

			UpdateLogic();

			MptLogic.GetObjects().ForEach(x =>
			{
				if (!InputDependentElements.Contains(x) && x != this)
					InputDependentElements.Add(x);
				if (!x.OutDependentElements.Contains(this) && x != this)
					x.OutDependentElements.Add(this);
			});
		}

		public override void UpdateLogic()
		{
			GKManager.DeviceConfiguration.InvalidateInputObjectsBaseLogic(this, MptLogic);
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
		/// Логика МПТ
		/// </summary>
		[DataMember]
		public GKLogic MptLogic { get; set; }

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
			get { return No + "." + Name; }
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Pim.png"; }
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
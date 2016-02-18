using Infrustructure.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
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

		public override void Invalidate(GKDeviceConfiguration deviceConfiguration)
		{
			var newMPTDevices = new List<GKMPTDevice>();
			foreach (var mptDevice in MPTDevices)
			{
				var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == mptDevice.DeviceUID);
				if (device != null)
				{
					newMPTDevices.Add(mptDevice);
					mptDevice.Device = device;
					device.IsInMPT = true;
					AddDependentElement(device);
				}
			}
			MPTDevices = newMPTDevices;

			UpdateLogic(deviceConfiguration);

			MptLogic.GetObjects().ForEach(x =>
			{
				AddDependentElement(x);
			});
		}

		public override void UpdateLogic(GKDeviceConfiguration deviceConfiguration)
		{
			deviceConfiguration.InvalidateOneLogic(this, MptLogic);
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
			get { return "/Controls;component/Images/BMPT.png"; }
		}

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		public bool HasAccessLevel
		{
			get
			{
				foreach (var codeDevice in MPTDevices.Where(x => x.Device != null && x.Device.Driver.IsCardReaderOrCodeReader))
				{
					if (codeDevice.CodeReaderSettings.MPTSettings.AccessLevel > 0)
						return true;
				}
				return false;
			}
		}
	}
}
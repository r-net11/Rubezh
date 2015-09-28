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
	/// Направление ГК
	/// </summary>
	[DataContract]
	public class GKDirection : GKBase, IPlanPresentable
	{
		public GKDirection()
		{
			DelayRegime = DelayRegime.On;
			Logic = new GKLogic();
			PlanElementUIDs = new List<Guid>();
		}

		public override void Invalidate()
		{
			UpdateLogic();

			Logic.GetObjects().ForEach(x =>
			{
				AddDependentElement(x);
			});
		}

		public override void UpdateLogic()
		{
			GKManager.DeviceConfiguration.InvalidateInputObjectsBaseLogic(this, Logic);
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Direction; } }

		/// <summary>
		/// Задержка на включение
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic Logic { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/BDirection.png"; }
		}

		public GKDirection Clone()
		{
			var direction = new GKDirection();
			direction.Name = Name;
			direction.Description = Description;
			direction.Delay = Delay;
			direction.Hold = Hold;
			direction.DelayRegime = DelayRegime;
			direction.Logic.OnClausesGroup = Logic.OnClausesGroup.Clone();
			direction.Logic.OffClausesGroup = Logic.OffClausesGroup.Clone();
			direction.Logic.StopClausesGroup = Logic.StopClausesGroup.Clone();
			direction.Logic.OnNowClausesGroup = Logic.OnNowClausesGroup.Clone();
			direction.Logic.OffNowClausesGroup = Logic.OffNowClausesGroup.Clone();
			return direction;
		}
	}
}
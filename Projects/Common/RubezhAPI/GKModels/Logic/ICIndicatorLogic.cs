using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RubezhAPI.GK
{
	[DataContract]
	public class ICIndicatorLogic
	{
		public ICIndicatorLogic()
		{
			Blink1ClausesGroup = new GKClauseGroup();
			Blink3ClausesGroup = new GKClauseGroup();
			OffClausesGroup = new GKClauseGroup();
			UseOffCounterLogic = false;
		}

		[DataMember]
		public GKClauseGroup Blink1ClausesGroup { get; set; }

		[DataMember]
		public GKClauseGroup Blink3ClausesGroup { get; set; }

		[DataMember]
		public GKClauseGroup OffClausesGroup { get; set; }

		[DataMember]
		public bool UseOffCounterLogic { get; set; }

		public bool IsNotEmpty()
		{
			return Blink1ClausesGroup.IsNotEmpty() || Blink3ClausesGroup.IsNotEmpty() || OffClausesGroup.IsNotEmpty();
		}

		public List<GKBase> GetObjects()
		{
			var result = new List<GKBase>();
			result.AddRange(Blink1ClausesGroup.GetObjects());
			result.AddRange(Blink3ClausesGroup.GetObjects());
			if (UseOffCounterLogic)
				result.AddRange(OffClausesGroup.GetObjects());
			return result;
		}
	}
}

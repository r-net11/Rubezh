using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DayTimeTrackPart
	{
		#region Properties
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public bool IsNew { get; set; }
		[DataMember]
		public bool IsDirty { get; set; }
		[DataMember]
		public bool IsManuallyAdded { get; set; }
		[DataMember]
		public bool NotTakeInCalculations { get; set; }
		[DataMember]
		public bool IsOpen { get; set; }
		[DataMember]
		public bool IsNeedAdjustment { get; set; }
		[DataMember]
		public DateTime? EnterDateTime { get; set; }
		[DataMember]
		public TimeSpan EnterTime { get; set; }
		[DataMember]
		public DateTime? ExitDateTime { get; set; }
		[DataMember]
		public TimeSpan ExitTime { get; set; }
		[DataMember]
		public string CorrectedDate { get; set; }
		[DataMember]
		public string CorrectedBy { get; set; }
		[DataMember]
		public TimeTrackZone TimeTrackZone { get; set; }
		[DataMember]
		public bool IsForceClosed { get; set; }
		[DataMember]
		public DateTime? EnterTimeOriginal { get; set; }
		[DataMember]
		public DateTime? ExitTimeOriginal { get; set; }

		[DataMember]
		public bool IsRemoveAllIntersections { get; set; }

		[DataMember]
		public Guid CorrectedByUID { get; set; }

		#endregion
	}
}

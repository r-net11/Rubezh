using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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
		public DateTime? AdjustmentDate { get; set; }

		[DataMember]
		public DateTime? EnterDateTime { get; set; }
		[DataMember]
		public TimeSpan EnterTime { get; set; }
		[DataMember]
		public DateTime? ExitDateTime { get; set; }
		[DataMember]
		public TimeSpan ExitTime { get; set; }

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
		public Guid? CorrectedByUID { get; set; }

		[DataMember]
		public bool IsNeedAdjustmentOriginal { get; set; }

		[DataMember]
		public bool NotTakeInCalculationsOriginal { get; set; }

		[DataMember]
		public TimeTrackActions TimeTrackActions { get; set; }

		#endregion
	}
}

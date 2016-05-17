using System;
using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace GKModule.Validation
{
	class ZoneValidationError : PlanObjectValidationError<GKZone, ShowGKZoneEvent, Guid>
	{
		public ZoneValidationError(GKZone zone, string error, ValidationErrorLevel level, bool? isRightPanelVisible = null, Guid? planUID = null)
			: base(zone, error, level, isRightPanelVisible, planUID)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.GK; }
		}
		protected override Guid KeyValue
		{
			get { return Object.UID; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string Address
		{
			get { return Object.No.ToString(); }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Zone.png"; }
		}
	}
}
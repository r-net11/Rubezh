using System;
using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure.Common;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	class GuardZoneValidationError : PlanObjectValidationError<GKGuardZone, ShowGKGuardZoneEvent, Guid>
	{
		public GuardZoneValidationError(GKGuardZone guardZone, string error, ValidationErrorLevel level, bool? isRightPanelVisible = null, Guid? planUID = null)
			: base(guardZone, error, level, isRightPanelVisible, planUID)
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
			get { return "/Controls;component/Images/GuardZone.png"; }
		}
	}
}
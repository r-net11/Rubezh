using System;
using StrazhAPI.Enums;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;
using StrazhModule.Events;
using Infrastructure.Common;

namespace StrazhModule.Validation
{
	public class ZoneValidationError : PlanObjectValidationError<SKDZone, ShowSKDZoneEvent, Guid>
	{
		public ZoneValidationError(SKDZone zone, string error, ValidationErrorLevel level, bool? isRightPanelVisible = null, Guid? planUID = null)
			: base(zone, error, level, isRightPanelVisible, planUID)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.SKD; }
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
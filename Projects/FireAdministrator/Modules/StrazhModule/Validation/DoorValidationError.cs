using System;
using FiresecAPI.Enums;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using StrazhModule.Events;
using Infrastructure.Common;

namespace StrazhModule.Validation
{
	public class DoorValidationError : PlanObjectValidationError<SKDDoor, ShowSKDDoorEvent, Guid>
	{
		public DoorValidationError(SKDDoor door, string error, ValidationErrorLevel level, bool? isRightPanelVisible = null, Guid? planUID = null)
			: base(door, error, level, isRightPanelVisible, planUID)
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
			get { return "/Controls;component/Images/Door.png"; }
		}
	}
}
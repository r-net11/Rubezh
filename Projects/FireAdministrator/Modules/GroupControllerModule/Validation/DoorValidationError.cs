using System;
using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Validation;

namespace GKModule.Validation
{
	class DoorValidationError : PlanObjectValidationError<GKDoor, ShowGKDoorEvent, Guid>
	{
		public DoorValidationError(GKDoor door, string error, ValidationErrorLevel level, bool? isRightPanelVisible = null, Guid? planUID = null)
			: base(door, error, level, isRightPanelVisible, planUID)
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
			get { return "/Controls;component/Images/Door.png"; }
		}
	}
}
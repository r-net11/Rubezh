using GKModule.Events;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using RubezhAPI.GK;
using System;

namespace GKModule.Validation
{
	public class DirectionValidationError : PlanObjectValidationError<GKDirection, ShowGKDirectionEvent, Guid>
	{
		public DirectionValidationError(GKDirection direction, string error, ValidationErrorLevel level, bool? isRightPanelVisible = null, Guid? planUID = null)
			: base(direction, error, level, isRightPanelVisible, planUID)
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
			get { return "/Controls;component/Images/Blue_Direction.png"; }
		}
	}
}
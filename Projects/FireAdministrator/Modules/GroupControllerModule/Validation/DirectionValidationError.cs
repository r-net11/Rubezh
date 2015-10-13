using System;
using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace GKModule.Validation
{
	public class DirectionValidationError : ObjectValidationError<GKDirection, ShowGKDirectionEvent, Guid>
	{
		public DirectionValidationError(GKDirection direction, string error, ValidationErrorLevel level)
			: base(direction, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.GK; }
		}

		protected override Guid Key
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
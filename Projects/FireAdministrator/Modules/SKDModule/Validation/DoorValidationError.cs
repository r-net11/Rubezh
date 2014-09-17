using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using SKDModule.Events;

namespace SKDModule.Validation
{
	public class DoorValidationError : ObjectValidationError<SKDDoor, ShowSKDDoorEvent, Guid>
	{
		public DoorValidationError(SKDDoor door, string error, ValidationErrorLevel level)
			: base(door, error, level)
		{
		}

		public override string Module
		{
			get { return "SKD"; }
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
			get { return "/Controls;component/Images/Door.png"; }
		}
	}
}
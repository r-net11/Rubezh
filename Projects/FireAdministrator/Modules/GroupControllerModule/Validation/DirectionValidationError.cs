using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using XFiresecAPI;
using Infrastructure.Events;

namespace GKModule.Validation
{
    public class DirectionValidationError : ObjectValidationError<XDirection, ShowXDirectionEvent, Guid>
    {
        public DirectionValidationError(XDirection direction, string error, ValidationErrorLevel level)
			: base(direction, error, level)
		{
		}

		public override string Module
		{
			get { return "GK"; }
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
    }
}
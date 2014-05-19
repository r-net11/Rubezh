﻿using System;
using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	class MPTValidationError : ObjectValidationError<XMPT, ShowXMPTEvent, Guid>
	{
		public MPTValidationError(XMPT mpt, string error, ValidationErrorLevel level)
			: base(mpt, error, level)
		{
		}

		public override string Module
		{
			get { return "GK"; }
		}
		protected override Guid Key
		{
			get { return Object.BaseUID; }
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
			get { return "/Controls;component/Images/BMPT.png"; }
		}
	}
}
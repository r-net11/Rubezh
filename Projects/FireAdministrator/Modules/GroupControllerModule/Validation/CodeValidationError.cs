﻿using System;
using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace GKModule.Validation
{
	public class CodeValidationError : ObjectValidationError<GKCode, ShowGKCodeEvent, Guid>
	{
		public CodeValidationError(GKCode code, string error, ValidationErrorLevel level)
			: base(code, error, level)
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
			get { return "/Controls;component/Images/Code.png"; }
		}
	}
}
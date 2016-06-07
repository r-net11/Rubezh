using System;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using StrazhAPI.Automation;
using StrazhAPI.Enums;

namespace SoundsModule.Validation
{
	class SoundValidationError : ObjectValidationError<AutomationSound, ShowSoundsEvent, Guid>
	{
		public SoundValidationError(AutomationSound sound, string error, ValidationErrorLevel level)
			: base(sound, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Sounds; }
		}
		protected override Guid Key
		{
			get { return Object.Uid; }
		}
		public override string Address
		{
			get { return ""; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Music.png"; }
		}
	}
}
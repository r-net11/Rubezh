using System;
using RubezhAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Validation;
using Infrastructure.Events;

namespace VideoModule.Validation
{
	class VideoValidationError : ObjectValidationError<Camera, ShowVideoEvent, Guid>
	{
		public VideoValidationError(Camera camera, string error, ValidationErrorLevel level)
			: base(camera, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Video; }
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
			get { return Object.Ip; }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Video1.png"; }
		}
	}
}

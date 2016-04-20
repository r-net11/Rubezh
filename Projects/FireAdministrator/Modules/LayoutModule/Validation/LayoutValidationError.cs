using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Validation;
using Infrastructure.Events;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows;

namespace LayoutModule.Validation
{
	public class LayoutValidationError : ObjectValidationError<Layout, ShowMonitorLayoutEvent, Guid>
	{
		public LayoutValidationError(Layout layout, string error, ValidationErrorLevel level)
			: base(layout, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Layout; }
		}
		protected override Guid Key
		{
			get { return Object.UID; }
		}
		public override string Source
		{
			get { return Object.Caption; }
		}
		public override string Address
		{
			get { return string.Empty; }
		}
		public override string ImageSource
		{
			get { return "BLayouts"; }
		}
	}
}

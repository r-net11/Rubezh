using System;
using FiresecAPI.Automation;
using Infrastructure.Common.Validation;
using FiltersModule.Events;
using FiresecAPI.Models;

namespace FilterModule.Validation
{
	class FilterValidationError : ObjectValidationError<JournalFilter, ShowFiltersEvent, Guid>
	{
		public FilterValidationError(JournalFilter journalFilter, string error, ValidationErrorLevel level)
			: base(journalFilter, error, level)
		{
		}

		public override string Module
		{
			get { return "Filter"; }
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
			get { return "/Controls;component/Images/SelectNone.png"; }
		}
	}
}
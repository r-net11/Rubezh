using System;
using FiltersModule.Events;
using RubezhAPI.Journal;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Validation;

namespace FilterModule.Validation
{
	class FilterValidationError : ObjectValidationError<JournalFilter, ShowFiltersEvent, Guid>
	{
		public FilterValidationError(JournalFilter journalFilter, string error, ValidationErrorLevel level)
			: base(journalFilter, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Filters; }
		}
		protected override Guid Key
		{
			get { return Object.UID; }
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
			get { return "/Controls;component/Images/BlueFilter.png"; }
		}
	}
}
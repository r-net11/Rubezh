using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<NamedInterval>>>(() => FiresecService.GetNamedIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveNamedIntervals(items));
		}
		public FiresecAPI.OperationResult MarkDeletedNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedNamedIntervals(items));
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<TimeInterval>>>(() => FiresecService.GetTimeIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveTimeIntervals(items));
		}
		public FiresecAPI.OperationResult MarkDeletedTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedTimeIntervals(items));
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public FiresecAPI.OperationResult SaveHolidays(IEnumerable<Holiday> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveHolidays(items));
		}
		public FiresecAPI.OperationResult MarkDeletedHolidays(IEnumerable<Holiday> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedHolidays(items));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleSchemes(IEnumerable<ScheduleScheme> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleSchemes(items));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleSchemes(IEnumerable<ScheduleScheme> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedScheduleSchemes(items));
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveDayIntervals(IEnumerable<DayInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayIntervals(items));
		}
		public FiresecAPI.OperationResult MarkDeletedDayIntervals(IEnumerable<DayInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedDayIntervals(items));
		}
	}
}

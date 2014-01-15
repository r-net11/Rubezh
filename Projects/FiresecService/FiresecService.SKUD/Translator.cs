using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecService.SKUD.DataAccess;

namespace FiresecService.SKUD
{
    class Translator
    {
        DataAccess.SKUDDataContext Context;
        
        public Translator(DataAccess.SKUDDataContext context)
        {
            Context = context;
        }

        public FiresecAPI.Models.Skud.Interval Translate(Interval interval)
        {
            if (interval == null)
                return null;
            FiresecAPI.Models.Skud.Transition transitition;
            switch(interval.Transition)
            {
                case "Day":
                    transitition = FiresecAPI.Models.Skud.Transition.Day;
                    break;
                case "Night":
                    transitition = FiresecAPI.Models.Skud.Transition.Night;
                    break;
                default:
                    transitition = FiresecAPI.Models.Skud.Transition.DayNight;
                    break;
            };
            return new FiresecAPI.Models.Skud.Interval
            {
                Uid = interval.Uid,
                BeginDate = interval.BeginDate,
                EndDate = interval.EndDate,
                Transition = transitition
            };
        }

        public FiresecAPI.Models.Skud.NamedInterval Translate(NamedInterval namedInterval)
        {
            if (namedInterval == null)
                return null;
            var intervals = new List<FiresecAPI.Models.Skud.Interval>();
            foreach (var item in Context.Interval.ToList())
            {
                if (item.NamedIntervalUid == namedInterval.Uid)
                    intervals.Add(Translate(item));
            }
            return new FiresecAPI.Models.Skud.NamedInterval
            {
                Uid = namedInterval.Uid,
                Name = namedInterval.Name,
                Intervals = intervals
            };
        }

        public FiresecAPI.Models.Skud.Day Translate(Day day)
        {
            if (day == null)
                return null;
            var namedInterval = Translate(Context.NamedInterval.ToList().FirstOrDefault(x => x.Uid == day.NamedIntervalUid));
            return new FiresecAPI.Models.Skud.Day
            {
                Uid = day.Uid,
                Number = day.Number,
                NamedInterval = namedInterval
            };
        }

        public FiresecAPI.Models.Skud.ScheduleScheme Translate(ScheduleScheme scheduleScheme)
        {
            if (scheduleScheme == null)
                return null;
            var days = new List<FiresecAPI.Models.Skud.Day>();
            foreach (var item in Context.Day)
	        {
		       if(item.ScheduleSchemeUid == scheduleScheme.Uid)
                   days.Add(Translate(item));
            }
            FiresecAPI.Models.Skud.ScheduleSchemeType type;
            switch(scheduleScheme.Type)
            {
                case "Month":
                    type = FiresecAPI.Models.Skud.ScheduleSchemeType.Month;
                    break;
                case "Shift":
                    type = FiresecAPI.Models.Skud.ScheduleSchemeType.Shift;
                    break;
                default:
                    type = FiresecAPI.Models.Skud.ScheduleSchemeType.Week;
                    break;
            }
            return new FiresecAPI.Models.Skud.ScheduleScheme
            {
                Uid = scheduleScheme.Uid,
                Name = scheduleScheme.Name,
                Days = days,
                Type = type
            };
        }

        public FiresecAPI.Models.Skud.RegisterDevice Translate(RegisterDevice registerDevice)
        {
            if (registerDevice == null)
                return null;
            bool canControl = false;
            if(registerDevice.CanControl == true)
                canControl = true;
            return new FiresecAPI.Models.Skud.RegisterDevice
            {
                Uid = registerDevice.Uid,
                CanControl = canControl
            };
        }

        public FiresecAPI.Models.Skud.Schedule Translate(Schedule schedule)
        {
            if (schedule == null)
                return null;
            var registerDevices = new List<FiresecAPI.Models.Skud.RegisterDevice>();
            schedule.RegisterDevice.ToList().ForEach(x => registerDevices.Add(Translate(x)));
            return new FiresecAPI.Models.Skud.Schedule
            {
                Uid = schedule.Uid,
                Name = schedule.Name,
                ScheduleScheme = Translate(schedule.ScheduleScheme),
                RegisterDevices = registerDevices
            };
        }

        public FiresecAPI.Models.Skud.Position Translate(Position position)
        {
            if (position == null)
                return null;
            return new FiresecAPI.Models.Skud.Position
            {
                Uid = position.Uid,
                Name = position.Name,
                Description = position.Description
            };
        }

        public FiresecAPI.Models.Skud.AdditionalColumn Translate(AdditionalColumn additionalColumn)
        {
            if (additionalColumn == null)
                return null;
            FiresecAPI.Models.Skud.AdditionalColumnType type;
            switch (additionalColumn.Type)
            {
                case "Graphics":
                    type = FiresecAPI.Models.Skud.AdditionalColumnType.Graphics;
                    break;
                case "Mixed":
                    type = FiresecAPI.Models.Skud.AdditionalColumnType.Mixed;
                    break;
                default:
                    type = FiresecAPI.Models.Skud.AdditionalColumnType.Text;
                    break;
            }
            return new FiresecAPI.Models.Skud.AdditionalColumn
            {
                Uid = additionalColumn.Uid,
                Name = additionalColumn.Name,
                Description = additionalColumn.Description,
                TextData = additionalColumn.TextData,
                GraphicsData = additionalColumn.GraphicsData.ToArray(),
                Type = type
            };
        }

        public FiresecAPI.Models.Skud.Phone Translate(Phone phone)
        {
            if (phone == null)
                return null;
            return new FiresecAPI.Models.Skud.Phone
            {
                Uid = phone.Uid,
                Name = phone.Name,
                NumberString = phone.NumberString
            };
        }

        public FiresecAPI.Models.Skud.Department Translate(Department department, FiresecAPI.Models.Skud.Employee contactEmployee = null, FiresecAPI.Models.Skud.Employee attendantEmployee = null)
        {
            if (department == null)
                return null;
            var phones = new List<FiresecAPI.Models.Skud.Phone>();
            department.Phone.ToList().ForEach(x => phones.Add(Translate(x)));
            var resultDepartment = new FiresecAPI.Models.Skud.Department
            {
                Uid = department.Uid,
                Name = department.Name,
                Description = department.Description,
                ParentDepartment = Translate(department.Department1),
                Phones = phones
            };
            if (contactEmployee != null)
                resultDepartment.ContactEmployee = contactEmployee;
            if (attendantEmployee != null)
                resultDepartment.AttendantEmployee = attendantEmployee;
            return resultDepartment;
        }

        public FiresecAPI.Models.Skud.EmployeeReplacement Translate(EmployeeReplacement employeeReplacement)
        {
            if (employeeReplacement == null)
                return null;
            return new FiresecAPI.Models.Skud.EmployeeReplacement
            {
                Uid = employeeReplacement.Uid,
                BeginDate = employeeReplacement.BeginDate,
                EndDate = employeeReplacement.EndDate,
                Department = Translate(employeeReplacement.Department),
                Schedule = Translate(employeeReplacement.Schedule)
            };
        }

        public FiresecAPI.Models.Skud.Employee Translate(Employee employee)
        {
            if (employee == null)
                return null;
            var additionalColumns = new List<FiresecAPI.Models.Skud.AdditionalColumn>();
            employee.AdditionalColumn.ToList().ForEach(x => additionalColumns.Add(Translate(x)));
            var resultEmployee = new FiresecAPI.Models.Skud.Employee
            {
                Uid = employee.Uid,
                FirstName = employee.FirstName,
                SecondName = employee.SecondName,
                LastName = employee.LastName,
                AdditionalColumns = additionalColumns,
                //Department = department,
                Appointed = employee.Appointed,
                Dismissed = employee.Dismissed,
                Position = Translate(employee.Position),
                Replacement = Translate(employee.EmployeeReplacement),
                Schedule = Translate(employee.Schedule)
            };

            FiresecAPI.Models.Skud.Department department;
            if (employee.Department2.AttendantUid == employee.Uid ||
                employee.Department2.ContactEmployeeUid == employee.Uid)
                department = Translate(employee.Department2, resultEmployee, resultEmployee);
            else if (employee.Department2.AttendantUid == employee.Uid)
                department = Translate(employee.Department2, null, resultEmployee);
            else if (employee.Department2.ContactEmployeeUid == employee.Uid)
                department = Translate(employee.Department2, resultEmployee);
            else
                department = Translate(employee.Department2);

            resultEmployee.Department = department;

            return resultEmployee;
        }

        public FiresecAPI.Models.Skud.Holiday Translate(Holiday holiday)
        {
            if (holiday == null)
                return null;
            FiresecAPI.Models.Skud.HolidayType type;
            switch (holiday.Type)
            {
                case "Holiday":
                    type = FiresecAPI.Models.Skud.HolidayType.Holiday;
                    break;
                case "Reduced":
                    type = FiresecAPI.Models.Skud.HolidayType.Reduced;
                    break;
                case "Transferred":
                    type = FiresecAPI.Models.Skud.HolidayType.Transferred;
                    break;
                default:
                    type = FiresecAPI.Models.Skud.HolidayType.Working;
                    break;
            }
            return new FiresecAPI.Models.Skud.Holiday
            {
                Uid = holiday.Uid,
                Name = holiday.Name,
                Date = holiday.Date,
                TransferDate = holiday.TransferDate,
                Reduction = holiday.Reduction,
                Type = type
            };
        }

        public FiresecAPI.Models.Skud.Document Translate(Document document)
        {
            if (document == null)
                return null;
            return new FiresecAPI.Models.Skud.Document
            {
                Uid = document.Uid,
                Name = document.Name,
                Description = document.Description,
                IssueDate = document.IssueDate,
                LaunchDate = document.LaunchDate
            };
        }
    }
}

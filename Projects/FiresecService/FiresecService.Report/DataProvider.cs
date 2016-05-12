using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using FiresecService.Report.Model;
using RubezhDAL;

namespace FiresecService.Report
{
	public class DataProvider : IDisposable
	{
		public DataProvider()
		{
			DbService = new RubezhDAL.DataClasses.DbService();
			IsCacheLoaded = false;
		}

		public RubezhDAL.DataClasses.DbService DbService { get; private set; }
		public bool IsCacheLoaded { get; private set; }

		public Dictionary<Guid, DeletableObjectInfo<Organisation>> Organisations { get; private set; }
		public Dictionary<Guid, OrganisationBaseObjectInfo<Department>> Departments { get; private set; }
		public Dictionary<Guid, OrganisationBaseObjectInfo<Position>> Positions { get; private set; }
		private Dictionary<Guid, EmployeeInfo> _employees;

		public void LoadCache()
		{
			if (IsCacheLoaded)
				return;

			_employees = new Dictionary<Guid, EmployeeInfo>();

			var organisationResult = DbService.OrganisationTranslator.Get(new OrganisationFilter() { LogicalDeletationType = LogicalDeletationType.All });
			Organisations = CreateDictionary(organisationResult, organisation => new DeletableObjectInfo<Organisation>()
			{
				IsDeleted = organisation.IsDeleted,
				Name = organisation.Name,
				UID = organisation.UID,
				Item = organisation,
			});

			var departmentResult = DbService.DepartmentTranslator.Get(new DepartmentFilter() { LogicalDeletationType = LogicalDeletationType.All });
			Departments = CreateDictionary(departmentResult, department => new OrganisationBaseObjectInfo<Department>()
			{
				IsDeleted = department.IsDeleted,
				Name = department.Name,
				Organisation = Organisations[department.OrganisationUID].Name,
				OrganisationUID = department.OrganisationUID,
				UID = department.UID,
				Item = department,
			});

			var positionResult = DbService.PositionTranslator.Get(new PositionFilter() { LogicalDeletationType = LogicalDeletationType.All });
			Positions = CreateDictionary(positionResult, position => new OrganisationBaseObjectInfo<Position>()
			{
				IsDeleted = position.IsDeleted,
				Name = position.Name,
				Organisation = Organisations[position.OrganisationUID].Name,
				OrganisationUID = position.OrganisationUID,
				UID = position.UID,
				Item = position,
			});
		}
		public EmployeeInfo GetEmployee(Guid uid)
		{
			if (!_employees.ContainsKey(uid))
			{
				var result = DbService.EmployeeTranslator.GetSingle(uid);
				_employees.Add(uid, result == null ? null : ConvertEmployee(result.Result));
			}
			return _employees[uid];
		}
		public List<EmployeeInfo> GetEmployees(IEnumerable<Guid> uids)
		{
			if (uids.IsEmpty())
				return new List<EmployeeInfo>();
			LoadCache();
			int partSize = 2000;
			if (uids.Count() > partSize)
			{
				var uidList = uids.ToList();
				var result = new List<EmployeeInfo>();
				for (int i = 0; i < uidList.Count; i += partSize)
				{
					var uidSubList = uidList.GetRange(i, Math.Min(partSize, uidList.Count - i));
					result.AddRange(GetEmployeesPart(uidSubList));
				}
				return result;
			}
			else
			{
				return GetEmployeesPart(uids);
			}
		}
		List<EmployeeInfo> GetEmployeesPart(IEnumerable<Guid> uids)
		{
			var employeeFilter = new EmployeeFilter()
			{
				UIDs = uids.ToList(),
				LogicalDeletationType = LogicalDeletationType.All,
			};
			var employeesResult = DbService.EmployeeTranslator.Get(employeeFilter);
			if (employeesResult == null || employeesResult.Result == null)
				return new List<EmployeeInfo>();
			var employees = employeesResult.Result.Select(ConvertEmployee).ToList();
			employees.ForEach(employee =>
			{
				if (_employees.ContainsKey(employee.UID))
					_employees[employee.UID] = employee;
				else
					_employees.Add(employee.UID, employee);
			});
			return employees;
		}
		public List<EmployeeInfo> GetEmployees(SKDReportFilter filter)
		{
			var list = new List<EmployeeInfo>();
			var employeeFilter = GetEmployeeFilter(filter);
            return GetEmployees(employeeFilter, filter.IsDefault);
        }
        public List<EmployeeInfo> GetEmployees(EmployeeFilter employeeFilter, bool isDefault)
        {
			LoadCache();
			CheckIfNoOrgansations(employeeFilter, isDefault);
            var employeesResult = DbService.EmployeeTranslator.Get(employeeFilter);
			if (employeesResult == null || employeesResult.Result == null)
				return new List<EmployeeInfo>();
			var employees = employeesResult.Result.Select(ConvertEmployee).ToList();
			employees.ForEach(employee =>
			{
				if (_employees.ContainsKey(employee.UID))
					_employees[employee.UID] = employee;
				else
					_employees.Add(employee.UID, employee);
			});
			return employees;
		}

		void CheckIfNoOrgansations(EmployeeFilter employeeFilter, bool isDefault)
		{
			if (employeeFilter.OrganisationUIDs.Count == 1 && isDefault)
			{
				var organisation = Organisations.FirstOrDefault(x => x.Key == employeeFilter.OrganisationUIDs.FirstOrDefault()).Value.Item;
				if (organisation == null || employeeFilter.User != null || !employeeFilter.User.IsAdm || !organisation.UserUIDs.Any(x => x == employeeFilter.User.UID))
					employeeFilter.OrganisationUIDs = new List<Guid>();
			}
		}

		public EmployeeFilter GetCardEmployeeFilter(SKDReportFilter filter)
		{
			LoadCache();
			var employeeFilter = GetEmployeeFilter(filter);
			CheckIfNoOrgansations(employeeFilter, filter.IsDefault);
			return employeeFilter;
		}

        public List<SKDCard> GetCards(CardFilter filter)
        {
            var result = DbService.CardTranslator.Get(filter);
            if (result.HasError)
                return new List<SKDCard>();
            return result.Result;
        }

		public EmployeeFilter GetEmployeeFilter(SKDReportFilter filter)
		{
			var employeeFilter = new EmployeeFilter();
			var withDeleted = filter is IReportFilterArchive ? ((IReportFilterArchive)filter).UseArchive : false;
			employeeFilter.LogicalDeletationType = withDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			employeeFilter.User = filter.User;
			if (filter is IReportFilterOrganisation)
			{
				employeeFilter.OrganisationUIDs = ((IReportFilterOrganisation)filter).Organisations ?? new List<Guid>();
			}
			if (filter is IReportFilterDepartment)
				employeeFilter.DepartmentUIDs = ((IReportFilterDepartment)filter).Departments ?? new List<Guid>();
			if (filter is IReportFilterPosition)
				employeeFilter.PositionUIDs = ((IReportFilterPosition)filter).Positions ?? new List<Guid>();
			if (filter is IReportFilterEmployee)
			{
				var reportFilterEmployee = (IReportFilterEmployee)filter;
				if (reportFilterEmployee.IsSearch)
				{
					employeeFilter.LastName = reportFilterEmployee.LastName;
					employeeFilter.FirstName = reportFilterEmployee.FirstName;
					employeeFilter.SecondName = reportFilterEmployee.SecondName;
				}
				else
					employeeFilter.UIDs = reportFilterEmployee.Employees ?? new List<Guid>();
				if (filter is IReportFilterEmployeeAndVisitor)
					employeeFilter.PersonType = ((IReportFilterEmployeeAndVisitor)filter).IsEmployee ? PersonType.Employee : PersonType.Guest;
			}
			return employeeFilter;
		}
		public bool IsEmployeeFilter(SKDReportFilter filter)
		{
			if ((filter is IReportFilterOrganisation && !((IReportFilterOrganisation)filter).Organisations.IsEmpty()) ||
				(filter is IReportFilterDepartment && !((IReportFilterDepartment)filter).Departments.IsEmpty()) ||
				(filter is IReportFilterPosition && !((IReportFilterPosition)filter).Positions.IsEmpty()))
				return true;
			var employeeFilter = filter as IReportFilterEmployee;
			if (employeeFilter == null)
				return false;
			return (!employeeFilter.IsSearch && !employeeFilter.Employees.IsEmpty()) || 
				(employeeFilter.IsSearch && (!employeeFilter.LastName.IsEmpty() || !employeeFilter.FirstName.IsEmpty() || !employeeFilter.SecondName.IsEmpty()));
		}

		#region IDisposable Members

		public void Dispose()
		{
			DbService.Dispose();
		}

		#endregion

		private EmployeeInfo ConvertEmployee(Employee employee)
		{
			if (employee == null)
				return null;
			return new EmployeeInfo()
				 {
					 Department = employee.DepartmentName,
					 DepartmentUID = employee.DepartmentUID,
					 IsDeleted = employee.IsDeleted,
					 Name = employee.FIO,
					 Organisation = Organisations[employee.OrganisationUID].Name,
					 OrganisationUID = employee.OrganisationUID,
					 Position = employee.PositionName,
					 PositionUID = employee.PositionUID,
					 UID = employee.UID,
					 Item = employee,
				 };
		}
		private Dictionary<Guid, T> CreateDictionary<ApiT, T>(OperationResult<IEnumerable<ApiT>> result, Converter<ApiT, T> converter)
			where T : ObjectInfo
			where ApiT : SKDModelBase
		{
			if (result == null || result.Result == null)
				return new Dictionary<Guid, T>();
			return result.Result.ToDictionary(item => item.UID, item => converter(item));
		}

        private Dictionary<Guid, T> CreateDictionary<ApiT, T>(OperationResult<List<ApiT>> result, Converter<ApiT, T> converter)
            where T : ObjectInfo
            where ApiT : SKDModelBase
        {
            if (result == null || result.Result == null)
                return new Dictionary<Guid, T>();
            return result.Result.ToDictionary(item => item.UID, item => converter(item));
        }
	}
}

using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD.Translator;

namespace FiresecService.SKUD
{
	public class FiresecServiceSKUD : IFiresecServiceSKUD
	{
		private DataAccess.FiresecDataContext _context = null;

		public DataAccess.FiresecDataContext Context
		{
			get
			{
				if (_context == null)
					//lock (this)
						if (_context == null)
							_context = new DataAccess.FiresecDataContext();
				return _context;
			}
		}


		#region IFiresecServiceSKUD Members

		public IEnumerable<EmployeeCard> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return EmployeeResultTranslator.Translate(Context.GetAllEmployees(filter.ClockNumber, filter.LastName, filter.FirstName, filter.SecondName, filter.GroupId, filter.DepartmentId, filter.PositionId));
		}

		public bool DeleteEmployee(int id)
		{
			int? rowCount = Context.DeleteEmployee(id);
			return rowCount.HasValue && rowCount == 1;
		}

		public EmployeeCardDetails GetEmployeeCard(int id)
		{
			return EmployeeResultTranslator.Translate(Context.GetEmployeeCard(id).FirstOrDefault());
		}

		public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
		{
			int? id = employeeCard.Id;
			Context.SaveEmployeeCard(
				ref id,
				employeeCard.LastName,
				employeeCard.FirstName,
				employeeCard.SecondName,
				employeeCard.ClockNumber,
				employeeCard.Comment,
				employeeCard.DepartmentId,
				employeeCard.Email,
				employeeCard.GroupId,
				employeeCard.Phone,
				employeeCard.PositionId,
				employeeCard.Address,
				employeeCard.AddressFact,
				employeeCard.BirthPlace,
				employeeCard.Birthday,
				employeeCard.Cell,
				employeeCard.ITN,
				employeeCard.PassportCode,
				employeeCard.PassportDate,
				employeeCard.PassportEmitter,
				employeeCard.PassportNumber,
				employeeCard.PassportSerial,
				employeeCard.Photo,
				employeeCard.SNILS,
				employeeCard.SexId);

			return id.HasValue ? id.Value : -1;
		}

		public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
		{
			return EmployeeDepartmentResultTranslator.Translate(Context.GetDepartments());
		}

		public IEnumerable<EmployeeGroup> GetEmployeeGroups()
		{
			return EmployeeGroupResultTranslator.Translate(Context.GetGroups());
		}

		public IEnumerable<EmployeePosition> GetEmployeePositions()
		{
			return EmployeePositionResultTranslator.Translate(Context.GetPositions());
		}

		#endregion
	}
}
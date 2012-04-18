using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
					lock (this)
						if (_context == null)
							_context = new DataAccess.FiresecDataContext();
				return _context;
			}
		}


		#region IFiresecServiceSKUD Members

		public IEnumerable<EmployeeCardIndex> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return EmployeeResultTranslator.Translate(Context.GetAllEmployees());
		}

		public bool DeleteEmployee(int id)
		{
			int? rowCount = null;
			Context.DeleteEmployee(id, ref rowCount);
			return rowCount.HasValue && rowCount == 1;
		}

		public EmployeeCard GetEmployeeCard(int id)
		{
			return EmployeeResultTranslator.Translate(Context.GetEmployeeCard(id).FirstOrDefault());
		}

		public int SaveEmployeeCard(EmployeeCard employeeCard)
		{
			int? id = employeeCard.Id;
			Context.SaveEmployeeCard(
				ref id,
				employeeCard.LastName,
				employeeCard.FirstName,
				employeeCard.SecondName,
				null,
				null,
				employeeCard.Department,
				employeeCard.Position,
				employeeCard.Comment);
			return id.HasValue ? id.Value : -1;
		}

		#endregion
	}
}

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

		public IEnumerable<EmployeeCardIndex> GetEmployees()
		{
			return EmployeeResultTranslator.Translate(Context.GetAllEmployees());
		}

		public ActionResult DeleteEmployee(int id)
		{
			int? rowCount = null;
			string error = null;
			Context.DeleteEmployee(id, ref rowCount, ref error);
			return new ActionResult()
			{
				Error = error,
				RowCount = rowCount ?? 0
			};
		}

		#endregion
	}
}

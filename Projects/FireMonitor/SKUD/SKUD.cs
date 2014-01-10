using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SKUD
{
	public class SKUD
	{
		public DataAccess.SKUDDataContext Context { get; private set; }

		public SKUD()
		{
			Context = new DataAccess.SKUDDataContext();
		}

		

		public void Test()
		{
			//var employees = Context.GetEmployees();
			foreach (var item in Context.Employee.ToList())
			{
				Trace.WriteLine(item.FirstName + " " + item.SecondName + " " + item.LastName);
			}
		}
	}
}

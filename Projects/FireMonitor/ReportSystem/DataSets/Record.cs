using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.CodeParser;

namespace ReportSystem.DataSets
{
	public class Record
	{
		public Record(string firstName, string secondName, string middleName)
		{
			FirstName = firstName;
			SecondName = secondName;
			MiddleName = middleName;
		}

		public string FirstName { get; private set; }
		public string SecondName { get; private set; }
		public string MiddleName { get; private set; }
	}
}

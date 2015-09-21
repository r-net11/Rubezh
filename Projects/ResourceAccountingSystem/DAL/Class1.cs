using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Class1
    {
		public static void Test()
		{
			using (var databaseContext = DAL.DatabaseContext.Initialize())
			{
				databaseContext.Test();
			}
		}

    }
}

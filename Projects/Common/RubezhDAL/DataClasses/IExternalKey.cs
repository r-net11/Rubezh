using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhDAL.DataClasses
{
	public interface IExternalKey
	{
		Guid UID { get; set; }
		string ExternalKey { get; set; }
		bool IsDeleted { get; set; }
		DateTime? RemovalDate { get; set; }
	}
}

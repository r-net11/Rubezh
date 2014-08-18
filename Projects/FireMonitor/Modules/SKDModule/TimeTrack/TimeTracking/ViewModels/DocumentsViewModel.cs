using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }

		public DocumentsViewModel(ShortEmployee shortEmployee)
		{
			ShortEmployee = shortEmployee;

		}


	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IHeaderViewModel
	{
		string Title { get; set; }
		double Height { get; }
	}
}

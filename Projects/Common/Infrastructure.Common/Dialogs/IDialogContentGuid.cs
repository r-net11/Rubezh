using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
	public interface IDialogContentGuid  : IDialogContent
	{
		Guid Guid { get; }
	}
}

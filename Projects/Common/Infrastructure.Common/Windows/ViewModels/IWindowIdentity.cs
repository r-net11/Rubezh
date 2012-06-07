using System;

namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IWindowIdentity
	{
		Guid Guid { get; }
	}
}

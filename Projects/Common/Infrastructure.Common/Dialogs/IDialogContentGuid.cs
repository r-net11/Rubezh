using System;

namespace Infrastructure.Common
{
	public interface IDialogContentGuid : IDialogContent
	{
		Guid Guid { get; }
	}
}
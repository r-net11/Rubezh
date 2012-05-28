using System;

namespace Infrastructure
{
	public interface IWaitService
	{
		bool Execute(Action action);
	}
}
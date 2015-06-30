using System;

namespace Infrastructure
{
	public interface IProgressService
	{
		void Run(Action work, Action completed, string tite);
	}
}
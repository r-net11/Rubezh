using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Navigation;

namespace Infrastructure.Common
{
	public interface IModule
	{
		string Name { get; }
		void RegisterResource();
		void Initialize();
		IEnumerable<NavigationItem> CreateNavigation();
	}
}

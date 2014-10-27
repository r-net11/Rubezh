using System;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace Infrastructure.Common
{
	public interface IModule : IDisposable
	{
		string Name { get; }
        ModuleType ModuleType { get; }
		int Order { get; }
		void RegisterResource();
		bool BeforeInitialize(bool firstTime);
		void Initialize();
		void AfterInitialize();
		void CreateViewModels();
		IEnumerable<NavigationItem> CreateNavigation();
	}
}
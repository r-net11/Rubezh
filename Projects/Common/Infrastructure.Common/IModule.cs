using FiresecAPI.Enums;
using Infrastructure.Common.Navigation;
using System;
using System.Collections.Generic;

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
using System.Collections.Generic;
using Infrastructure.Common.Navigation;
using System;

namespace Infrastructure.Common
{
    public interface IModule : IDisposable
    {
        string Name { get; }
        void RegisterResource();
        void Initialize();
        IEnumerable<NavigationItem> CreateNavigation();
    }
}
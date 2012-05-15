using System.Collections.Generic;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;

namespace Infrastructure
{
	public abstract class ModuleBase : IModule
	{
		public ModuleBase()
		{
			RegisterResource();
		}

		#region IModule Members

		public virtual string Name
		{
			get { return GetType().Name; }
		}

		public virtual void RegisterResource()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}

		public abstract void Initialize();
		public abstract IEnumerable<NavigationItem> CreateNavigation();

		#endregion
	}
}
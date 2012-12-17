using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace Infrastructure.Common
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

		public virtual int Order
		{
			get { return 10; }
		}

		public virtual void RegisterResource()
		{
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}

		public virtual bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		public virtual void AfterInitialize()
		{
		}

		public abstract void CreateViewModels();
		public abstract void Initialize();
		public abstract IEnumerable<NavigationItem> CreateNavigation();

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion
	}
}
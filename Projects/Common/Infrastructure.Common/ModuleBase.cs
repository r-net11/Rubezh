using StrazhAPI;
using StrazhAPI.Enums;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;

namespace Infrastructure.Common
{
	public abstract class ModuleBase : IModule
	{
		protected ModuleBase()
		{
			RegisterResource();
		}

		#region IModule Members

		public string Name
		{
			get { return ModuleType.ToDescription(); }
		}

		public abstract ModuleType ModuleType { get; }

		public virtual int Order //TODO: Remove this field
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

		#endregion IModule Members

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion IDisposable Members
	}
}
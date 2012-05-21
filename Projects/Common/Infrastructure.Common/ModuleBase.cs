using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public virtual void RegisterResource()
		{
			ResourceService resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}

		public abstract void Initialize();
		public abstract IEnumerable<NavigationItem> CreateNavigation();

		#endregion
	}
}

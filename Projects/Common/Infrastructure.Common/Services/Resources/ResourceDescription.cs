using System;
using System.Reflection;

namespace Infrastructure.Common
{
	public class ResourceDescription
	{
		public ResourceDescription(Assembly callerAssembly, string name)
		{
			Source = ResourceHelper.ComposeResourceUri(callerAssembly, name);
		}

		public Uri Source { get; private set; }
	}
}
using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKModuleTest
{
	public class MockResourceService : IResourceService
	{
		public void AddResource(System.Reflection.Assembly callerAssembly, string name)
		{
		}
	}
}
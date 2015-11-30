
using System.Reflection;
namespace Infrastructure.Common
{
	public interface IResourceService
	{
		void AddResource(Assembly callerAssembly, string name);
	}
}
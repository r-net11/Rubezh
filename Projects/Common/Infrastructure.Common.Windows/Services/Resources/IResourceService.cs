
using System.Reflection;
namespace Infrastructure.Common.Windows
{
	public interface IResourceService
	{
		void AddResource(Assembly callerAssembly, string name);
	}
}
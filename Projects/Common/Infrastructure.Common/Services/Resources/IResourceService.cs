using System.Windows;

namespace Infrastructure.Common
{
	public interface IResourceService
	{
		void AddResource(ResourceDescription description);
	}
}
namespace Infrastructure.Common.Windows.Services
{
	public interface ISecurityService
	{
		bool Validate(bool flag = true);
	}
}
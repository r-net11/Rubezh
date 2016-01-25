namespace Infrastructure.Common.Services
{
	public interface ISecurityService
	{
		bool Validate(bool flag = true);
	}
}
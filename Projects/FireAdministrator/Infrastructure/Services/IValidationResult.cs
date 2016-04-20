using Infrastructure.Common;
namespace Infrastructure
{
	public interface IValidationResult
	{
		bool HasErrors(ModuleType? module = null);
		bool CannotSave(ModuleType? module = null);
		bool CannotWrite(ModuleType? module = null);
	}
}
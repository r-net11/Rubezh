namespace Infrastructure
{
	public interface IValidationResult
	{
		bool HasErrors(string module = null);
		bool CannotSave(string module = null);
		bool CannotWrite(string module = null);
	}
}
namespace Infrastructure
{
	public interface IValidationResult
	{
		bool HasErrors { get; }
		bool CannotSave { get; }
		bool CannotWrite { get; }
	}
}
namespace Infrastructure.Common.Windows.Validation
{
	public interface IValidationError
	{
		ModuleType Module { get; }
		string Source { get; }
		string Address { get; }
		string Error { get; }
		string ImageSource { get; }
		ValidationErrorLevel ErrorLevel { get; }
		void Navigate();
	}
}
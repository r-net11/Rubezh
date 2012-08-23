
namespace Infrastructure.Common.Validation
{
    public interface IValidationError
    {
		string Module { get; }
        string Source { get; }
        string Address { get; }
        string Error { get; }
        ValidationErrorLevel ErrorLevel { get; }
        void Navigate();
    }
}
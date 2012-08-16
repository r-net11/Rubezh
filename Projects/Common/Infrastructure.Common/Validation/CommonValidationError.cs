
namespace Infrastructure.Common.Validation
{
	public class CommonValidationError : IValidationError
	{
		public CommonValidationError(string error, ValidationErrorLevel validationErrorLevel)
		{
			Error = error;
			ErrorLevel = validationErrorLevel;
			Source = string.Empty;
			Address = string.Empty;
		}
		public CommonValidationError(string source, string address, string error, ValidationErrorLevel validationErrorLevel)
			: this(error, validationErrorLevel)
		{
			Source = source;
			Address = address;
		}

		#region IValidationError Members

		public virtual string Source { get; protected set; }

		public virtual string Address { get; protected set; }

		public string Error { get; private set; }
		public ValidationErrorLevel ErrorLevel { get; private set; }

		public virtual void Navigate()
		{

		}

		#endregion
	}
}

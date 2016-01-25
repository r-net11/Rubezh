
namespace Infrastructure.Common.Validation
{
	public class CommonValidationError : IValidationError
	{
		public CommonValidationError(string error, ValidationErrorLevel validationErrorLevel)
		{
			Error = error;
			ErrorLevel = validationErrorLevel;
			Source = string.Empty;
			ImageSource = string.Empty;
			Address = string.Empty;
		}
		public CommonValidationError(ModuleType module, string source, string address, string error, ValidationErrorLevel validationErrorLevel)
			: this(error, validationErrorLevel)
		{
			Module = module;
			Source = source;
			Address = address;
		}

		#region IValidationError Members

		public virtual ModuleType Module { get; protected set; }
		public virtual string Source { get; protected set; }
		public virtual string Address { get; protected set; }
		public virtual string ImageSource { get; protected set; }

		public string Error { get; private set; }
		public ValidationErrorLevel ErrorLevel { get; private set; }

		public virtual void Navigate()
		{

		}

		#endregion
	}
}
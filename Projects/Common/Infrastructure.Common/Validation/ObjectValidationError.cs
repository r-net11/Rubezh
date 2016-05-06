using StrazhAPI.Enums;
using Infrastructure.Common.Services;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Validation
{
	public abstract class ObjectValidationError<TObject, TEvent, TKey> : IValidationError
		where TEvent : CompositePresentationEvent<TKey>, new()
	{
		public ObjectValidationError(TObject obj, string error, ValidationErrorLevel validationErrorLevel)
		{
			Object = obj;
			Error = error;
			ErrorLevel = validationErrorLevel;
		}

		#region IValidationError Members

		public abstract ModuleType Module { get; }

		public abstract string Source { get; }

		public abstract string ImageSource { get; }

		public virtual string Address { get { return Key.ToString(); } }

		public string Error { get; private set; }

		public ValidationErrorLevel ErrorLevel { get; private set; }

		public virtual void Navigate()
		{
			ServiceFactoryBase.Events.GetEvent<TEvent>().Publish(Key);
		}

		#endregion IValidationError Members

		protected abstract TKey Key { get; }

		public TObject Object { get; private set; }
	}
}
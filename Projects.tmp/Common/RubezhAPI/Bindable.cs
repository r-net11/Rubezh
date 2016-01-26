
using System.Runtime.Serialization;

namespace RubezhAPI
{
	[DataContract]
	public abstract class Bindable : IBindable
	{
		public event ValueChangedEventHandler ValueChanged;

		public void OnValueChanged()
		{
			if (ValueChanged != null)
				ValueChanged();
		}
	}
}


namespace RubezhAPI
{
	public interface IBindable
	{
		event ValueChangedEventHandler ValueChanged;
		void OnValueChanged();
	}

	public delegate void ValueChangedEventHandler();
}

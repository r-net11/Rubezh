namespace Infrastructure.Common.Windows.Windows.ViewModels
{
	public interface ISelectable<T>
	{
		void Select(T item);
	}
}
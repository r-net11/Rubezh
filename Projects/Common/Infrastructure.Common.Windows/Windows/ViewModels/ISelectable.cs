namespace Infrastructure.Common.Windows.ViewModels
{
	public interface ISelectable<T>
	{
		void Select(T item);
	}
}
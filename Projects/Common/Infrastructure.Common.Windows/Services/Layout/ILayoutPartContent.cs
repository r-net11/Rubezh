
namespace Infrastructure.Common.Windows.Services.Layout
{
	public interface ILayoutPartContent
	{
		ILayoutPartContainer Container { get; }
		void SetLayoutPartContainer(ILayoutPartContainer container);
	}
}


namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartContent
	{
		ILayoutPartContainer Container { get; }
		void SetLayoutPartContainer(ILayoutPartContainer container);
	}
}

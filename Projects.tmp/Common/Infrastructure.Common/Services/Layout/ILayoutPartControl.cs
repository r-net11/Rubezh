using RubezhAPI.Automation;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartControl
	{
		object GetProperty(LayoutPartPropertyName property);
		void SetProperty(LayoutPartPropertyName property, object value);
	}
}

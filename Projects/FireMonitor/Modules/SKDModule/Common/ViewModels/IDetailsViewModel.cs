using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public interface IDetailsViewModel<ModelT>
		where ModelT : class
	{
		ModelT Model { get; }
		void Initialize(Organisation organisation, ModelT model, ViewPartViewModel parentViewModel);
	}
}
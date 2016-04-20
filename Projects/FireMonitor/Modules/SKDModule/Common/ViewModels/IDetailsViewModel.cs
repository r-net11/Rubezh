using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public interface IDetailsViewModel<ModelT>
		where ModelT : class
	{
		ModelT Model { get; }
		bool Initialize(Organisation organisation, ModelT model, ViewPartViewModel parentViewModel);
	}
}
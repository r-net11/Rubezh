using FiresecAPI.SKD;

namespace SKDModule.Common.ViewModels
{
    public interface IDetailsViewModel<ModelT>
        where ModelT : class
    {
        ModelT Model { get; }
        void Initialize(Organisation organisation, ModelT model);
    }
}

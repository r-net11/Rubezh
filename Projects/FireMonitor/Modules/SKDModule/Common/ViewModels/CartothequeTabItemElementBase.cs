
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;
namespace SKDModule.Common.ViewModels
{

    public class CartothequeTabItemElementBase<T, ModelT> : TreeNodeViewModel<T>
        where T : TreeNodeViewModel<T>
        where ModelT : IWithName
    {
        public Organisation Organisation { get; protected set; }
        public bool IsOrganisation { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public ModelT Model { get; protected set; }

        public CartothequeTabItemElementBase()
        {
            ;
        }

        public virtual void InitializeOrganisation(Organisation organisation)
        {
            Organisation = organisation;
            IsOrganisation = true;
            Name = organisation.Name;
            IsExpanded = true;
        }

        public virtual void InitializeModel(Organisation organisation, ModelT model)
        {
            Organisation = organisation;
            Model = model;
            IsOrganisation = false;
            Name = model.Name;
        }

        public virtual void Update(ModelT model)
        {
            Name = model.Name;
            Update();
        }

        public virtual void Update(Organisation organisation)
        {
            Organisation = organisation;
            IsOrganisation = true;
            Name = organisation.Name;
            Description = organisation.Description;
            Update();
        }

        public virtual void Update()
        {
            OnPropertyChanged(() => Name);
            OnPropertyChanged(() => Description);
        }
    }
}

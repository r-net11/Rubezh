using System.Windows.Controls;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class ElementBaseViewModel : TreeBaseViewModel<ElementBaseViewModel>
    {
        public RelayCommand ShowOnPlanCommand { get; protected set; }

		public virtual ContextMenu ContextMenu
		{
			get { return null; }
		}
    }
}

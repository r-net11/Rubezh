using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
    public class DelayRectangleAdorner : BaseRectangleAdorner
    {
        public DelayRectangleAdorner(CommonDesignerCanvas designerCanvas, DelaysViewModel delayViewModel)
            : base(designerCanvas)
        {
            this._delaysViewModel = delayViewModel;
        }

        protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
        {
            var element = new ElementRectangleGKDelay();
            var propertiesViewModel = new DelayPropertiesViewModel(element, _delaysViewModel);
            if (!DialogService.ShowModalWindow(propertiesViewModel))
                return null;
            GKPlanExtension.Instance.SetItem<GKDelay>(element);
            return element;
        }

        #region Fields

        private DelaysViewModel _delaysViewModel;

        #endregion
    }
}

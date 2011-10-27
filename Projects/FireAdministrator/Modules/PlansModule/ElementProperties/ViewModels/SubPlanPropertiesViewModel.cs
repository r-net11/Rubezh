using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace PlansModule.ViewModels
{
    public class SubPlanPropertiesViewModel : SaveCancelDialogContent
    {
        ElementSubPlan _elementSubPlan;

        public SubPlanPropertiesViewModel(ElementSubPlan elementSubPlan)
        {
            Title = "Свойства фигуры: Подплан";
            _elementSubPlan = elementSubPlan;
        }

        protected override void Save(ref bool cancel)
        {
        }
    }
}

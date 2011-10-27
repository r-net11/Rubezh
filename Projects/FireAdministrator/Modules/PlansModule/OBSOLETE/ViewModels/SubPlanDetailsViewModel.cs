using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class SubPlanDetailsViewModel : DialogContent
    {
        public SubPlanDetailsViewModel(Plan parent)
        {
            Title = "Новый субплан";

            _isNew = true;
            ElementSubPlan = new ElementSubPlan();
            if (parent.ElementSubPlans == null) parent.ElementSubPlans = new List<ElementSubPlan>();
            parent.ElementSubPlans.Add(ElementSubPlan);
            Parent = parent;
            Name = Parent.Name;

            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        bool _isNew;
        public ElementSubPlan ElementSubPlan { get; private set; }
        public Plan Parent { get; private set; }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        void Save()
        {
            //ElementSubPlan.Name = Name;
            if (_isNew)
            {
                Plan plan = Parent;
                while (plan.Parent != null) plan = plan.Parent;

                if (FiresecManager.PlansConfiguration.Plans == null) FiresecManager.PlansConfiguration.Plans = new List<FiresecAPI.Models.Plan>();
                int index = FiresecManager.PlansConfiguration.Plans.IndexOf(plan);
                if (index == -1)
                {
                    FiresecManager.PlansConfiguration.Plans.Add(plan);
                }
                else
                {
                    FiresecManager.PlansConfiguration.Plans[index] = plan;
                }
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Save();
            Close(true);
        }

        bool CanSave()
        {
            return Name != null;
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
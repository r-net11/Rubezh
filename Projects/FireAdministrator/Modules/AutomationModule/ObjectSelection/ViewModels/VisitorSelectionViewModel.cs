using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class VisitorSelectionViewModel : SaveCancelDialogViewModel
	{
		private VisitorViewModel _selectedVisitor;

		public VisitorViewModel SelectedVisitor
		{
			get { return _selectedVisitor; }
			set
			{
				if (_selectedVisitor == value)
					return;
				_selectedVisitor = value;
				OnPropertyChanged(() => SelectedVisitor);
			}
		}

		public List<VisitorViewModel> Visitors { get; private set; }

		public VisitorSelectionViewModel(ShortEmployee visitor)
		{
			Title = CommonViewModel.VisitorSelectionViewModel_Title;

			BuildTreeAndSelect(visitor);
		}

		private void BuildTreeAndSelect(ShortEmployee selectedVisitor)
		{
			Visitors = new List<VisitorViewModel>();
			foreach (var organisation in OrganisationHelper.Get(new OrganisationFilter { LogicalDeletationType = LogicalDeletationType.Active }).OrderBy(x => x.Name))
			{
				var rootNode = new VisitorViewModel(organisation);
				foreach(var employee in EmployeeHelper.Get(new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisation.UID }, PersonType = PersonType.Guest, LogicalDeletationType = LogicalDeletationType.Active }))
				{
					var childNode = new VisitorViewModel(employee);
					if (selectedVisitor != null && employee.UID == selectedVisitor.UID)
					{
						SelectedVisitor = childNode;
						rootNode.IsExpanded = true;
					}
					rootNode.AddChild(childNode);
				}
				Visitors.Add(rootNode);
			}
		}

		protected override bool CanSave()
		{
			return SelectedVisitor != null && SelectedVisitor.IsVisitor;
		}
	}
}

using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationCardTemplateViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public PassCardTemplate CardTemplate { get; private set; }

		public OrganisationCardTemplateViewModel(Organisation organisation, PassCardTemplate cardTemplate)
		{
			Organisation = organisation;
			CardTemplate = cardTemplate;
			if (Organisation != null)
			{
				if (Organisation.CardTemplateUIDs == null)
					Organisation.CardTemplateUIDs = new List<Guid>();
			}
			_isChecked = Organisation != null && Organisation.CardTemplateUIDs != null && Organisation.CardTemplateUIDs.Contains(CardTemplate.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");

				if (value)
				{
					if (!Organisation.CardTemplateUIDs.Contains(CardTemplate.UID))
						Organisation.CardTemplateUIDs.Add(CardTemplate.UID);
				}
				else
				{
					if (Organisation.CardTemplateUIDs.Contains(CardTemplate.UID))
						Organisation.CardTemplateUIDs.Remove(CardTemplate.UID);
				}

				var saveResult = OrganisationHelper.SaveCardTemplates(Organisation);
			}
		}
	}
}

using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateSoundName()
		{
			var nameList = new List<string>();
			foreach (var sound in FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				if (nameList.Contains(sound.Name))
					Errors.Add(new SoundValidationError(sound, "Звуковой элемент с таким именем уже существует " + sound.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(sound.Name);
			}
		}
	}
}
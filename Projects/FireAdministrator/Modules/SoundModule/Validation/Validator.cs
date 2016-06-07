using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using StrazhAPI.SKD;

namespace SoundsModule.Validation
{
	public class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			SKDManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			
			ValidateSoundName();
			
			return Errors;
		}
		
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
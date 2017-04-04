using Microsoft.Practices.Prism.Events;
using StrazhAPI.SKD;
using System.Collections.Generic;

namespace SKDModule.Events
{
	public class UpdateAdditionalColumns : CompositePresentationEvent<List<ShortAdditionalColumnType>>
	{
	}
}
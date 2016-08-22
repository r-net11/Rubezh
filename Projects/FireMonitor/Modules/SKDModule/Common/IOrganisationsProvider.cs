using System.Collections.Generic;
using StrazhAPI.SKD;

namespace SKDModule
{
	public interface IOrganisationsProvider
	{
		IEnumerable<Organisation> Get();
	}
}
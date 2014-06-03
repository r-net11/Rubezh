using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class PasswordTranslator:OrganisationElementTranslator<DataAccess.Password, Password, PasswordFilter>
	{
		
		public PasswordTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{
			
		}

		protected override OperationResult CanSave(Password item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && 
				x.OrganisationUID == item.OrganisationUID && 
				x.UID != item.UID &&
				!x.IsDeleted);
			if (sameName)
				return new OperationResult("Попытка добавления пароля с совпадающим названием");
			return base.CanSave(item);
		}

		protected override Password Translate(DataAccess.Password tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.PasswordString = tableItem.PasswordString;
			return result;
		}

		protected override void TranslateBack(DataAccess.Password tableItem, Password apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.PasswordString = apiItem.PasswordString;
		}

		protected override Expression<Func<DataAccess.Password, bool>> IsInFilter(PasswordFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Password>();
			result = result.And(base.IsInFilter(filter));
			return result;
		}
	}
}

using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;

namespace SKDDriver
{
	public class PhotoTranslator : IsDeletedTranslator<DataAccess.Photo, Photo, PhotoFilter>
	{
		public PhotoTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{

		}

		protected override Photo Translate(DataAccess.Photo tableItem)
		{
			var result = base.Translate(tableItem);
			result.Data = tableItem.Data.ToArray();
			return result;
		}

		protected override void TranslateBack(DataAccess.Photo tableItem, Photo apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Data = apiItem.Data;
		}

		protected override Expression<Func<DataAccess.Photo, bool>> IsInFilter(PhotoFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Photo>();
			result = result.And(base.IsInFilter(filter));
			return result;
		}

	}
}

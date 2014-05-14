using System;
using System.Linq.Expressions;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class PhotoTranslator : TranslatorBase<DataAccess.Photo, Photo, PhotoFilter>
	{
		public PhotoTranslator(DataAccess.SKDDataContext context)
			: base(context)	{ }

		protected override Photo Translate(DataAccess.Photo tableItem)
		{
			var result = base.Translate(tableItem);
			result.Data = tableItem.Data.ToArray();
			return result;
		}

		protected override void TranslateBack(DataAccess.Photo tableItem, Photo apiItem)
		{
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
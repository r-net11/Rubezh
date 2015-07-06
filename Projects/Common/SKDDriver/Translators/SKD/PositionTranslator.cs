using FiresecAPI;
using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Serialization;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class PositionTranslator : OrganisationItemTranslatorBase<Position, API.Position, API.PositionFilter>
	{
		DataContractSerializer _serializer;
		public PositionShortTranslator ShortTranslator { get; private set; }
		
		public PositionTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.Position));
			ShortTranslator = new PositionShortTranslator(this);
            AsyncTranslator = new PositionAsyncTranslator(ShortTranslator);
        }

        public PositionAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<Position> Table
		{
			get { return Context.Positions; }
		}

        public override System.Linq.IQueryable<Position> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.Photo);
		}
		
		public override API.Position Translate(Position tableItem)
		{
			var result = base.Translate(tableItem);
			result.Photo = result.Photo = tableItem.Photo != null ? tableItem.Photo.Translate() : null;
			return result;
		}

		public override void TranslateBack(API.Position apiItem, Position tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Photo = Photo.Create(apiItem.Photo);
		}

		protected override void ClearDependentData(Position tableItem)
		{
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}
	}

	public class PositionShortTranslator : OrganisationShortTranslatorBase<Position, API.ShortPosition, API.Position, API.PositionFilter>
	{
		public PositionShortTranslator(PositionTranslator translator) : base(translator) { }
	}

    public class PositionAsyncTranslator : AsyncTranslator<Position, API.ShortPosition, API.PositionFilter>
    {
        public PositionAsyncTranslator(PositionShortTranslator translator) : base(translator as ITranslatorGet<Position, API.ShortPosition, API.PositionFilter>) { }
        public override List<API.ShortPosition> GetCollection(DbCallbackResult callbackResult)
        {
            return callbackResult.Positions;
        }
    }
}
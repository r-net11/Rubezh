using System.Data.Entity;
using System.Runtime.Serialization;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class PositionTranslator : OrganisationItemTranslatorBase<Position, API.Position, API.PositionFilter>
	{
		DataContractSerializer _serializer;
		public ShortPositionTranslator ShortTranslator { get; private set; }
		
		public PositionTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.Position));
			ShortTranslator = new ShortPositionTranslator(this);
		}

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

	public class ShortPositionTranslator : OrganisationShortTranslatorBase<Position, API.ShortPosition, API.Position, API.PositionFilter>
	{
		public ShortPositionTranslator(PositionTranslator translator) : base(translator) { }
	}
}
using System;
using System.Data.Linq;
using System.Linq.Expressions;
using FiresecAPI.SKD;
using LinqKit;
using System.Data.Entity;

namespace SKDDriver.DataClasses
{
	public class PositionSynchroniser : Synchroniser<ExportPosition, Position>
	{
		public PositionSynchroniser(DbSet<Position> table, DbService databaseService) : base(table, databaseService) { }

		public override ExportPosition Translate(Position item)
		{
			return new ExportPosition 
			{ 
				Name = item.Name, 
				Description = item.Description,

				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation)
			};
		}

        //protected override Expression<Func<DataAccess.Position, bool>> IsInFilter(ExportFilter filter)
        //{
        //    return base.IsInFilter(filter).And(x => x.OrganisationUID == filter.OrganisationUID);
        //}

		protected override string Name
		{
			get { return "Positions"; }
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportPosition"; }
		}

		public override void TranslateBack(ExportPosition exportItem, Position tableItem)
		{
			tableItem.Name  = exportItem.Name;
			tableItem.Description = exportItem.Description;

			tableItem.OrganisationUID = GetUIDbyExternalKey(exportItem.OrganisationExternalKey, _DatabaseService.Context.Organisations);
		}
    }
}
